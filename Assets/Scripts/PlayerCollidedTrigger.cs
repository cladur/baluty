using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollidedTrigger : MonoBehaviour
{
    bool _wasTriggered = false;

    void OnTriggerEnter(Collider other)
    {
        if (_wasTriggered)
        {
            return;
        }
        _wasTriggered = true;
        TutorialManager.Instance.ShowTagSpots();
    }
}
