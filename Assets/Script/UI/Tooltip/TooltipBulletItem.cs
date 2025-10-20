using TMPro;
using UnityEngine;

public class TooltipBulletItem : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI text;
    public void Set(string s) { text.text = s;}
}
