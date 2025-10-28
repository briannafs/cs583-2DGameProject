using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Farmstand : MonoBehaviour, IInteractable {
    [Header("Prices")]
    public int priceWheat = 5;
    public int priceBread = 20;

    public void Interact(GameObject interactor){
        var inv = Inventory.I;
        var gm  = GameManager.I;
        if (inv == null || gm == null){ Debug.LogWarning("Missing Inventory or GameManager."); return; }

        int wheat = inv.Get(ItemType.Wheat);
        int bread = inv.Get(ItemType.Bread);

        int total = wheat * priceWheat + bread * priceBread;

        if (total == 0){
            Debug.Log("Nothing to sell.");
            return;
        }

        inv.Clear();
        gm.AddMoney(total);
        Debug.Log($"Sold goods for ${total}. Balance ${gm.money}");
    }
}
