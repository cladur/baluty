using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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

    [Header("Internal")]
    public GameObject enemy;

    public List<TagSpline> tagSplines = new List<TagSpline>();

    int _splinesOccupiedByPlayer = 0;
    int _splinesOccupiedByEnemy = 0;
    bool _enemyPresent = false;
    float _timeSinceLastEnemySpawn = 0.0f;
    float _timeSincePlayerSprayed = 0.0f;
    TagSpline tagSplineToOverrideByEnemy = null;
    AudioSource _sprayAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        _sprayAudioSource = GetComponent<AudioSource>();

        // Hide the enemy
        enemy.SetActive(false);

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

        GameManager.Instance.tagSpots.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        _timeSinceLastEnemySpawn += Time.deltaTime;
        _timeSincePlayerSprayed += Time.deltaTime;
    }

    public float GetPlayerPercentage()
    {
        return (float)_splinesOccupiedByPlayer / tagSplines.Count;
    }

    public float GetEnemyPercentage()
    {
        return (float)_splinesOccupiedByEnemy / tagSplines.Count;
    }

    public void ResetTagSpot()
    {
        _splinesOccupiedByPlayer = 0;
        _splinesOccupiedByEnemy = 0;

        for (int i = 0; i < tagSplines.Count; i++)
        {
            tagSplines[i].UpdateVisibility(true);

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

    public void OnTagSplineChanged(bool byPlayer)
    {
        RecalculateOccupancies();
        if (byPlayer)
        {
            _timeSincePlayerSprayed = 0.0f;
        }
    }

    public void SpawnEnemy()
    {
        // Pick a random tag spline unoccupied by the enemy
        var randomTagSplines = tagSplines.OrderBy(x => UnityEngine.Random.value).ToList();

        foreach (var tagSpline in randomTagSplines)
        {
            if (tagSpline.occupant != TagSpline.TagSplineOccupant.Enemy)
            {
                ShowEnemy();
                tagSplineToOverrideByEnemy = tagSpline;
                Invoke(nameof(FinishEnemySpray), GameManager.enemySprayDuration);
                break;
            }
        }
    }

    void ShowEnemy()
    {
        enemy.SetActive(true);
        _enemyPresent = true;
        _timeSinceLastEnemySpawn = 0.0f;
        _sprayAudioSource.Play();
    }

    public void FinishEnemySpray()
    {
        enemy.SetActive(false);

        if (_enemyPresent && tagSplineToOverrideByEnemy != null)
        {
            tagSplineToOverrideByEnemy.OvertakeByEnemy();
            RecalculateOccupancies();
            Debug.Log($"TagSpot_{name} overriden by enemy.");
        }

        _enemyPresent = false;
        _sprayAudioSource.Stop();
    }

    public bool CanSpawnEnemy()
    {
        bool notFullyOccupied = GetEnemyPercentage() < 1.0f;
        bool enoughTimeSinceLastSpawn = _timeSinceLastEnemySpawn > 10.0f;
        bool enoughTimeSincePlayerSprayed = _timeSincePlayerSprayed > 10.0f;
        bool playerFarEnough = Vector3.Distance(transform.position, PlayerManager.Instance.transform.position) > 5.0f;
        return !_enemyPresent && notFullyOccupied && enoughTimeSinceLastSpawn && enoughTimeSincePlayerSprayed && playerFarEnough;
    }

    void RecalculateOccupancies()
    {
        _splinesOccupiedByPlayer = 0;
        _splinesOccupiedByEnemy = 0;

        foreach (var tagSpline in tagSplines)
        {
            if (tagSpline.occupant == TagSpline.TagSplineOccupant.Player)
            {
                _splinesOccupiedByPlayer += 1;
            }
            else if (tagSpline.occupant == TagSpline.TagSplineOccupant.Enemy)
            {
                _splinesOccupiedByEnemy += 1;
            }
        }
    }

    public void OnEnemyHit()
    {
        _enemyPresent = false;
        FinishEnemySpray();
    }
}
