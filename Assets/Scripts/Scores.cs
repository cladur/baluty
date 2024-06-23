using System.Collections.Generic;
using UnityEngine;

public class Scores : MonoBehaviour
{
    public Material grayMaterial;
    public Material redMaterial;
    public Material blueMaterial;

    public List<MeshRenderer> meshRenderers;

    private void Start()
    {
        SetScore(0, 0);
    }

    public void SetScore(int playerScores, int enemyScores)
    {
        int i = 0;
        int maxScores = meshRenderers.Count;
        int grayScores = maxScores - (playerScores + enemyScores);

        for (; i < grayScores; i++)
        {
            meshRenderers[i].material = grayMaterial;
        }

        for (; i < playerScores + grayScores; i++)
        {
            meshRenderers[i].material = blueMaterial;
        }

        for (; i < maxScores; i++)
        {
            meshRenderers[i].material = redMaterial;
        }
    }
}
