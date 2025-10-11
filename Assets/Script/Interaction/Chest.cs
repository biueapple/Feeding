using UnityEngine;
using UnityEngine.EventSystems;

//�ϴ� �̰� UI���� ������Ʈ���� �𸣰����� �Ѵ� ����� ����
public class Chest : MonoBehaviour, IPointerDownHandler, IClickable
{
    [SerializeField]
    private int length;
    public int Length => length;

    private InventoryInterface inventoryInterface;
    public InventoryInterface InventoryInterface => inventoryInterface;

    private void Awake()
    {
        inventoryInterface = new(length);
    }

    //ui������ Ŭ��
    public void OnPointerDown(PointerEventData eventData)
    {
        UIManager.Instance.OpenStorageInterface(inventoryInterface, transform.position + new Vector3(0, 250));
    }

    //������Ʈ ������ Ŭ��
    public void OnClick(RaycastHit hit)
    {
        UIManager.Instance.OpenStorageInterface(inventoryInterface, Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0, 350));
    }

    public void OnDoubleClick(RaycastHit hit)
    {

    }
}
