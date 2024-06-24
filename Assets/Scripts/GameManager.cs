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
    public GameObject objectToMove;
    private Camera _mainCamera;
    private Transform _previousParent;
    private bool _canGoToNextSpot;
    private int _tutorialStep;

    private void GoToNextShowcaseSpot()
    {
        if (_tutorialStep >= showcaseSpots.Count)
        {
            StartActualGame();
        }

        objectToMove.transform.position = showcaseSpots[_tutorialStep].transform.position;
        objectToMove.transform.rotation = showcaseSpots[_tutorialStep].transform.rotation;
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

    private Vector3 _startingPosition;
    private Quaternion _startingRotation;

    public void StartMapShowcase()
    {
        Debug.Log("Map showcase started");
        leftController.SetActive(false);
        rightController.SetActive(false);
        locomotionSystem.SetActive(false);
        _startingPosition = objectToMove.transform.position;
        _startingRotation = objectToMove.transform.rotation;
        _previousParent = objectToMove.transform.parent;
        objectToMove.transform.parent = null;
        Camera.main.GetComponent<TrackedPoseDriver>().trackingType = TrackedPoseDriver.TrackingType.RotationOnly;

        GoToNextShowcaseSpot();
    }

    public GameObject leftController;
    public GameObject rightController;
    public GameObject locomotionSystem;

    private void StartActualGame()
    {
        Debug.Log("Actual game started");
        leftController.SetActive(true);
        rightController.SetActive(true);
        locomotionSystem.SetActive(true);
        // objectToMove.transform.parent = _previousParent;
        // objectToMove.transform.position = _startingPosition;
        // objectToMove.transform.rotation = _startingRotation;
        Camera.main.GetComponent<TrackedPoseDriver>().trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
        InvokeRepeating(nameof(CheckTagSpots), 0, scoreUpdateInterval);
        InvokeRepeating(nameof(SpawnEnemy), enemySpawnInterval, enemySpawnInterval);
    }

    private void Start()
    {
        StartActualGame();
        // Invoke(nameof(StartMapShowcase), 1.0f);
    }
}
