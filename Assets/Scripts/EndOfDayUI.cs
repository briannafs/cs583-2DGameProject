using UnityEngine;
using TMPro;

public class EndOfDayUI : MonoBehaviour {
    [Header("Panel")]
    public GameObject panel;
    [Header("Labels")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI profitText;
    public TextMeshProUGUI balanceText;
    public TextMeshProUGUI noteText;   // optional

    public void Show(int profit, int balance, int dayJustEnded, bool passedOut, int penalty){
        if (panel) panel.SetActive(true);
        if (titleText)   titleText.text   = $"End of Day {dayJustEnded}";
        if (profitText)  profitText.text  = $"Profit: +${profit}";
        if (balanceText) balanceText.text = $"Balance: ${balance}";

        if (noteText){
            if (passedOut){
                if (penalty > 0) noteText.text = $"You passed out and were taken home.\nFee: ${penalty}.";
                else             noteText.text = "You passed out and were taken home.\n(No fee charged.)";
            } else {
                noteText.text = "You slept well.";
            }
        }

        // pause the clock while panel is visible
        TimeOfDay.I?.SetPaused(true);
    }

    // Hook this to the Continue button's OnClick
    public void OnContinue(){
        if (panel) panel.SetActive(false);
        // Resume the new day
        TimeOfDay.I?.StartNewDayUnpause();
    }
}
