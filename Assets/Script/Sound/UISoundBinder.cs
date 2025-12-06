using UnityEngine;
using UnityEngine.EventSystems;

public class UISoundBinder : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField]
    private SoundType enter;
    [SerializeField]
    private SoundType click;

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.Instance.Play(click);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.Instance.Play(enter);
    }
}
