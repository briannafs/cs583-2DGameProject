using UnityEngine;
using System;

public class GameManager : MonoBehaviour {
    public static GameManager I;

    public int day = 1;
    public int money = 0;
    public int dayProfit = 0;
    public int lastDayProfit = 0;

    // Pass-out penalty fields
    public int passOutFee = 25;   // tweak in Inspector
    public bool lastPassedOut { get; private set; }
    public int lastPenalty { get; private set; }

    public event Action OnNewDay;

    void Awake() {
        if (I == null) { I = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public void AddMoney(int amount) {
        money += amount;
        dayProfit += amount;
    }

    // Called by TimeOfDay.ForceSleep(passOut)
    public void SleepAndAdvanceDay(bool passOut) {
        // finalize today's profit
        lastDayProfit = dayProfit;
        dayProfit = 0;

        // Grow crops, etc.
        OnNewDay?.Invoke();
        var farm = FindAnyObjectByType<FarmGrid>();
        if (farm) farm.OnNewDay();

        // Record and apply penalty if passed out
        lastPassedOut = passOut;
        lastPenalty = 0;
        if (passOut && passOutFee > 0) {
            lastPenalty = Mathf.Min(money, passOutFee); // don't go negative
            money -= lastPenalty;
        }

        // Show summary popup (EndOfDayUI)
        var ui = FindAnyObjectByType<EndOfDayUI>();
        if (ui) ui.Show(lastDayProfit, money, day, lastPassedOut, lastPenalty);
    }
}
