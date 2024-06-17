using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<TagSpot> tagSpots = new List<TagSpot>();

    // How many points are awarded for full control of a tag spot
    public float ScoreMultiplier = 0.2f;

    // How frequently the scores are updated
    public float ScoreUpdateInterval = 1.0f;

    // How frequently enemies are spawned
    public float EnemySpawnInterval = 5.0f;

    public static float playerScore = 0.0f;
    public static float enemyScore = 0.0f;
    public static float EnemySprayDuration = 10.0f;

    public delegate void ScoreUpdated(float playerScore, float enemyScore);
    public static event ScoreUpdated OnScoreUpdated;

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

    void CheckTagSpots()
    {
        foreach (var tagSpot in tagSpots)
        {
            playerScore += tagSpot.GetPlayerPercentage() * ScoreMultiplier;
            enemyScore += tagSpot.GetEnemyPercentage() * ScoreMultiplier;
        }

        OnScoreUpdated?.Invoke(playerScore, enemyScore);
    }

    void SpawnEnemy()
    {
        // Pick a random tag spot
        // TODO: Ignore the tag spots near player location
        // Shuffle the list of tag spots
        var randomTagSpots = tagSpots.OrderBy(x => UnityEngine.Random.value).ToList();

        foreach (var tagSpot in randomTagSpots)
        {
            if (tagSpot.CanSpawnEnemy())
            {
                tagSpot.SpawnEnemy();
                break;
            }
        }
    }

    private void Start()
    {
        InvokeRepeating(nameof(CheckTagSpots), 0, ScoreUpdateInterval);
        InvokeRepeating(nameof(SpawnEnemy), EnemySpawnInterval, EnemySpawnInterval);
    }
}
