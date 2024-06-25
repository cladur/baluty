using System;
using UnityEditor;
using UnityEngine;

public class Paintable : MonoBehaviour
{

    const int TextureSize = 1024;

    private static readonly string GraffitiFolderPath = "Assets/Materials/Graffiti/";


    public float extendsIslandOffset = 1;
    public GraffitiDiscover graffitiDiscover;


    private RenderTexture _extendIslandsRenderTexture;
    private RenderTexture _uvIslandsRenderTexture;
    private RenderTexture _maskRenderTexture;
    private RenderTexture _supportTexture;

    private Renderer _rend;

    private int _maskTextureID = Shader.PropertyToID("_MaskTexture");

    private bool _wasPaintedAtLeastOnce;

    public RenderTexture GetMask() => _maskRenderTexture;
    public RenderTexture GetUVIslands() => _uvIslandsRenderTexture;
    public RenderTexture GetExtend() => _extendIslandsRenderTexture;
    public RenderTexture GetSupport() => _supportTexture;
    public Renderer GetRenderer() => _rend;

    void Start()
    {
        _maskRenderTexture = new RenderTexture(TextureSize, TextureSize, 0);
        _maskRenderTexture.filterMode = FilterMode.Bilinear;

        _extendIslandsRenderTexture = new RenderTexture(TextureSize, TextureSize, 0);
        _extendIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        _uvIslandsRenderTexture = new RenderTexture(TextureSize, TextureSize, 0);
        _uvIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        _supportTexture = new RenderTexture(TextureSize, TextureSize, 0);
        _supportTexture.filterMode = FilterMode.Bilinear;

        _rend = GetComponent<Renderer>();
        _rend.material.SetTexture(_maskTextureID, _extendIslandsRenderTexture);

        Initialize();
    }

    public void Initialize()
    {
        PaintManager.instance.initTextures(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_wasPaintedAtLeastOnce || graffitiDiscover is null)
        {
            return;
        }

        if (other.CompareTag("Painter"))
        {
            _wasPaintedAtLeastOnce = true;
            graffitiDiscover.GraffitiPointsDiscovered++;
        }
    }

    private void OnDisable()
    {
        _maskRenderTexture.Release();
        _uvIslandsRenderTexture.Release();
        _extendIslandsRenderTexture.Release();
        _supportTexture.Release();
    }
}