using UnityEngine;

public class TajrolMover : MonoBehaviour
{
    public bool isFirstPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        var tajrol = transform.parent.parent.parent.GetComponent<Tajrol>();

        if (tajrol.IsFirstPosition != isFirstPosition)
        {
            tajrol.MoveTajrolToOtherSide();
        }
    }
}
