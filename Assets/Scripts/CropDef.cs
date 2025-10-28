// Crop Definition
using UnityEngine;

[CreateAssetMenu(menuName="Game/Crop")]
public class CropDef : ScriptableObject {
    public string id = "wheat";
    public int stages = 3;             // 0..2
    public int daysPerStage = 1;       // how many sleeps to advance
    public bool requiresWater = true;  // growth only if watered that day
    public Sprite[] stageSprites;      // optional; else we’ll use Tiles
}
