using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("adding player score");
            PlayerScore += 0.2f;
        }
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

    private void Start()
    {
        InvokeRepeating(nameof(CheckTagSpots), 0, scoreUpdateInterval);
        InvokeRepeating(nameof(SpawnEnemy), enemySpawnInterval, enemySpawnInterval);
    }
}
