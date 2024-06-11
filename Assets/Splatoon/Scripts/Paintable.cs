using UnityEngine;

public class Paintable : MonoBehaviour {
    const int TEXTURE_SIZE = 1024;

    public float extendsIslandOffset = 1;

    public RenderTexture extendIslandsRenderTexture;
    public RenderTexture uvIslandsRenderTexture;
    public RenderTexture maskRenderTexture;
    public RenderTexture supportTexture;
    
    public Renderer rend;

    int maskTextureID = Shader.PropertyToID("_MaskTexture");

    public RenderTexture getMask() => maskRenderTexture;
    public RenderTexture getUVIslands() => uvIslandsRenderTexture;
    public RenderTexture getExtend() => extendIslandsRenderTexture;
    public RenderTexture getSupport() => supportTexture;
    public Renderer getRenderer() => rend;

    void Start() {
        maskRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        maskRenderTexture.filterMode = FilterMode.Bilinear;

        extendIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        extendIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        uvIslandsRenderTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        uvIslandsRenderTexture.filterMode = FilterMode.Bilinear;

        supportTexture = new RenderTexture(TEXTURE_SIZE, TEXTURE_SIZE, 0);
        supportTexture.filterMode =  FilterMode.Bilinear;

        rend = GetComponent<Renderer>();
        rend.material.SetTexture(maskTextureID, extendIslandsRenderTexture);

        Initialize();
    }

    public void Initialize()
    {
        PaintManager.instance.initTextures(this);
    }

    void OnDisable(){
        maskRenderTexture.Release();
        uvIslandsRenderTexture.Release();
        extendIslandsRenderTexture.Release();
        supportTexture.Release();
    }
}