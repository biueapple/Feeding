using UnityEngine;

public class Inventory : MonoBehaviour
{
    public InventoryInterface InventoryInterface { get; private set; }


    private void Awake()
    {
        InventoryInterface = new(10);
    }
}
