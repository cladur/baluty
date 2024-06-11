using System;
using UnityEngine;

[ExecuteInEditMode]
public class Graffiti : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private Material _material;

    public Texture2D texture;

    [Range(0, 1)]
    public float fillPercent = 0.5f;

    private static readonly int FillPercentPropertyName = Shader.PropertyToID("_FillPercent");
    private static readonly int TexturePropertyName = Shader.PropertyToID("_Texture");

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _material = _meshRenderer.material;
    }

    private void OnValidate()
    {
        _material.SetTexture(TexturePropertyName, texture);
        _material.SetFloat(FillPercentPropertyName, fillPercent);
    }
}
