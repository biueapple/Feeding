using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image icon;

    private Buff buff;

    //마우스를 올리면 설명도 보여야 함
    public void Init(Buff buff)
    {
        this.buff = buff;
        icon.sprite = buff.Icon;
    }

    //버프 설명 보여주기 닫기
    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
