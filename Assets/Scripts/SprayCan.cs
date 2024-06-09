using TMPro;
using UnityEngine;

public class SprayCan : MonoBehaviour
{
    public GameObject sprayPoint;
    public Painter painter;
    public MeshRenderer sprayConeMesh;
    public TextMeshPro debugText;

    public float maxSprayRadius = 0.1f;
    public float maxSprayDistance = 1.0f;
    public float VelocityToShakeThreshold = 1.0f;
    public float ShakeRegenMultiplier = 1.0f;

    Rigidbody rb;

    float paintTimeLeft = 5000.0f;
    bool isSpraying = false;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        sprayConeMesh.gameObject.SetActive(false);
    }

    void Update()
    {
        float hue = 0.05f * Time.timeSinceLevelLoad % 1.0f;
        Color color = Color.HSVToRGB(hue, 1.0f, 1.0f);

        if (isSpraying)
        {
            paintTimeLeft -= Time.deltaTime;
            painter.Paint(sprayPoint.transform, maxSprayDistance, maxSprayRadius, color);
            sprayConeMesh.material.SetColor("_Color", color);
            if (paintTimeLeft <= 0)
            {
                StopSpray();
            }
        }

        debugText.text = "Paint Time Left: " + paintTimeLeft.ToString("F2") + "\n" + "Velocity: " + rb.velocity.magnitude.ToString("F2");

        // If spray can is shaken, add to paint time
        if (rb.velocity.magnitude > VelocityToShakeThreshold && !isSpraying)
        {
            paintTimeLeft += Time.deltaTime * ShakeRegenMultiplier;
        }
    }

    public void StartSpray()
    {
        if (paintTimeLeft <= 0.01f)
        {
            return;
        }
        isSpraying = true;
        sprayConeMesh.gameObject.SetActive(true);
    }

    public void StopSpray()
    {
        isSpraying = false;
        sprayConeMesh.gameObject.SetActive(false);
    }
}
