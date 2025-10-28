using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public enum SoilState { Untilled, Tilled, Watered }

public class FarmGrid : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tilemap soilMap;
    public Tilemap cropMap;

    [Header("Soil Variants")]
    [Tooltip("Tiles you PAINT by hand for UNTILLED soil (corners, edges, etc.)")]
    public TileBase[] soilUntilledVariants;   // painted manually
    [Tooltip("Matching variants for TILLED soil (same order as untilled array).")]
    public TileBase[] soilTilledVariants;
    [Tooltip("Matching variants for WATERED soil (same order as untilled array).")]
    public TileBase[] soilWateredVariants;

    [Header("Crop Stage Tiles (3 stages)")]
    public TileBase[] cropStageTiles;

    [Header("Growth Tuning")]
    [Tooltip("How many WATERED nights until the crop is fully grown.")]
    public int daysToMature = 3;

    // -------- internal cell data --------
    class Cell
    {
        public SoilState soil = SoilState.Untilled;
        public int daysWatered = 0;
        public int cropStage = -1;
        public bool wateredToday = false;
        public int variantIndex = 0;     // remembers which corner/edge variant it was

        public bool HasCrop => cropStage >= 0;
    }

    private readonly Dictionary<Vector3Int, Cell> cells = new();

    void Start()
    {
        // Pre-seed cells for every painted soil tile
        var bounds = soilMap.cellBounds;
        foreach (var p in bounds.allPositionsWithin)
        {
            if (soilMap.HasTile(p))
            {
                var tile = soilMap.GetTile(p);
                int index = IndexOf(soilUntilledVariants, tile);
                if (!cells.ContainsKey(p))
                    cells[p] = new Cell { soil = SoilState.Untilled, variantIndex = index };
            }
        }
    }

    int IndexOf(TileBase[] arr, TileBase tile)
    {
        if (arr == null || tile == null) return 0;
        for (int i = 0; i < arr.Length; i++)
            if (arr[i] == tile) return i;
        return 0;
    }

    TileBase SafeGet(TileBase[] arr, int i)
    {
        if (arr == null || arr.Length == 0) return null;
        return arr[Mathf.Clamp(i, 0, arr.Length - 1)];
    }

    // -------- helpers --------
    Vector3Int WorldToCell(Vector3 worldPos) => soilMap.WorldToCell(worldPos);
    bool HasSoil(Vector3Int p) => soilMap != null && soilMap.HasTile(p);

    Cell Get(Vector3Int p)
    {
        if (!cells.TryGetValue(p, out var c))
        {
            if (!HasSoil(p)) return null;
            c = new Cell();
            cells[p] = c;
        }
        return c;
    }

    int StageFromProgress(int days)
    {
        if (cropStageTiles == null || cropStageTiles.Length == 0) return 0;
        int maxStage = cropStageTiles.Length - 1;
        int clamped = Mathf.Clamp(days, 0, daysToMature);
        float t = (float)clamped / Mathf.Max(1, daysToMature);
        int stage = Mathf.FloorToInt(t * maxStage);
        return Mathf.Clamp(stage, 0, maxStage);
    }

    // -------- player actions --------
    public void Till(Vector3 worldPos)
    {
        var p = WorldToCell(worldPos);
        if (!HasSoil(p)) return;
        var c = Get(p); if (c == null) return;
        if (c.soil == SoilState.Untilled)
        {
            c.soil = SoilState.Tilled;
            SetSoilVisual(p, c.soil, c.variantIndex);
        }
    }

    public void Water(Vector3 worldPos)
    {
        var p = WorldToCell(worldPos);
        if (!HasSoil(p)) return;
        var c = Get(p); if (c == null) return;
        if (c.soil == SoilState.Tilled || c.soil == SoilState.Watered)
        {
            c.soil = SoilState.Watered;
            c.wateredToday = true;
            SetSoilVisual(p, c.soil, c.variantIndex);
        }
    }

    public void Plant(Vector3 worldPos)
    {
        var p = WorldToCell(worldPos);
        if (!HasSoil(p)) return;
        var c = Get(p); if (c == null) return;
        if (c.soil == SoilState.Tilled && !c.HasCrop)
        {
            c.daysWatered = 0;
            c.cropStage = 0;
            c.wateredToday = false;
            SetCropVisual(p, c.cropStage);
        }
    }

    public void TryHarvest(Vector3 worldPos)
    {
        var p = WorldToCell(worldPos);
        if (!HasSoil(p)) return;
        var c = Get(p); if (c == null || !c.HasCrop) return;
        if (c.daysWatered >= daysToMature)
        {
            c.cropStage = -1;
            c.daysWatered = 0;
            SetCropVisual(p, -1);
            if (Inventory.I != null) Inventory.I.Add(ItemType.Wheat, 1);
            Debug.Log("Harvested Wheat.");
        }
    }

    // -------- day progression --------
    public void OnNewDay()
    {
        foreach (var kv in cells)
        {
            var p = kv.Key;
            var c = kv.Value;

            if (c.HasCrop)
            {
                if (c.wateredToday)
                    c.daysWatered++;
                c.cropStage = StageFromProgress(c.daysWatered);
                SetCropVisual(p, c.cropStage);
                c.wateredToday = false;
            }

            // Dry soil overnight
            if (c.soil != SoilState.Untilled)
            {
                c.soil = SoilState.Tilled;
                SetSoilVisual(p, c.soil, c.variantIndex);
            }
        }
    }

    // -------- visuals --------
    void SetSoilVisual(Vector3Int p, SoilState state, int variantIndex)
    {
        TileBase tile = null;

        switch (state)
        {
            case SoilState.Untilled:
                tile = SafeGet(soilUntilledVariants, variantIndex);
                break;
            case SoilState.Tilled:
                tile = SafeGet(soilTilledVariants, variantIndex);
                break;
            case SoilState.Watered:
                tile = SafeGet(soilWateredVariants, variantIndex);
                break;
        }

        soilMap.SetTile(p, tile);
    }

    void SetCropVisual(Vector3Int p, int stage)
    {
        if (stage < 0)
        {
            cropMap.SetTile(p, null);
            return;
        }
        if (cropStageTiles != null && stage < cropStageTiles.Length)
            cropMap.SetTile(p, cropStageTiles[stage]);
    }
}
