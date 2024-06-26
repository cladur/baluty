using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<TagSpot> tagSpots = new();
    public TextMeshPro timeText;
    public MeshRenderer blackThingy;
    public Transform gameFinishedTransform;
    public GameObject playerGameObject;
    private int _remainingGameSeconds = 60 * 5;

    // How many points are awarded for full control of a tag spot
    public float scoreMultiplier = 0.2f;

    // How frequently the scores are updated
    public float scoreUpdateInterval = 1.0f;

    // How frequently enemies are spawned
    public float enemySpawnInterval = 5.0f;

    public float enemySprayDuration = 10.0f;
    public float timeSinceLastEnemyKill = 0.0f;
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
    public GameObject vignette;
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
            return;
        }
        StartCoroutine(FadeToBlackQuick());
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
            Debug.Log("Max enemies reached, can't spawn more!");
            return;
        }

        // If enemy was killed recently, don't spawn another one
        if (timeSinceLastEnemyKill < 5.0f)
        {
            return;
        }

        Debug.Log("Spawning enemy");

        // Pick a random tag spot
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
        vignette.SetActive(false);
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

    public void StartActualGame()
    {
        Debug.Log("Actual game started");
        PlayerScore = 0;
        EnemyScore = 0;
        leftController.SetActive(true);
        rightController.SetActive(true);
        locomotionSystem.SetActive(true);
        vignette.SetActive(true);
        objectToMove.transform.parent = _previousParent;
        objectToMove.transform.position = _startingPosition;
        objectToMove.transform.rotation = _startingRotation;
        Camera.main.GetComponent<TrackedPoseDriver>().trackingType = TrackedPoseDriver.TrackingType.RotationAndPosition;
        InvokeRepeating(nameof(CheckTagSpots), 0, scoreUpdateInterval);
        InvokeRepeating(nameof(SpawnEnemy), enemySpawnInterval, enemySpawnInterval);

        StartCoroutine(GameTimeCoroutine());
    }

    private IEnumerator GameTimeCoroutine()
    {
        while (_remainingGameSeconds > 0)
        {
            yield return new WaitForSeconds(1);

            _remainingGameSeconds--;

            timeText.text = $"{_remainingGameSeconds / 60:00} : {_remainingGameSeconds % 60:00}";
        }

        timeText.text = "Game finished";
        FinishGame();
    }

    private void FinishGame()
    {
        CancelInvoke(nameof(CheckTagSpots));
        CancelInvoke(nameof(SpawnEnemy));

        foreach (var tagSpot in tagSpots)
        {
            tagSpot.KillEnemy();
        }

        leftController.SetActive(false);
        rightController.SetActive(false);
        locomotionSystem.SetActive(false);
        Debug.Log("Game finished!");

        StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlackQuick()
    {
        for (float i = 0; i <= 1.2; i += Time.deltaTime * 2.0f)
        {
            blackThingy.sharedMaterial.color = new Color(0, 0, 0, i);
            yield return null;
        }

        objectToMove.transform.position = showcaseSpots[_tutorialStep].transform.position;
        objectToMove.transform.rotation = showcaseSpots[_tutorialStep].transform.rotation;
        _tutorialStep++;

        yield return new WaitForSeconds(0.5f);

        Invoke(nameof(GoToNextShowcaseSpot), waitTimeOnSingleShowcase);

        StartCoroutine(FadeFromBlack());
    }

    private IEnumerator FadeToBlack()
    {
        for (float i = 0; i <= 1.2; i += Time.deltaTime)
        {
            blackThingy.sharedMaterial.color = new Color(0, 0, 0, i);
            yield return null;
        }

        string whoWon = PlayerScore > EnemyScore ? "Player" : "Enemies";
        timeText.text = $"{whoWon} won!";
        timeText.fontSize = 3f;

        yield return new WaitForSeconds(1.0f);

        playerGameObject.transform.position = gameFinishedTransform.position;
        playerGameObject.transform.rotation = gameFinishedTransform.rotation;

        StartCoroutine(FadeFromBlack());
    }

    private IEnumerator FadeFromBlack()
    {
        for (float i = 1; i > 0; i -= Time.deltaTime * 2.0f)
        {
            blackThingy.sharedMaterial.color = new Color(0, 0, 0, i);
            yield return null;
        }
    }

    private void Start()
    {
        // StartActualGame();
        blackThingy.sharedMaterial.color = new Color(0, 0, 0, 0);
        // Invoke(nameof(StartMapShowcase), 3.0f);
        // Invoke(nameof(StartMapShowcase), 1.0f);
    }

    private void Update()
    {
        timeSinceLastEnemyKill += Time.deltaTime;
    }
}
