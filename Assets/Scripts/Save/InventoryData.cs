using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryData
{
    public List<string> inventoryItems = new List<string>();
    public List<int> inventoryAmounts = new List<int>();

    public InventoryData(Inventory inventory)
    {
        foreach(KeyValuePair<SpellObjects, int> pair in inventory.GetInventoryDict())
        {
            inventoryItems.Add(pair.Key.GetName());
            inventoryAmounts.Add(pair.Value);
        }
    }
}
