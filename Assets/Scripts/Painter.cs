using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour
{
    public void Paint(Transform sprayPoint, float maxSprayDistance, float maxRadius, Color color)
    {
        Vector3 position = sprayPoint.position;
        Ray ray = new Ray(position, sprayPoint.forward);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * maxSprayDistance, Color.red);

        if (Physics.Raycast(ray, out hit, maxSprayDistance))
        {
            transform.position = hit.point;
            Paintable p = hit.collider.GetComponent<Paintable>();
            if (p != null)
            {
                // Scale radius based on distance
                float distance = Vector3.Distance(sprayPoint.position, hit.point);
                float farnessFromWall = distance / maxSprayDistance;
                float radius = maxRadius * farnessFromWall;
                float strength = 1.0f - (farnessFromWall + 0.1f);
                float hardness = 1.0f - farnessFromWall;
                PaintManager.instance.paint(p, hit.point, radius, 0.0001f, 1.0f, color);
            }
        }
    }
}
