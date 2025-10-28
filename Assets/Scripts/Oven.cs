using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Oven : MonoBehaviour, IInteractable {
    public int wheatPerBread = 2;

    public void Interact(GameObject interactor){
        var inv = Inventory.I;
        if (inv == null) { Debug.LogWarning("No Inventory found."); return; }

        if (inv.Get(ItemType.Wheat) >= wheatPerBread){
            inv.TryRemove(ItemType.Wheat, wheatPerBread);
            inv.Add(ItemType.Bread, 1);
            Debug.Log("Baked 1 Bread!");
        } else {
            Debug.Log($"Need {wheatPerBread} Wheat to bake Bread.");
        }
    }
}
