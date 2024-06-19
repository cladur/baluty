using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public XRDirectInteractor leftHand;
    public XRDirectInteractor rightHand;

    public XRInteractionManager interactionManager;

    public LayerMask groundLayerMask;
    public InteractionLayerMask teleportMask;
    public InteractionLayerMask noTeleportMask;

    private GameObject _previousTeleportArea;

    public AudioClip ladderTouchSound;

    public bool lastHandWasLeft;

    public void SetLastHand(bool isLeft)
    {
        lastHandWasLeft = !isLeft;
    }

    public void PlayLadderTouchSound()
    {
        var hand = lastHandWasLeft ? leftHand : rightHand;

        if (hand.TryGetComponent(typeof(AudioSource), out var audioSource))
        {
            ((AudioSource)audioSource).PlayOneShot(ladderTouchSound);
        }
    }

    private void Update()
    {
        CheckFloorInteractionLayerAndSetItOnTeleport();
    }

    private void CheckFloorInteractionLayerAndSetItOnTeleport()
    {
        if (!Physics.Raycast(transform.position + transform.up * 0.5f, -transform.up, out RaycastHit hit, 1000, groundLayerMask))
        {
            return;
        }

        if (!hit.transform.gameObject.TryGetComponent(typeof(TeleportationArea), out var teleportationArea))
        {
            return;
        }

        if (teleportationArea.transform.gameObject == _previousTeleportArea)
        {
            return;
        }

        if (_previousTeleportArea is not null)
        {
            _previousTeleportArea.GetComponent<TeleportationArea>().interactionLayers = noTeleportMask;
        }

        _previousTeleportArea = teleportationArea.transform.gameObject;
        ((TeleportationArea)teleportationArea).interactionLayers = teleportMask;
    }

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

    public static bool IsGrabbed(string itemName)
    {
        return Instance.rightHand.interactablesSelected.Select(x => x.transform.name).Contains(itemName) ||
               Instance.leftHand.interactablesSelected.Select(x => x.transform.name).Contains(itemName);
    }
}
