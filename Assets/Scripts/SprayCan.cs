using System;
using TMPro;
using UnityEngine;

public enum SprayColor
{
    Violet,
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
    public Color canColor;
    public GameObject vfxHitPrefab;

    public AudioSource fireAudioSource;
    public AudioSource shakeAudioSource;
    public AudioSource flyAudioSource;
    public AudioSource singleShotAudioSource;

    public AudioClip enemyHitSound;
    public AudioClip grabSound;

    public float maxSprayRadius = 0.1f;
    public float maxSprayDistance = 1.0f;
    public float resetWasInHandTime = 3.0f;

    private Rigidbody _rb;
    private XRAlyxGrabInteractable _grabInteractable;

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
            case SprayColor.Violet:
                return Color.magenta;
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
        _grabInteractable = gameObject.GetComponent<XRAlyxGrabInteractable>();
        _grabInteractable.sprayCan = this;
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

        if (PlayerManager.IsGrabbed(name))
        {
            _wasInHand = true;
            _canSetWasInHandToFalse = false;
        }

        if (!_wasGrabbed && PlayerManager.IsGrabbed(name))
        {
            PlayGrabSound();
        }

        if (_isSpraying)
        {
            painter.Paint(sprayPoint.transform, maxSprayDistance, maxSprayRadius, currentColor);
            ShootRaycastForTagSplineColliders();
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

        singleShotAudioSource.Play();
    }

    public void PlayFlySound()
    {
        flyAudioSource.Play();
    }

    private bool _canSetWasInHandToFalse;

    public void ResetWasInHand()
    {
        _wasGrabbed = false;
        _lerpTarget = null;
        _canSetWasInHandToFalse = true;

        Invoke(nameof(SetWasInHandToFalse), 3.0f);
    }

    private void SetWasInHandToFalse()
    {
        if (!_canSetWasInHandToFalse)
        {
            return;
        }

        _wasInHand = false;
    }

    private void UpdateSprayCanColor(Color currentColor)
    {
        sprayCanMesh.material.SetFloat(CanColorFillPercentPropertyName, 1.0f);
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

            var closestPoint = other.ClosestPoint(transform.position);
            var vfx = Instantiate(vfxHitPrefab, closestPoint, Quaternion.identity);
            vfx.GetComponent<Hit>().SetColor(GetColor(sprayColor));
            vfx.GetComponent<Hit>().Play();

            _lerpTarget = null;
            _wasInHand = false;
            singleShotAudioSource.PlayOneShot(enemyHitSound);
        }
    }
}
