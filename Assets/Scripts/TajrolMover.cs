using UnityEngine;

public class TajrolMover : MonoBehaviour
{
    public bool isFirstPosition;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered");

        if (!other.CompareTag("Player"))
        {
            return;
        }

        Debug.Log("Player entered");

        var tajrol = transform.parent.parent.parent.GetComponent<Tajrol>();

        if (tajrol.IsFirstPosition != isFirstPosition)
        {
            tajrol.MoveTajrolToOtherSide();
        }
    }
}
