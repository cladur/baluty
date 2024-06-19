using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class TagSpline : MonoBehaviour
{
    public SprayColor tagSplineColor;
    // TODO: Swap those two together, player should be blue and enemy should be red
    public Texture2D playerDecalTexture;
    public Texture2D enemyDecalTexture;
    public TagSplineCollider tagSplineColliderPrefab;
    public int splineInstancesCount = 10;

    public TagSpot tagSpot;
    private SplineContainer _splineContainer;
    private SplineExtrude _splineExtrude;
    private DecalProjector _decalProjector;
    private MeshRenderer _mesh;

    public enum TagSplineOccupant
    {
        None,
        Player,
        Enemy
    }

    public TagSplineOccupant occupant = TagSplineOccupant.None;

    private List<TagSplineCollider> _splineColliders = new List<TagSplineCollider>();
    private List<int> indicesDone = new List<int>();

    void OnValidate()
    {
        _splineContainer = GetComponent<SplineContainer>();
        _splineExtrude = GetComponent<SplineExtrude>();

        var mesh = GetComponent<MeshRenderer>();
        mesh.material = new Material(mesh.material);
        mesh.material.SetColor("_Color", SprayCan.GetColor(tagSplineColor));

        var decal = GetComponent<DecalProjector>();
        decal.material = new Material(decal.material);
        decal.material.SetTexture("_Texture", enemyDecalTexture);
    }

    // Start is called before the first frame update
    void Start()
    {
        _splineContainer = GetComponent<SplineContainer>();
        _splineExtrude = GetComponent<SplineExtrude>();

        _mesh = GetComponent<MeshRenderer>();
        _mesh.material = new Material(_mesh.material);
        _mesh.material.SetColor("_Color", SprayCan.GetColor(tagSplineColor));

        _decalProjector = GetComponent<DecalProjector>();
        _decalProjector.material = new Material(_decalProjector.material);
        _decalProjector.material.SetTexture("_Texture", enemyDecalTexture);

        _decalProjector.enabled = false;

        if (Application.isPlaying && _splineContainer != null)
        {
            // Spawn 10 TagSplineColliders along the spline
            for (int i = 0; i < splineInstancesCount; i++)
            {
                var tagSplineCollider = Instantiate(tagSplineColliderPrefab, transform);
                tagSplineCollider.transform.position = _splineContainer.EvaluatePosition(0.05f + (i / (float)(splineInstancesCount - 1)) * 0.9f);
                tagSplineCollider.transform.forward = _splineContainer.EvaluateTangent(0.05f + (i / (float)(splineInstancesCount - 1)) * 0.9f);
                // tagSplineCollider.transform.position += tagSplineCollider.transform.up * 0.05f;
                tagSplineCollider.colliderQuality = ColliderQuality.Perfect;
                tagSplineCollider.index = i;
                tagSplineCollider.TagSpline = this;
                tagSplineCollider.name = $"TagSplineCollider_{i}";
                _splineColliders.Add(tagSplineCollider);
            }

            var offset = 0.06f;
            var goodWidth = 0.07f;

            // Spawn 10 TagSplineColliders along the spline
            for (int i = 0; i < splineInstancesCount; i++)
            {
                var tagSplineCollider = Instantiate(tagSplineColliderPrefab, transform);
                tagSplineCollider.transform.position = _splineContainer.EvaluatePosition(0.05f + (i / (float)(splineInstancesCount - 1)) * 0.9f);
                tagSplineCollider.transform.forward = _splineContainer.EvaluateTangent(0.05f + (i / (float)(splineInstancesCount - 1)) * 0.9f);
                tagSplineCollider.transform.position += tagSplineCollider.transform.up * offset;
                tagSplineCollider.GetComponent<BoxCollider>().size = new Vector3(0.1f, goodWidth, 0.1f);
                tagSplineCollider.colliderQuality = ColliderQuality.Good;
                tagSplineCollider.index = i;
                tagSplineCollider.TagSpline = this;
                tagSplineCollider.name = $"TagSplineCollider_{i}";
                _splineColliders.Add(tagSplineCollider);
            }

            // Spawn 10 TagSplineColliders along the spline
            for (int i = 0; i < splineInstancesCount; i++)
            {
                var tagSplineCollider = Instantiate(tagSplineColliderPrefab, transform);
                tagSplineCollider.transform.position = _splineContainer.EvaluatePosition(0.05f + (i / (float)(splineInstancesCount - 1)) * 0.9f);
                tagSplineCollider.transform.forward = _splineContainer.EvaluateTangent(0.05f + (i / (float)(splineInstancesCount - 1)) * 0.9f);
                tagSplineCollider.transform.position -= tagSplineCollider.transform.up * offset;
                tagSplineCollider.GetComponent<BoxCollider>().size = new Vector3(0.1f, goodWidth, 0.1f);
                tagSplineCollider.colliderQuality = ColliderQuality.Good;
                tagSplineCollider.index = i;
                tagSplineCollider.TagSpline = this;
                tagSplineCollider.name = $"TagSplineCollider_{i}";
                _splineColliders.Add(tagSplineCollider);
            }
        }
    }

    void ResetColliders()
    {
        foreach (var splineInstance in _splineColliders)
        {
            splineInstance.boxCollider.enabled = true;
        }
        indicesDone.Clear();
    }

    public bool OnColliderTriggered(int index, ColliderQuality quality, SprayColor color)
    {
        if (color != tagSplineColor)
        {
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
            OvertakeByPlayer();
        }

        return true;
    }

    public void OvertakeByPlayer()
    {
        occupant = TagSplineOccupant.Player;
        _decalProjector.enabled = true;
        _decalProjector.material.SetTexture("_Texture", enemyDecalTexture);
        UpdateVisibility(false);
        tagSpot.OnTagSplineChanged(true);
    }

    public void OvertakeByEnemy()
    {
        occupant = TagSplineOccupant.Enemy;
        _decalProjector.enabled = true;
        _decalProjector.material.SetTexture("_Texture", playerDecalTexture);
        UpdateVisibility(true);
        tagSpot.OnTagSplineChanged(false);
    }

    public void UpdateVisibility(bool isActive)
    {
        _mesh.enabled = isActive;
        foreach (var splineInstance in _splineColliders)
        {
            splineInstance.gameObject.SetActive(isActive);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
