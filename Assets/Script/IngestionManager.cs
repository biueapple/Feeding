using System.Collections;
using UnityEngine;

public class IngestionManager : MonoBehaviour
{
    public static IngestionManager Instance { get; private set; }

    [SerializeField]
    private Hero hero;

    [SerializeField]
    private FoodSlotUI slotUI;
    private ItemSlot slot;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        slot = new();
        slotUI.Init(slot);
    }

    public IEnumerator Ingestion()
    {
        if (slot.Item != null && slot.Item.TryGetAttribute<FoodAttribute>(out var attr))
        {
            attr.Apply(hero);
        }
        slot.Insert(null);
        yield return new WaitForSeconds(1);
    }
}
