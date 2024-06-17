using TMPro;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    public TextMeshPro playerScoreText;
    public TextMeshPro enemyScoreText;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnScoreUpdated += UpdateScoreboard;
    }

    void UpdateScoreboard(float playerScore, float enemyScore)
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
    }
}
