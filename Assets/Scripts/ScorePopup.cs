using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScorePopup : MonoBehaviour
{
    public Texture2D goodTexture;
    public Texture2D perfectTexture;
    Material _material;

    // Start is called before the first frame update
    void Awake()
    {
        _material = GetComponent<MeshRenderer>().material;
        if (goodTexture != null)
        {
            _material.SetTexture("_Texture", goodTexture);
        }
    }

    public void StartAnimation(ColliderQuality quality)
    {
        if (quality == ColliderQuality.Perfect)
        {
            _material.SetTexture("_Texture", perfectTexture);
        }
        else
        {
            _material.SetTexture("_Texture", goodTexture);
        }
    }

    private void OnValidate()
    {
        if (_material == null)
        {
            _material = GetComponent<MeshRenderer>().material;
        }
        if (goodTexture != null)
        {
            _material.SetTexture("_Texture", goodTexture);
        }
    }
}
