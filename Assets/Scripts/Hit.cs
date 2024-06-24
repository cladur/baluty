using UnityEngine;
using UnityEngine.VFX;

public class Hit : MonoBehaviour
{
    public VisualEffect hitVfx;

    public void SetColor(Color color)
    {
        hitVfx.SetVector4("MainColor", color);
    }

    public void Play()
    {
        hitVfx.Play();
    }

    private void Start()
    {
        Invoke(nameof(DestroyMe), 1f);
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }
}
