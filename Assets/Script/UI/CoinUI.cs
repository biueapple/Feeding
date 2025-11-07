using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InventoryManager.Instance.OnAfterGold += OnAfterGold;
        OnAfterGold(InventoryManager.Instance.Gold);
    }

    private void OnAfterGold(int gold)
    {
        text.text = gold.ToString();
    }
}
