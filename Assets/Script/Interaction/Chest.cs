using UnityEngine;
using UnityEngine.EventSystems;

//일단 이게 UI일지 오브젝트일지 모르겠으니 둘다 만들어 놓기
public class Chest : MonoBehaviour, IPointerDownHandler, IClickable
{
    [SerializeField]
    private int length;
    public int Length => length;

    [SerializeField]
    private Animator animator;

    private InventoryInterface inventoryInterface;
    public InventoryInterface InventoryInterface => inventoryInterface;

    private void Awake()
    {
        inventoryInterface = new(length);
    }

    //ui상태인 클릭 실제 사용은 이거
    public void OnPointerDown(PointerEventData eventData)
    {
        animator.SetBool("IsOpened", true);
        StorageUserInterface storage = UIManager.Instance.OpenStorageInterface(inventoryInterface, transform.position + new Vector3(0, 250));
        storage.OnClose += Storage_OnClose;
    }

    private void Storage_OnClose(InventoryInterface obj)
    {
        animator.SetBool("IsOpened", false);
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
