using System;
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

    public AudioSource fireAudioSource;
    public AudioSource shakeAudioSource;
    public AudioSource flyAudioSource;
    public AudioSource singleShotAudioSource;

    public AudioClip enemyHitSound;
    public AudioClip grabSound;

    public float maxSprayRadius = 0.1f;
    public float maxSprayDistance = 1.0f;
    public float velocityToShakeThreshold = 1.0f;
    public float shakeRegenMultiplier = 1.0f;
    public float resetWasInHandTime = 3.0f;

    private Rigidbody _rb;

    private float _paintTimeLeft = 5.0f;
    private float _maxPaintTime = 15.0f;
    private bool _isSpraying;
    private Transform _lerpTarget;
    private bool _wasInHand;
    private bool _wasGrabbed;

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

        UpdateFlySound();

        if (PlayerManager.IsGrabbed(name))
        {
            if (_rb.velocity.magnitude > 0.25f)
            {
                _wasInHand = true;
            }
        }

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
            _paintTimeLeft < _maxPaintTime && PlayerManager.IsGrabbed(name))
        {
            _paintTimeLeft += Time.deltaTime * shakeRegenMultiplier;
            _paintTimeLeft = Mathf.Clamp(_paintTimeLeft, 0, _maxPaintTime);
        }

        // shakeAudioSource.volume = Mathf.Clamp01((_rb.velocity.magnitude - 1.0f) / 4);
        // shakeAudioSource.pitch = 0.8f + 0.4f * Mathf.Clamp01((_rb.velocity.magnitude - 1.0f) / 4);

        UpdateSprayCanColor(currentColor);

        if (_lerpTarget is null || !_wasInHand)
        {
            return;
        }

        transform.position = Vector3.Lerp(transform.position, _lerpTarget.position, Time.deltaTime * 5);
    }

    public void StartSpray()
    {
        if (_paintTimeLeft <= 0.01f)
        {
            return;
        }
        _isSpraying = true;
        sprayConeMesh.gameObject.SetActive(true);
        fireAudioSource.Play();
    }

    public void StopSpray()
    {
        _isSpraying = false;
        sprayConeMesh.gameObject.SetActive(false);
        fireAudioSource.Stop();
    }

    public void PlayGrabSound()
    {
        if (_wasGrabbed || !PlayerManager.IsGrabbed(name))
        {
            return;
        }

        _wasGrabbed = true;

        singleShotAudioSource.PlayOneShot(grabSound);
    }

    public void ResetWasInHand()
    {
        _wasGrabbed = false;
        _lerpTarget = null;

        Invoke(nameof(SetWasInHandToFalse), 2.0f);
    }

    private void SetWasInHandToFalse() => _wasInHand = false;

    private void UpdateFlySound()
    {
        if (PlayerManager.IsGrabbed(name))
        {
            if (flyAudioSource.isPlaying)
            {
                flyAudioSource.Stop();
            }
        }
        else
        {
            switch (_rb.velocity.magnitude)
            {
                case > 0.3f when !flyAudioSource.isPlaying:
                    flyAudioSource.Play();
                    break;
                case < 0.3f when flyAudioSource.isPlaying:
                    flyAudioSource.Stop();
                    break;
            }
        }
    }

    private void UpdateSprayCanColor(Color currentColor)
    {
        float fillPercent = Mathf.Clamp(_paintTimeLeft / _maxPaintTime, 0, 1);
        sprayCanMesh.material.SetFloat(CanColorFillPercentPropertyName, fillPercent);
        sprayCanMesh.material.SetColor(CanFillColorPropertyName, currentColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_lerpTarget is null && other.gameObject.CompareTag("EnemyLerp") && _wasInHand)
        {
            _rb.velocity = Vector3.zero;
            var enemy = other.transform.parent;
            _lerpTarget = enemy;
        }

        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Enemy hit!");
            var enemy = other.gameObject.GetComponent<Enemy>();
            enemy.OnHit();

            _lerpTarget = null;
            _wasInHand = false;
            singleShotAudioSource.PlayOneShot(enemyHitSound);
        }
    }
}
