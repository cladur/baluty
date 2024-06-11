using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GraffitiManager : MonoBehaviour
{
    public Paintable leftWall;
    public Paintable rightWall;
    private Vector3 distanceDifference;

    public List<PaintPoint> PaintPoints { get; private set; } = new();

    public class PaintPoint
    {
        public Paintable paintable;
        public Vector3 position;
        public float radius;
        public float hardness;
        public float strength;
        public Color color;

        public PaintPoint(Paintable paintable, Vector3 position, float radius, float hardness, float strength, Color color)
        {
            this.paintable = paintable;
            this.position = position;
            this.radius = radius;
            this.hardness = hardness;
            this.strength = strength;
            this.color = color;
        }
    }

    private void Awake()
    {
        distanceDifference = rightWall.transform.position - leftWall.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var currentPaintPoints = PaintPoints.ToList();

            foreach (var point in currentPaintPoints.Where(x => x.paintable == leftWall))
            {
                PaintManager.instance.paint(
                    rightWall,
                    point.position + distanceDifference,
                    point.radius,
                    point.hardness,
                    point.strength,
                    point.color);
            }
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            RenderTexture rt = rightWall.maskRenderTexture;

            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            RenderTexture.active = null;

            byte[] bytes;
            bytes = tex.EncodeToPNG();

            string path = "Image.png";
            System.IO.File.WriteAllBytes(path, bytes);
            Debug.Log("Saved to " + path);
        }
    }
}
