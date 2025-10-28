using UnityEngine;

public enum Tool { Hoe, WateringCan, Seed }

public class InteractController : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRadius = 0.8f;       // radius for E key interactions
    public int toolReachTiles = 1;            // how many tiles away tools can reach
    public Camera cam;
    public Tool currentTool = Tool.Hoe;

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        // --- Tool hotkeys ---
        if (Input.GetKeyDown(KeyCode.Alpha1)) currentTool = Tool.Hoe;
        if (Input.GetKeyDown(KeyCode.Alpha2)) currentTool = Tool.WateringCan;
        if (Input.GetKeyDown(KeyCode.Alpha3)) currentTool = Tool.Seed;

        // --- Interact with nearby objects (E) ---
        if (Input.GetKeyDown(KeyCode.E))
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, interactRadius);
            foreach (var h in hits)
            {
                var inter = h.GetComponent<IInteractable>();
                if (inter != null)
                {
                    inter.Interact(gameObject);
                    break;
                }
            }
        }

        // --- Use tool / harvest on mouse click ---
        if (Input.GetMouseButtonDown(0))
        {
            if (cam == null) return;

            // world position of the mouse click
            Vector3 w = cam.ScreenToWorldPoint(Input.mousePosition);
            w.z = 0f;

            var farm = FindAnyObjectByType<FarmGrid>();
            if (farm == null) return;

            // 🔒 Check reach distance before allowing action
            if (!IsWithinToolReach(w, farm))
            {
                // Optional feedback: Debug.Log("Tile out of reach.");
                return;
            }

            switch (currentTool)
            {
                case Tool.Hoe:          farm.Till(w);  break;
                case Tool.WateringCan:  farm.Water(w); break;
                case Tool.Seed:         farm.Plant(w); break;
            }

            // Try to harvest if clicked crop is ready
            farm.TryHarvest(w);
        }

        // -----------------------
        // DEBUG CONTROLS (Editor only)
        // -----------------------
#if UNITY_EDITOR
        // G = Skip to next day instantly
        if (Input.GetKeyDown(KeyCode.G))
        {
            TimeOfDay.I?.ForceSleep(false);   // behaves like sleeping normally
        }

        // H = Jump to night (for testing sleep/pass-out)
        if (Input.GetKeyDown(KeyCode.H))
        {
            if (TimeOfDay.I != null)
            {
                TimeOfDay.I.SetTimeToNight();
                Debug.Log("DEBUG: Time set to night for testing sleep/pass-out.");
            }
        }
#endif
    }

    // --- Helper: check tool reach distance ---
    bool IsWithinToolReach(Vector3 worldPos, FarmGrid farm)
    {
        if (farm == null || farm.soilMap == null) return false;

        var grid = farm.soilMap;
        Vector3Int pTarget = grid.WorldToCell(worldPos);
        Vector3Int pPlayer = grid.WorldToCell(transform.position);

        int dx = Mathf.Abs(pTarget.x - pPlayer.x);
        int dy = Mathf.Abs(pTarget.y - pPlayer.y);

        // Chebyshev distance (includes diagonals)
        int chebyshev = Mathf.Max(dx, dy);
        return chebyshev <= toolReachTiles;
    }

    // --- Gizmo to visualize interact radius ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
