using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class Inventory : MonoBehaviour {
    public static Inventory I;

    private readonly Dictionary<ItemType,int> counts = new();

    void Awake(){
        if (I == null) { I = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
    }

    public int Get(ItemType t) => counts.TryGetValue(t, out var c) ? c : 0;

    public void Add(ItemType t, int amount = 1){
        if (amount <= 0) return;
        counts.TryGetValue(t, out var c);
        counts[t] = c + amount;
        Debug.Log($"+{amount} {t} (now {counts[t]})");
    }

    public bool TryRemove(ItemType t, int amount = 1){
        counts.TryGetValue(t, out var c);
        if (c < amount) return false;
        c -= amount;
        if (c == 0) counts.Remove(t); else counts[t] = c;
        Debug.Log($"-{amount} {t} (now {Get(t)})");
        return true;
    }

    public void Clear(){ counts.Clear(); }

    public IEnumerable<KeyValuePair<ItemType,int>> All() => counts;

    public string ToDisplayString(){
        if (counts.Count == 0) return "(empty)";
        var sb = new StringBuilder();
        foreach (var kv in counts) sb.AppendLine($"{kv.Key} x{kv.Value}");
        return sb.ToString().TrimEnd();
    }
}
