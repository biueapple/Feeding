using UnityEngine;
using UnityEngine.EventSystems;

//일단 이게 UI일지 오브젝트일지 모르겠으니 둘다 만들어 놓기
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

    //ui상태인 클릭
    public void OnPointerDown(PointerEventData eventData)
    {
        UIManager.Instance.OpenStorageInterface(inventoryInterface, transform.position + new Vector3(0, 250));
    }

    //오브젝트 상태인 클릭
    public void OnClick(RaycastHit hit)
    {
        UIManager.Instance.OpenStorageInterface(inventoryInterface, Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0, 350));
    }

    public void OnDoubleClick(RaycastHit hit)
    {

    }
}
