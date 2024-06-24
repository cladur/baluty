using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public List<TagSpline> tagSplines = new();

    public Scores currentTagScores;
    public bool isTutorialSpot = false;

    private int _splinesOccupiedByPlayer;
    private int _splinesOccupiedByEnemy;
    private bool _enemyPresent;
    private float _timeSinceLastEnemySpawn;
    private float _timeSincePlayerSprayed;
    private TagSpline _tagSplineToOverrideByEnemy;
    private AudioSource _sprayAudioSource;

    // Start is called before the first frame update
    private void Start()
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

        if (!isTutorialSpot)
        {
            GameManager.Instance.tagSpots.Add(this);
        }
        else
        {
            _enemyPresent = true;
            enemy.SetActive(true);
        }
    }

    // Update is called once per frame
    private void Update()
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

    public void ContinueEnemy()
    {
        if (_enemyPresent)
        {
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        // Pick a random tag spline unoccupied by the enemy
        foreach (var tagSpline in tagSplines.OrderBy(_ => UnityEngine.Random.value).ToList()
                     .Where(tagSpline => tagSpline.occupant != TagSpline.TagSplineOccupant.Enemy))
        {
            ShowEnemy();
            _tagSplineToOverrideByEnemy = tagSpline;
            Invoke(nameof(FinishEnemySpray), GameManager.Instance.enemySprayDuration);
            return;
        }

        // No more tagsplines to override here.
        KillEnemy();
    }

    private void ShowEnemy()
    {
        if (_enemyPresent)
        {
            _sprayAudioSource.Play();
            return;
        }

        enemy.SetActive(true);
        _enemyPresent = true;
        _timeSinceLastEnemySpawn = 0.0f;
        _sprayAudioSource.Play();
        GameManager.Instance.CurrentEnemies += 1;
    }

    public void FinishEnemySpray()
    {
        if (_enemyPresent && _tagSplineToOverrideByEnemy != null)
        {
            _tagSplineToOverrideByEnemy.OvertakeByEnemy();
            RecalculateOccupancies();
            Debug.Log($"TagSpot_{name} overriden by enemy.");
        }

        _sprayAudioSource.Stop();

        Invoke(nameof(ContinueEnemy), GameManager.Instance.delayBetweenTagSplines);
    }

    public void KillEnemy()
    {
        enemy.SetActive(false);

        _enemyPresent = false;
        _sprayAudioSource.Stop();
        GameManager.Instance.CurrentEnemies -= 1;
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

        currentTagScores.SetScore(
            playerScores: _splinesOccupiedByPlayer,
            enemyScores: _splinesOccupiedByEnemy);
    }

    public void OnEnemyHit()
    {
        _enemyPresent = false;
        KillEnemy();
    }
}
