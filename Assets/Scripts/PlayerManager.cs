using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public XRDirectInteractor leftHand;
    public XRDirectInteractor rightHand;

    public XRRayInteractor leftTeleport;
    public XRRayInteractor rightTeleport;

    public XRInteractionManager interactionManager;

    public LayerMask groundLayerMask;
    public InteractionLayerMask teleportMask;
    public InteractionLayerMask noTeleportMask;

    private GameObject _previousTeleportArea;

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

        if (!hit.transform.parent.gameObject.TryGetComponent(typeof(TeleportationArea), out var teleportationArea))
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
