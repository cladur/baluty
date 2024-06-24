using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem.XR;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<TagSpot> tagSpots = new();

    // How many points are awarded for full control of a tag spot
    public float scoreMultiplier = 0.2f;

    // How frequently the scores are updated
    public float scoreUpdateInterval = 1.0f;

    // How frequently enemies are spawned
    public float enemySpawnInterval = 5.0f;

    public float enemySprayDuration = 10.0f;
    public float delayBetweenTagSplines = 3.0f;

    public static float PlayerScore;
    public static float EnemyScore;
    public delegate void ScoreUpdated(float playerScore, float enemyScore);
    public static event ScoreUpdated OnScoreUpdated;

    public int maxEnemies = 3;
    public int CurrentEnemies { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public List<GameObject> showcaseSpots = new();
    public float waitTimeOnSingleShowcase = 3.0f;
    private Camera _mainCamera;
    private Transform _previousCameraParent;
    private bool _canGoToNextSpot;
    private int _tutorialStep;

    private void GoToNextShowcaseSpot()
    {
        if (_tutorialStep >= showcaseSpots.Count)
        {
            StartActualGame();
        }

        var currentLerpTargetPosition = showcaseSpots[_tutorialStep].transform.position;
        var currentLerpTargetRotation = showcaseSpots[_tutorialStep].transform.rotation;
        _mainCamera.transform.position = currentLerpTargetPosition;
        _mainCamera.transform.rotation = currentLerpTargetRotation;
        _tutorialStep++;

        Invoke(nameof(GoToNextShowcaseSpot), waitTimeOnSingleShowcase);
    }

    void CheckTagSpots()
    {
        foreach (var tagSpot in tagSpots)
        {
            PlayerScore += tagSpot.GetPlayerPercentage() * scoreMultiplier;
            EnemyScore += tagSpot.GetEnemyPercentage() * scoreMultiplier;
        }

        OnScoreUpdated?.Invoke(PlayerScore, EnemyScore);
    }

    private void SpawnEnemy()
    {
        if (CurrentEnemies >= maxEnemies)
        {
            return;
        }

        // Pick a random tag spot
        // TODO: Ignore the tag spots near player location
        // Shuffle the list of tag spots
        var randomTagSpots = tagSpots.OrderBy(_ => Random.value).ToList();

        foreach (var tagSpot in randomTagSpots.Where(tagSpot => tagSpot.CanSpawnEnemy()))
        {
            tagSpot.SpawnEnemy();
            break;
        }
    }

    private Vector3 _startingCameraPosition;
    private Quaternion _startingCameraRotation;

    public void StartMapShowcase()
    {
        Debug.Log("Map showcase started");
        _startingCameraPosition = _mainCamera.transform.position;
        _startingCameraRotation = _mainCamera.transform.rotation;
        _previousCameraParent = _mainCamera.transform.parent;
        _mainCamera.transform.parent = null;
        _mainCamera.GetComponent<TrackedPoseDriver>().enabled = false;

        GoToNextShowcaseSpot();
    }

    public void StartActualGame()
    {
        Debug.Log("Actual game started");
        _mainCamera.transform.parent = _previousCameraParent;
        _mainCamera.transform.position = _startingCameraPosition;
        _mainCamera.transform.rotation = _startingCameraRotation;
        _mainCamera.GetComponent<TrackedPoseDriver>().enabled = true;
        InvokeRepeating(nameof(CheckTagSpots), 0, scoreUpdateInterval);
        InvokeRepeating(nameof(SpawnEnemy), enemySpawnInterval, enemySpawnInterval);
    }

    private void Start()
    {
        _mainCamera = Camera.main;

        Invoke(nameof(StartMapShowcase), 1.0f);
    }
}
