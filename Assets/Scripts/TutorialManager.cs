using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public TextMeshPro textAttachedToHead;

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
        textAttachedToHead.text = "Welcome to Graffiti Master!";
        Invoke(nameof(ShowTutorialMovement), 4.0f);
    }

    void ShowTutorialMovement()
    {
        textAttachedToHead.text = "Use the left stick to move around.";
        Invoke(nameof(ShowTutorialTeleport), 5.0f);
    }

    void ShowTutorialTeleport()
    {
        textAttachedToHead.text = "Move the right stick forward to teleport.";
        Invoke(nameof(ShowTutorialRotateView), 6.0f);
    }

    void ShowTutorialRotateView()
    {
        textAttachedToHead.text = "Flick right stick to the sides to rotate your view.";
        Invoke(nameof(ShowTutorialGrabbing), 6.0f);
    }

    void ShowTutorialGrabbing()
    {
        textAttachedToHead.text = "Aim at the spray can, hold the grip button and pull the hand towards you to grab it.";
        // TODO: If grabbed the can, show next tutorial
        Invoke(nameof(ShowTutorialEnemy), 8.0f);
    }

    void ShowTutorialEnemy()
    {
        textAttachedToHead.text = "Throw the spray can at the enemy to 'neutralize' him.";
        // TODO: If hit enemy, show next tutorial
        Invoke(nameof(ShowTutorialTagSpot), 10.0f);
    }

    void ShowTutorialTagSpot()
    {
        textAttachedToHead.text = "Good! Now walk up to the tag spot and spray it to claim it!";
        // TODO: If tagged the spot, show next tutorial
        Invoke(nameof(ShowTutorialClimb), 10.0f);
    }

    void ShowTutorialClimb()
    {
        textAttachedToHead.text = "Well done! Now climb up the ladder to start the game!";
        // TODO: If climbed the ladder, show next tutorial
        Invoke(nameof(FinishTutorial), 6.0f);
    }

    void FinishTutorial()
    {
        textAttachedToHead.text = "";
    }

    void Start()
    {
        if (textAttachedToHead == null)
        {
            Debug.LogError("Please attach a TextMeshPro object to the TutorialManager script in the inspector");
            return;
        }
        ShowTutorialWelcome();
    }
}
