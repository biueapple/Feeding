using System.Collections.Generic;
using UnityEngine;

public class TooltipView : MonoBehaviour
{
    float height = 0;

    public void Attaching(RectTransform rect)
    {
        rect.transform.SetParent(transform);
        rect.transform.localPosition = new (0, height);
        height -= rect.rect.height;
    }

    public void Clear()
    {
        height = 0;        
    }
}
