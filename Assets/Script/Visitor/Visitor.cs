using UnityEngine;
using UnityEngine.UI;

public class Visitor : MonoBehaviour
{
    [SerializeField]
    private VisitorSO so;
    public VisitorSO SO { get => so; set { so = value; } }

    [SerializeField]
    private Image image;

    public void Init()
    {
        image.sprite = so.Portrait;
    }
}
