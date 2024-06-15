using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public XRDirectInteractor leftHand;
    public XRDirectInteractor rightHand;
    public XRInteractionManager interactionManager;

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
}
