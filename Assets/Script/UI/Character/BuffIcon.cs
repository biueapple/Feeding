using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private Image icon;

    private Buff buff;

    //���콺�� �ø��� ���� ������ ��
    public void Init(Buff buff)
    {
        this.buff = buff;
        icon.sprite = buff.Icon;
    }

    //���� ���� �����ֱ� �ݱ�
    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void OnPointerExit(PointerEventData eventData)
    {

    }
}
