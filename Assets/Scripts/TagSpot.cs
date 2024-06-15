using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Video;

public class TagSpot : MonoBehaviour
{
    public enum TagSpotSize
    {
        Small,
        Medium,
        Large
    }

    public enum TagSpotOccupant
    {
        None,
        Player,
        Enemy
    }

    [Header("Internal")]
    public DecalProjector decal;


    [Header("Tag Spot Settings")]
    public TagSpotSize size;
    public TagSpotOccupant occupant = TagSpotOccupant.None;

    public List<TagSpline> tagSplines = new List<TagSpline>();

    int _splinesOccupiedByPlayer = 0;
    int _splinesOccupiedByEnemy = 0;

    private Painter _painter;

    // Start is called before the first frame update
    void Start()
    {
        _painter = GetComponent<Painter>();
        // For child tag splines
        foreach (Transform child in transform)
        {
            var tagSpline = child.GetComponent<TagSpline>();
            if (tagSpline != null)
            {
                tagSplines.Add(tagSpline);
                tagSpline.tagSpot = this;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResetTagSpot()
    {
        _splinesOccupiedByPlayer = 0;
        _splinesOccupiedByEnemy = 0;

        for (int i = 0; i < tagSplines.Count; i++)
        {
            tagSplines[i].gameObject.SetActive(true);
        }
        ClearPainting();
    }

    void ClearPainting()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 1.0f, Color.red);

        if (Physics.Raycast(ray, out hit, 1.0f))
        {
            transform.position = hit.point;
            Paintable p = hit.collider.GetComponent<Paintable>();
            if (p != null)
            {
                // Scale radius based on distance
                float radius = 10.0f;
                Color color = new Color(0, 0, 0, 0);
                PaintManager.instance.paint(p, hit.point, radius, 0.5f, 0.5f, color);
            }
        }
    }

    public void OnTagSplineFinished()
    {
        _splinesOccupiedByPlayer += 1;
        _splinesOccupiedByEnemy = tagSplines.Count - _splinesOccupiedByPlayer;

        Debug.Log($"TagSpot_{name} has {_splinesOccupiedByPlayer} splines occupied by the player and {_splinesOccupiedByEnemy} splines occupied by the enemy.");

        // Wait for 3 seconds before resetting the tag spot
        Invoke("ResetTagSpot", 3f);
    }
}
