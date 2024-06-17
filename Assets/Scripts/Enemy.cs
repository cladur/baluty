using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public TagSpot tagSpot;

    void Start()
    {
        tagSpot = GetComponentInParent<TagSpot>();
    }

    internal void OnHit()
    {
        tagSpot.OnEnemyHit();
    }
}
