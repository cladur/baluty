using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class Graffiti : MonoBehaviour
{
    private Material _material;
    public DecalProjector decalProjector;
    public Texture2D texture;

    [Range(0, 1)]
    public float fillPercent = 0.5f;

    private static readonly int FillPercentPropertyName = Shader.PropertyToID("_FillPercent");
    private static readonly int TexturePropertyName = Shader.PropertyToID("_Texture");

    private void OnValidate()
    {
        UpdateShader();
    }

    private void UpdateShader()
    {
        if (_material is null)
        {
            _material = decalProjector.material;
            _material = new Material(_material);
            decalProjector.material = _material;

            if (_material is null)
            {
                return;
            }
        }


        if (texture is not null)
        {
            try
            {
                _material.SetTexture(TexturePropertyName, texture);
            }
            catch (Exception ex)
            {
                // Ignore ;)
            }
        }

        try
        {
            _material.SetFloat(FillPercentPropertyName, fillPercent);
        }
        catch (Exception ex)
        {
            // Ignore ;)
        }
    }

    public void SetFillPercent(float fillValue)
    {
        fillPercent = Mathf.Clamp(fillValue, 0f, 1f);
        UpdateShader();
    }
}
