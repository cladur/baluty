using TMPro;
using UnityEngine;

public enum SprayColor
{
    Red,
    Green,
    Blue
}

public class SprayCan : MonoBehaviour
{
    public SprayColor sprayColor;
    public GameObject sprayPoint;
    public Painter painter;
    public MeshRenderer sprayConeMesh;
    public MeshRenderer sprayCanMesh;
    public TextMeshPro debugText;
    public Color canColor;

    public float maxSprayRadius = 0.1f;
    public float maxSprayDistance = 1.0f;
    public float velocityToShakeThreshold = 1.0f;
    public float shakeRegenMultiplier = 1.0f;

    private Rigidbody _rb;

    private float _paintTimeLeft = 5.0f;
    private float _maxPaintTime = 15.0f;
    private bool _isSpraying;

    private static readonly int ColorPropertyName = Shader.PropertyToID("_Color");
    private static readonly int CanColorFillPercentPropertyName = Shader.PropertyToID("_FillHeight");
    private static readonly int CanFillColorPropertyName = Shader.PropertyToID("_CanFillColor");
    private static readonly int CanColorPropertyName = Shader.PropertyToID("_CanColor");

    public static Color GetColor(SprayColor sprayColor)
    {
        switch (sprayColor)
        {
            case SprayColor.Red:
                return Color.red;
            case SprayColor.Green:
                return Color.green;
            case SprayColor.Blue:
                return Color.blue;
            default:
                return Color.white;
        }
    }

    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        sprayConeMesh.gameObject.SetActive(false);
        sprayCanMesh.material.SetColor(CanColorPropertyName, canColor);
        sprayCanMesh.material.SetColor(CanFillColorPropertyName, GetColor(sprayColor));
        sprayConeMesh.material.SetColor(ColorPropertyName, GetColor(sprayColor));
    }

    void ShootRaycastForTagSplineColliders()
    {
        RaycastHit hit;
        int layerMask = LayerMask.GetMask("TagSplineCollider");
        if (Physics.Raycast(sprayPoint.transform.position, sprayPoint.transform.forward, out hit, maxSprayDistance, layerMask))
        {
            var tagSplineCollider = hit.collider.GetComponent<TagSplineCollider>();
            if (tagSplineCollider != null)
            {
                tagSplineCollider.OnShotWithPaint(sprayColor);
            }
        }
    }

    void Update()
    {
        var currentColor = GetColor(sprayColor);

        if (_isSpraying)
        {
            _paintTimeLeft -= Time.deltaTime;
            painter.Paint(sprayPoint.transform, maxSprayDistance, maxSprayRadius, currentColor);
            ShootRaycastForTagSplineColliders();

            if (_paintTimeLeft <= 0)
            {
                StopSpray();
            }
        }

        debugText.text = "Paint Time Left: " + _paintTimeLeft.ToString("F2") + "\n" + "Velocity: " + _rb.velocity.magnitude.ToString("F2");

        // If spray can is shaken, add to paint time
        if (_rb.velocity.magnitude > velocityToShakeThreshold &&
            !_isSpraying &&
            _paintTimeLeft < _maxPaintTime)
        {
            _paintTimeLeft += Time.deltaTime * shakeRegenMultiplier;
            _paintTimeLeft = Mathf.Clamp(_paintTimeLeft, 0, _maxPaintTime);
        }

        UpdateSprayCanColor(currentColor);
    }

    public void StartSpray()
    {
        if (_paintTimeLeft <= 0.01f)
        {
            return;
        }
        _isSpraying = true;
        sprayConeMesh.gameObject.SetActive(true);
    }

    public void StopSpray()
    {
        _isSpraying = false;
        sprayConeMesh.gameObject.SetActive(false);
    }

    private void UpdateSprayCanColor(Color currentColor)
    {
        float fillPercent = Mathf.Clamp(_paintTimeLeft / _maxPaintTime, 0, 1);
        sprayCanMesh.material.SetFloat(CanColorFillPercentPropertyName, fillPercent);
        sprayCanMesh.material.SetColor(CanFillColorPropertyName, currentColor);
    }
}
