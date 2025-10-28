using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class House : MonoBehaviour, IInteractable {
    [Tooltip("Where the player appears after sleeping (optional).")]
    public Transform wakePoint;

    public void Interact(GameObject interactor){
        if (TimeOfDay.I == null) { Debug.LogWarning("TimeOfDay missing."); return; }
        if (!TimeOfDay.I.CanSleepNow()) { Debug.Log("Too early to sleep!"); return; }

        TimeOfDay.I.ForceSleep(passOut:false);

        if (wakePoint != null)
            interactor.transform.position = wakePoint.position;
    }
}
