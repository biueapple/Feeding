using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField]
    private Canvas canvas;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField]
    private StorageUserInterface storageUIPrefab;
    //비활성화된 UI를 보관
    private readonly Queue<StorageUserInterface> storageQueue = new();
    //열려있는 UI를 담아두는 곳
    private readonly Dictionary<InventoryInterface, StorageUserInterface> openStorageUI = new();

    [SerializeField]
    private DragSlotUI dragSlot;
    public DragSlotUI DragSlot => dragSlot;

    [SerializeField]
    private ItemSlotUI tradeSlot;
    public ItemSlotUI TradeSlot => tradeSlot;

    //UI를 열때 clamp 해서 화면을 나가지 않도록 하고 UI에 드래그로 움직일 수 있도록 추가해야 함

    public void OpenStorageInterface(InventoryInterface inventory, Vector3 position = default)
    {
        //이미 열려있다는 뜻
        if (openStorageUI.ContainsKey(inventory)) return;


        //비황성화 상태로 있는 UI가 있다면 그걸로 열기
        if (!storageQueue.TryDequeue(out StorageUserInterface result))
        {
            //없다면 생성해서 열기
            result = Instantiate(storageUIPrefab, canvas.transform);
        }

        result.Interlock(inventory);
        result.Open();
        openStorageUI[inventory] = result;

        result.transform.position = position;
        ClampPosition(result.GetComponent<RectTransform>());
    }

    public void CloseStorageInterface(InventoryInterface inventory)
    {
        if(openStorageUI.ContainsKey(inventory))
        {
            openStorageUI[inventory].Close();
            storageQueue.Enqueue(openStorageUI[inventory]);
            openStorageUI.Remove(inventory);
        }
    }









    private void ClampPosition(RectTransform ui)
    {
        float minX = ui.rect.width * 0.5f;
        float maxX = Screen.width - ui.rect.width * 0.5f;
        float minY = ui.rect.height * 0.5f;
        float maxY = Screen.height - ui.rect.height * 0.5f;

        Vector2 screen = new Vector2(Mathf.Clamp(ui.transform.position.x, minX, maxX) , Mathf.Clamp(ui.transform.position.y, minY, maxY));
        ui.transform.position = screen;
    }
}
