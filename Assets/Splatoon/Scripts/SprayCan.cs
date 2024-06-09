using TMPro;
using UnityEngine;

public class SprayCan : MonoBehaviour
{
    public GameObject sprayPoint;
    public GameObject sprayObject;
    public TextMeshPro debugText;

    public float VelocityToShakeThreshold = 1.0f;
    public float ShakeRegenMultiplier = 1.0f;

    Rigidbody rb;

    float paintTimeLeft = 5.0f;
    bool isSpraying = false;

    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        sprayObject.SetActive(false);
    }

    void Update()
    {
        if (isSpraying)
        {
            paintTimeLeft -= Time.deltaTime;
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
        if (paintTimeLeft <= 0.01f) {
            return;
        }
        sprayObject.SetActive(true);
        isSpraying = true;
    }

    public void StopSpray()
    {
        sprayObject.SetActive(false);
        isSpraying = false;
    }
}
