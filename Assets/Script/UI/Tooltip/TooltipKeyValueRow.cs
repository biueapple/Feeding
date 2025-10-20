using UnityEngine;
using TMPro;

public class TooltipKeyValueRow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI left, right;
    public void Set(string k, string v) { left.text = k; right.text = v; }
}
