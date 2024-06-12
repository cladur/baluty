using UnityEngine;

public class GraffitiDiscover : MonoBehaviour
{
    public GameObject paintPointsParentGameObject;
    public GameObject paintPointsParentGameObject2;
    public GameObject paintPointsParentGameObject3;
    public int graffitiPointsCount;
    public int graffitiPointsCount2;
    public int graffitiPointsCount3;

    private bool firstDone = false;
    private bool secondDone = false;
    private bool thirdDone = false;

    public int GraffitiPointsDiscovered
    {
        get => _graffitiPointsDiscovered;
        set
        {
            _graffitiPointsDiscovered = value;
            Debug.Log("Discovering graffiti points: " + _graffitiPointsDiscovered + "/" + graffitiPointsCount);

            CheckStep();
        }
    }

    private int _graffitiPointsDiscovered;
    private float _fillThreshold = 0.8f;

    public Graffiti graffiti;

    public GameObject firstMark;
    public GameObject secondMark;
    public GameObject thirdMark;

    private void Start()
    {
        graffitiPointsCount = paintPointsParentGameObject.transform.childCount;
        graffitiPointsCount2 = paintPointsParentGameObject2.transform.childCount;
        graffitiPointsCount3 = paintPointsParentGameObject3.transform.childCount;
        Debug.Log("Point count: " + graffitiPointsCount);

        graffiti.SetFillPercent(0);

        paintPointsParentGameObject.SetActive(true);
        paintPointsParentGameObject2.SetActive(false);
        paintPointsParentGameObject3.SetActive(false);

        firstMark.SetActive(true);
        secondMark.SetActive(false);
        thirdMark.SetActive(false);
    }

    private void CheckStep()
    {
        if (GraffitiPointsDiscovered >= _fillThreshold * graffitiPointsCount && !firstDone)
        {
            firstDone = true;
            paintPointsParentGameObject.SetActive(false);
            paintPointsParentGameObject2.SetActive(true);
            firstMark.SetActive(false);
            secondMark.SetActive(true);
            graffiti.SetFillPercent(0.33f);
            GraffitiPointsDiscovered = 0;
        }

        if (GraffitiPointsDiscovered >= _fillThreshold * graffitiPointsCount2 && !secondDone)
        {
            secondDone = true;
            paintPointsParentGameObject2.SetActive(false);
            paintPointsParentGameObject3.SetActive(true);
            secondMark.SetActive(false);
            thirdMark.SetActive(true);
            graffiti.SetFillPercent(0.66f);
            GraffitiPointsDiscovered = 0;
        }

        if (GraffitiPointsDiscovered >= _fillThreshold * graffitiPointsCount3 && !thirdDone)
        {
            thirdDone = true;
            paintPointsParentGameObject3.SetActive(false);
            thirdMark.SetActive(false);
            graffiti.SetFillPercent(1.0f);
        }
    }
}