using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    private InventoryInterface inventoryInterface;
    public InventoryInterface InventoryInterface { get => inventoryInterface; private set { } }


    private void Awake()
    {
        inventoryInterface = new(10);
    }
}
