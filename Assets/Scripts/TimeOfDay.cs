using UnityEngine;
using System;

public class TimeOfDay : MonoBehaviour {
    public static TimeOfDay I;

    [Header("Clock")]
    [Tooltip("In-game minutes per real second (1 = 1 min/sec).")]
    public float minutesPerRealSecond = 1f;
    [Tooltip("Hour to start each new day (24h).")]
    public int startHour = 6;

    [Header("Rules")]
    [Tooltip("Night begins at this hour (24h).")]
    public int nightStartsHour = 20; // 8pm
    [Tooltip("Latest time you can be out; at/after this you pass out.")]
    public int passOutHour = 2;      // 2am
    [Tooltip("Wake time after sleeping or passing out (24h).")]
    public int wakeHour = 6;

    public int Day { get; private set; } = 1;
    public int Hour { get; private set; }
    public int Minute { get; private set; }

    public event Action OnMinute;
    public event Action OnHour;

    bool paused;              // pause the clock (e.g., during popup)
    bool hasSleptThisDay;     // prevents multiple pass-outs

    float accum;

    void Awake() {
        if (I == null) I = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
        Hour = startHour; Minute = 0;
        hasSleptThisDay = false;
    }

    void Update() {
        if (paused) return;

        accum += Time.deltaTime * minutesPerRealSecond;
        while (accum >= 1f) {
            accum -= 1f;
            AdvanceMinute();

            // Pass-out only when we hit the exact curfew time once
            if (!hasSleptThisDay && Hour == passOutHour && Minute == 0) {
                ForceSleep(passOut: true);
                break; // we've just slept; stop advancing further this frame
            }
        }
    }


    void AdvanceMinute() {
        Minute++;
        if (Minute >= 60) { Minute = 0; Hour++; OnHour?.Invoke(); }
        if (Hour >= 24) { Hour = 0; } // wrap without changing Day
        OnMinute?.Invoke();
    }

    public bool IsNight() {
        // Night spans [nightStartsHour..23] and [0..passOutHour)
        return Hour >= nightStartsHour || Hour < passOutHour;
    }

    public bool CanSleepNow() {
        return IsNight();
    }

    /// <summary>
    /// Sleep or pass out. Increments Day, resets time to morning,
    /// and asks GameManager to advance the game (crops, summary).
    /// </summary>
    public void ForceSleep(bool passOut) {
        // Mark we slept for this calendar cycle so we don't pass out twice.
        hasSleptThisDay = true;

        // Advance day + game logic first (so UI can show last day's summary)
        var gm = GameManager.I;
        if (gm != null) gm.SleepAndAdvanceDay(passOut);

        // Reset the clock to the new morning
        Day++;
        Hour = wakeHour;
        Minute = 0;
    }

    /// <summary>Call when the player dismisses the end-of-day popup.</summary>
    public void StartNewDayUnpause() {
        hasSleptThisDay = false;
        SetPaused(false);
    }

    public void SetPaused(bool value) { paused = value; }

    public string TimeString() => $"{Hour:00}:{Minute:00}";

    // --- DEBUG: instantly jump to night time ---
    public void SetTimeToNight()
    {
        Hour = nightStartsHour;
        Minute = 59;
    }

}
