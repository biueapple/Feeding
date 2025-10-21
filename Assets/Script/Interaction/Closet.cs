using UnityEngine;
using UnityEngine.EventSystems;

public class Closet : MonoBehaviour, IPointerDownHandler, IClickable
{
    [SerializeField]
    private int length;
    public int Length => length;

    [SerializeField]
    private ClosetUserInterface closetUserInterface;

    private InventoryInterface inventoryInterface;
    public InventoryInterface InventoryInterface => inventoryInterface;

    private void Awake()
    {
        inventoryInterface = new(length);
    }

    //ui상태인 클릭 사용중
    public void OnPointerDown(PointerEventData eventData)
    {
        UIManager.Instance.OpenClosetInterface(inventoryInterface, closetUserInterface, transform.position + new Vector3(0, 250));
    }

    //오브젝트 상태인 클릭 미사용중
    public void OnClick(RaycastHit hit)
    {
        UIManager.Instance.OpenClosetInterface(inventoryInterface, closetUserInterface, Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0, 350));
    }

    public void OnDoubleClick(RaycastHit hit)
    {

    }
}
