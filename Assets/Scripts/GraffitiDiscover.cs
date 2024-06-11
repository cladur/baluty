using UnityEngine;

public class GraffitiDiscover : MonoBehaviour
{
    public GameObject paintPointsParentGameObject;
    public int GraffitiPointsCount { get; set; }

    private int _stepsDone = 1;

    public int GraffitiPointsDiscovered
    {
        get => _graffitiPointsDiscovered;
        set
        {
            _graffitiPointsDiscovered = value;
            Debug.Log("Discovering graffiti points: " + _graffitiPointsDiscovered + "/" + GraffitiPointsCount);

            CheckStep();
        }
    }

    private int _nextStep;
    private int _graffitiPointsDiscovered;

    public Graffiti graffiti;

    private void Start()
    {
        GraffitiPointsCount = paintPointsParentGameObject.transform.childCount;
        Debug.Log("Point count: " + GraffitiPointsCount);

        _nextStep = (int) (GraffitiPointsCount / 3.0);
        Debug.Log("Next step: " + _nextStep);
    }

    private void CheckStep()
    {
        if (_graffitiPointsDiscovered < _nextStep)
        {
            return;
        }

        graffiti.SetFillPercent(Mathf.Clamp(0.34f * _stepsDone++, 0f, 1f));
        _nextStep += GraffitiPointsCount / 3;
    }
}