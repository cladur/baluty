using System;
using TMPro;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    public TextMeshPro playerScoreText;
    public TextMeshPro enemyScoreText;
    public TextMeshPro numberOfEnemiesText;
    public MeshRenderer scoreBar;
    private static readonly int FillPercent = Shader.PropertyToID("_FillPercent");

    // Start is called before the first frame update
    private void Start()
    {
        GameManager.OnScoreUpdated += UpdateScoreboard;
    }

    private void UpdateScoreboard(float playerScore, float enemyScore)
    {
        if (playerScoreText == null || enemyScoreText == null)
        {
            Debug.LogWarning("Scoreboard text fields not set in the inspector!");
            return;
        }

        var playerScoreForDisplay = Mathf.Round(playerScore * 10) / 10;
        var enemyScoreForDisplay = Mathf.Round(enemyScore * 10) / 10;
        playerScoreText.text = $"Player\n{playerScoreForDisplay}";
        enemyScoreText.text = $"Enemy\n{enemyScoreForDisplay}";
        numberOfEnemiesText.text = $"Enemies Count \n{GameManager.Instance.CurrentEnemies}";

        if (playerScore == 0f && enemyScore == 0f)
        {
            return;
        }

        float fillPercent = 1.0f - (playerScore / (playerScore + enemyScore));
        Debug.Log("Fill: " + fillPercent);
        scoreBar.material.SetFloat(FillPercent, fillPercent);
    }
}
