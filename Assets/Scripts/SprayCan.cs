using TMPro;
using UnityEngine;

public class SprayCan : MonoBehaviour
{
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

    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
        sprayConeMesh.gameObject.SetActive(false);
        sprayCanMesh.material.SetColor(CanColorPropertyName, canColor);
        sprayCanMesh.material.SetColor(CanFillColorPropertyName, GetFillColor());
    }

    void Update()
    {
        var currentColor = GetFillColor();

        if (_isSpraying)
        {
            _paintTimeLeft -= Time.deltaTime;
            painter.Paint(sprayPoint.transform, maxSprayDistance, maxSprayRadius, currentColor);
            sprayConeMesh.material.SetColor(ColorPropertyName, currentColor);

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

    private static Color GetFillColor()
    {
        float hue = 0.05f * Time.timeSinceLevelLoad % 1.0f;
        Color color = Color.HSVToRGB(hue, 1.0f, 1.0f);
        return color;
    }

    private void UpdateSprayCanColor(Color currentColor)
    {
        float fillPercent = Mathf.Clamp(_paintTimeLeft / _maxPaintTime, 0, 1);
        sprayCanMesh.material.SetFloat(CanColorFillPercentPropertyName, fillPercent);
        sprayCanMesh.material.SetColor(CanFillColorPropertyName, currentColor);
    }
}
