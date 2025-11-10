using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class VisitorText : MonoBehaviour, IPointerDownHandler
{
    [SerializeField]
    private TextMeshProUGUI text;

    public event Action OnClick;

    public void OnPointerDown(PointerEventData eventData)
    {
        OnClick?.Invoke();
    }

    public void Texting(string str)
    {
        text.text = str;
    }
}
