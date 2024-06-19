using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRAlyxGrabInteractable : XRGrabInteractable
{
    public float velocityThreshold = 2.0f;
    public float jumpAngleInDegrees = 60.0f;
    public float snapDistance = 0.5f;

    private GameObject _rayInteractorGameObject;
    private XRRayInteractor _rayInteractor;
    private bool _rightHandRayInteracting;
    private Vector3 _previousPosition;
    private Rigidbody _interactableRigidbody;
    private bool _canJump = true;
    private XRDirectInteractor _leftHand;
    private XRDirectInteractor _rightHand;
    private XRInteractionManager _manager;
    private bool _wasSnapped;

    // dług technologiczny
    public SprayCan sprayCan;

    protected override void Awake()
    {
        base.Awake();
        _interactableRigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        _leftHand = PlayerManager.Instance.leftHand;
        _rightHand = PlayerManager.Instance.rightHand;
        _manager = PlayerManager.Instance.interactionManager;
    }

    private void Update()
    {
        if (!isSelected)
        {
            return;
        }

        SnapToHandIfClose(hand: _rightHandRayInteracting ? _rightHand : _leftHand,
                          snapDistance: snapDistance);

        if (firstInteractorSelecting is not XRRayInteractor || !_canJump)
        {
            return;
        }

        var rayInteractorPosition = _rayInteractor.transform.position;
        Vector3 velocity = (rayInteractorPosition - _previousPosition) / Time.deltaTime;
        _previousPosition = rayInteractorPosition;

        if (!(velocity.magnitude > velocityThreshold))
        {
            return;
        }

        Drop();
        _interactableRigidbody.velocity = ComputeVelocity();
        _canJump = false;
        if (sprayCan != null)
        {
            sprayCan.PlayFlySound();
        }
    }

    public void ResetWasSnapped() => _wasSnapped = false;

    private void SnapToHandIfClose(XRDirectInteractor hand, float snapDistance)
    {
        var distanceToRight = Vector3.Distance(hand.transform.position, transform.position);

        if (!(distanceToRight < snapDistance) || _wasSnapped)
        {
            return;
        }

        //transform.position = handPosition;
        _interactableRigidbody.velocity = Vector3.zero;

        Detach();
        Drop();

        var exitArgs = new SelectExitEventArgs
        {
            interactableObject = this,
            interactorObject = _rayInteractor,
            manager = _manager
        };

        OnSelectExited(exitArgs);
        _rayInteractorGameObject.SetActive(false);

        var args = new SelectEnterEventArgs
        {
            interactorObject = hand,
            interactableObject = this,
            manager = _manager
        };

        OnSelectEntered(args);
        interactionManager.SelectEnter((IXRSelectInteractor)hand, this);

        _rayInteractorGameObject.SetActive(true);

        _canJump = false;
        _wasSnapped = true;
    }

    private Vector3 ComputeVelocity()
    {
        Vector3 diff = _rayInteractor.transform.position - transform.position;
        Vector3 diffXZ = new Vector3(diff.x, 0, diff.z);
        float diffXZLength = diffXZ.magnitude;
        float diffYLength = diff.y;

        float angleInRadians = jumpAngleInDegrees * Mathf.Deg2Rad;
        float jumpSpeed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(diffXZLength, 2) / (2 * Mathf.Cos(angleInRadians) * Mathf.Cos(angleInRadians) * (diffXZ.magnitude * Mathf.Tan(angleInRadians) - diffYLength)));
        Vector3 jumpVelocityVector = diffXZ.normalized * (Mathf.Cos(angleInRadians) * jumpSpeed) + Vector3.up * (Mathf.Sin(angleInRadians) * jumpSpeed);


        if (!float.IsNaN(jumpVelocityVector.x) &&
            !float.IsNaN(jumpVelocityVector.y) &&
            !float.IsNaN(jumpVelocityVector.z))
        {
            return jumpVelocityVector;
        }

        if (float.IsNaN(jumpSpeed))
        {
            return diff.normalized * 10f;
        }

        return diff.normalized * jumpSpeed;

    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRRayInteractor interactor)
        {
            _rightHandRayInteracting = interactor.transform.parent.name == "Right Controller";

            trackPosition = false;
            trackRotation = false;
            throwOnDetach = false;

            _rayInteractor = interactor;
            _rayInteractorGameObject = interactor.gameObject;
            _previousPosition = _rayInteractor.transform.position;
            _canJump = true;
        }
        else
        {
            trackPosition = true;
            trackRotation = true;
            throwOnDetach = true;
        }

        base.OnSelectEntered(args);
    }
}
