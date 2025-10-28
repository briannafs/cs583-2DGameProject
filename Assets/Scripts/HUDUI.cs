using UnityEngine;
using TMPro;

public class HUDUI : MonoBehaviour {
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dayText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI inventoryText;

    void Update(){
        if (TimeOfDay.I && timeText) timeText.text = TimeOfDay.I.TimeString();
        if (TimeOfDay.I && dayText)  dayText.text  = $"Day {TimeOfDay.I.Day}";
        if (GameManager.I && moneyText) moneyText.text = $"$ {GameManager.I.money}";

        if (inventoryText){
            var inv = Inventory.I;
            inventoryText.text = inv ? inv.ToDisplayString() : "(inv missing)";
        }
    }
}
