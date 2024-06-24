using UnityEngine;

public class Tajrol : MonoBehaviour
{
    public GameObject tajrolHold;

    private Vector3 _firstPosition;
    private Vector3 _secondPosition;
    private Vector3 _targetPosition;

    public bool IsFirstPosition { get; set; }
    private bool _isTajroling;
    private Transform _previousPlayerParent;

    public float speed = 1.0f;

    private void Start()
    {
        _firstPosition = transform.Find("TajrolBody").transform.Find("FirstPosition").position;
        _secondPosition = transform.Find("TajrolBody").transform.Find("SecondPosition").position;

        tajrolHold.transform.position = _firstPosition;
        IsFirstPosition = true;
    }

    private void Update()
    {
        if (_targetPosition == Vector3.zero)
        {
            return;
        }

        tajrolHold.transform.position = Vector3.Lerp(tajrolHold.transform.position, _targetPosition, speed * Time.deltaTime);

        if (!(Vector3.Distance(tajrolHold.transform.position, _targetPosition) < 0.01f))
        {
            return;
        }

        tajrolHold.transform.position = _targetPosition;
        IsFirstPosition = !IsFirstPosition;
        _targetPosition = Vector3.zero;
        _isTajroling = false;
    }

    public void MoveTajrolToOtherSide()
    {
        _targetPosition = IsFirstPosition ? _secondPosition : _firstPosition;
    }

    public void StartTajrol()
    {
        if (_isTajroling)
        {
            return;
        }

        _previousPlayerParent = PlayerManager.Instance.gameObject.transform.parent;
        PlayerManager.Instance.gameObject.transform.SetParent(tajrolHold.transform);

        _isTajroling = true;
        _targetPosition = IsFirstPosition ? _secondPosition : _firstPosition;
    }

    public void ResetIsTajroling()
    {
        if (!_isTajroling)
        {
            return;
        }

        PlayerManager.Instance.gameObject.transform.SetParent(_previousPlayerParent);
        _isTajroling = false;
    }
}
