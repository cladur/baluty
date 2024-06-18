using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;

public enum ColliderQuality
{
    Perfect,
    Good
}

public class TagSplineCollider : MonoBehaviour
{
    public int index;
    public ColliderQuality colliderQuality;
    public TagSpline TagSpline;
    public ScorePopup scorePopupPrefab;
    private BoxCollider _boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void OnShotWithPaint(SprayColor color)
    {
        if (TagSpline.OnColliderTriggered(index, colliderQuality, color))
        {
            gameObject.SetActive(false);
            // Instantiate empty object
            var empty = new GameObject();
            empty.transform.position = transform.position + transform.right * 0.1f;
            var scorePopup = Instantiate(scorePopupPrefab, transform.position, Quaternion.LookRotation(-transform.right), empty.transform);
            scorePopup.StartAnimation(colliderQuality);
            Destroy(scorePopup.gameObject, 1.0f);
            Destroy(empty, 1.0f);
        }
    }
}
