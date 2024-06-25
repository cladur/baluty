using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public GameObject welcomeText;
    public GameObject movementText;
    public GameObject teleportText;
    public GameObject grabbingText;
    public GameObject enemyText;
    public GameObject tagSpotText;
    public GameObject climbText;

    public GameObject ladder;

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

    void ShowTutorialWelcome()
    {
        welcomeText.SetActive(true);
        Invoke(nameof(ShowTutorialMovement), 4.0f);
    }

    void ShowTutorialMovement()
    {
        welcomeText.SetActive(false);
        movementText.SetActive(true);
        Invoke(nameof(ShowTutorialTeleport), 5.0f);
    }

    void ShowTutorialTeleport()
    {
        movementText.SetActive(false);
        teleportText.SetActive(true);
        Invoke(nameof(ShowTutorialGrabbing), 6.0f);
    }
    void ShowTutorialGrabbing()
    {
        teleportText.SetActive(false);
        grabbingText.SetActive(true);
        // TODO: If grabbed the can, show next tutorial
        Invoke(nameof(ShowTutorialEnemy), 8.0f);
    }

    void ShowTutorialEnemy()
    {
        grabbingText.SetActive(false);
        enemyText.SetActive(true);
        // TODO: If hit enemy, show next tutorial
        Invoke(nameof(ShowTutorialTagSpot), 10.0f);
    }

    void ShowTutorialTagSpot()
    {
        enemyText.SetActive(false);
        tagSpotText.SetActive(true);
        // TODO: If tagged the spot, show next tutorial
        Invoke(nameof(ShowTutorialClimb), 10.0f);
    }

    void ShowTutorialClimb()
    {
        tagSpotText.SetActive(false);
        climbText.SetActive(true);
        ladder.SetActive(true);
        // TODO: If climbed the ladder, show next tutorial
        Invoke(nameof(FinishTutorial), 6.0f);
    }

    void FinishTutorial()
    {
        climbText.SetActive(false);
        // Invoke(nameof(StartGame), 6.0f);
    }

    void DeactivateAll()
    {
        welcomeText.SetActive(false);
        movementText.SetActive(false);
        teleportText.SetActive(false);
        grabbingText.SetActive(false);
        enemyText.SetActive(false);
        tagSpotText.SetActive(false);
        climbText.SetActive(false);
        ladder.SetActive(false);
    }

    public void ShowTagSpots()
    {
        GameManager.Instance.StartMapShowcase();
    }

    void Start()
    {
        DeactivateAll();
        ShowTutorialWelcome();
    }
}
