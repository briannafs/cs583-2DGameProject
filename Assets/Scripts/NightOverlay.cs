using UnityEngine;
using UnityEngine.UI;

public class NightOverlay : MonoBehaviour {
    public Image overlay; // black image
    [Range(0,1)] public float nightAlpha = 0.6f;

    void Update(){
        if (!TimeOfDay.I || overlay==null) return;
        bool night = TimeOfDay.I.IsNight();
        float target = night ? nightAlpha : 0f;
        Color c = overlay.color; c.a = Mathf.MoveTowards(c.a, target, Time.deltaTime*0.8f);
        overlay.color = c;
    }
}
