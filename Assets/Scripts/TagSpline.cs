using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Splines;

public class TagSpline : MonoBehaviour
{
    public SprayColor tagSplineColor;
    public TagSplineCollider tagSplineColliderPrefab;
    public int splineInstancesCount = 10;

    public TagSpot tagSpot;
    private SplineContainer _splineContainer;
    private SplineExtrude _splineExtrude;

    private List<TagSplineCollider> _splineInstances = new List<TagSplineCollider>();
    private List<int> indicesDone = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        _splineContainer = GetComponent<SplineContainer>();
        _splineExtrude = GetComponent<SplineExtrude>();

        var mesh = GetComponent<MeshRenderer>().sharedMaterial;
        mesh.SetColor("_Color", SprayCan.GetColor(tagSplineColor));

        if (_splineContainer != null)
        {
            // Spawn 10 TagSplineColliders along the spline
            for (int i = 0; i < splineInstancesCount; i++)
            {
                var tagSplineCollider = Instantiate(tagSplineColliderPrefab, transform);
                tagSplineCollider.transform.position = _splineContainer.EvaluatePosition(i / (float)(splineInstancesCount - 1));
                tagSplineCollider.transform.forward = _splineContainer.EvaluateTangent(i / (float)(splineInstancesCount - 1));
                // tagSplineCollider.transform.position += tagSplineCollider.transform.up * 0.05f;
                tagSplineCollider.colliderQuality = ColliderQuality.Perfect;
                tagSplineCollider.index = i;
                tagSplineCollider.TagSpline = this;
                tagSplineCollider.name = $"TagSplineCollider_{i}";
                _splineInstances.Add(tagSplineCollider);
            }

            var offset = 0.06f;
            var goodWidth = 0.07f;

            // Spawn 10 TagSplineColliders along the spline
            for (int i = 0; i < splineInstancesCount; i++)
            {
                var tagSplineCollider = Instantiate(tagSplineColliderPrefab, transform);
                tagSplineCollider.transform.position = _splineContainer.EvaluatePosition(i / (float)(splineInstancesCount - 1));
                tagSplineCollider.transform.forward = _splineContainer.EvaluateTangent(i / (float)(splineInstancesCount - 1));
                tagSplineCollider.transform.position += tagSplineCollider.transform.up * offset;
                tagSplineCollider.GetComponent<BoxCollider>().size = new Vector3(0.1f, goodWidth, 0.1f);
                tagSplineCollider.colliderQuality = ColliderQuality.Good;
                tagSplineCollider.index = i;
                tagSplineCollider.TagSpline = this;
                tagSplineCollider.name = $"TagSplineCollider_{i}";
                _splineInstances.Add(tagSplineCollider);
            }

            // Spawn 10 TagSplineColliders along the spline
            for (int i = 0; i < splineInstancesCount; i++)
            {
                var tagSplineCollider = Instantiate(tagSplineColliderPrefab, transform);
                tagSplineCollider.transform.position = _splineContainer.EvaluatePosition(i / (float)(splineInstancesCount - 1));
                tagSplineCollider.transform.forward = _splineContainer.EvaluateTangent(i / (float)(splineInstancesCount - 1));
                tagSplineCollider.transform.position -= tagSplineCollider.transform.up * offset;
                tagSplineCollider.GetComponent<BoxCollider>().size = new Vector3(0.1f, goodWidth, 0.1f);
                tagSplineCollider.colliderQuality = ColliderQuality.Good;
                tagSplineCollider.index = i;
                tagSplineCollider.TagSpline = this;
                tagSplineCollider.name = $"TagSplineCollider_{i}";
                _splineInstances.Add(tagSplineCollider);
            }
        }
    }

    void ResetColliders()
    {
        foreach (var splineInstance in _splineInstances)
        {
            splineInstance.gameObject.SetActive(true);
        }
        indicesDone.Clear();
    }

    public bool OnColliderTriggered(int index, ColliderQuality quality, SprayColor color)
    {
        if (color != tagSplineColor)
        {
            Debug.Log($"TagSpline_{name} was hit with the wrong color!");
            return false;
        }

        if (indicesDone.Contains(index))
        {
            return false;
        }

        indicesDone.Add(index);

        // If all colliders have been triggered, the player has finished the spline
        if (indicesDone.Count == splineInstancesCount - 1)
        {
            Debug.Log($"TagSpline_{name} was finished!");

            ResetColliders();
            gameObject.SetActive(false);
            tagSpot.OnTagSplineFinished();
        }

        return true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
