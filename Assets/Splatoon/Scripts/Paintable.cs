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

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.A))
        {
            return;
        }

        RenderTexture rt = _maskRenderTexture;
        SaveRenderTexture(rt, gameObject.name + ".png");
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

    private static void SaveRenderTexture(RenderTexture rt, string imageName = "Image.png")
    {
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        RenderTexture.active = null;

        byte[] bytes;
        bytes = tex.EncodeToPNG();

        string path = $"{GraffitiFolderPath}/{imageName}";
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);
        AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        Debug.Log("Saved to " + path);
    }
}