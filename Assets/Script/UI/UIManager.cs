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

    //ui가 단계별로 나눠져야 할 것 같아서
    [SerializeField]
    private Transform one;
    [SerializeField]
    private Transform two;
    [SerializeField]
    private Transform thr;

    [SerializeField]
    private GameObject villageSelectUI;
    public GameObject VillageSelectUI => villageSelectUI;

    private void Start()
    {
        WorldContext.Instance.OnVillageChanged += Instance_OnVillageChanged;
        DayCycleManager.Instance.OnNextDay += Instance_OnNextDay;
    }

    private void Instance_OnNextDay()
    {
        villageSelectUI.gameObject.SetActive(true);
    }

    private void Instance_OnVillageChanged(VillageSO obj)
    {
        villageSelectUI.gameObject.SetActive(false);
    }

    //
    //인벤토리 관련된 메소드
    //
    public void OpenClosetInterface(InventoryInterface inventory, ClosetUserInterface closetInterface, Vector3 position = default)
    {
        if (!closetInterface.gameObject.activeSelf)
            closetInterface.gameObject.SetActive(true);
        else
            closetInterface.Close();

        closetInterface.transform.position = position;
        closetInterface.Interlock(inventory);
        closetInterface.Open();
        ClampPosition(closetInterface.GetComponent<RectTransform>());
    }

    public void CloseClosetInterface(ClosetUserInterface closetInterface)
    {
        closetInterface.gameObject.SetActive(false);
        closetInterface.Close();
    }

    public StorageUserInterface OpenStorageInterface(InventoryInterface inventory, Vector3 position = default)
    {
        //이미 열려있다는 뜻
        if (openStorageUI.ContainsKey(inventory)) return openStorageUI[inventory];


        //비황성화 상태로 있는 UI가 있다면 그걸로 열기
        if (!storageQueue.TryDequeue(out StorageUserInterface result))
        {
            //없다면 생성해서 열기
            result = Instantiate(storageUIPrefab, two);
        }

        result.Interlock(inventory);
        result.Open();
        openStorageUI[inventory] = result;

        result.transform.position = position;
        result.OnClose += CloseStorageInterface;
        ClampPosition(result.GetComponent<RectTransform>(), Vector2.zero, new Vector2(50, 0));

        return result;
    }

    private void CloseStorageInterface(InventoryInterface inventory)
    {
        if(openStorageUI.ContainsKey(inventory))
        {
            openStorageUI[inventory].Close();
            storageQueue.Enqueue(openStorageUI[inventory]);
            openStorageUI.Remove(inventory);
        }
    }
    //
    //
    //






    public void ClampPosition(RectTransform ui)
    {
        float minX = ui.rect.width * ui.pivot.x;
        float maxX = Screen.width - ui.rect.width * (1 - ui.pivot.x);
        float minY = ui.rect.height * ui.pivot.y;
        float maxY = Screen.height - ui.rect.height * (1 - ui.pivot.y);

        Vector2 screen = new (Mathf.Clamp(ui.transform.position.x, minX, maxX) , Mathf.Clamp(ui.transform.position.y, minY, maxY));
        ui.transform.position = screen;
    }

    public void ClampPosition(RectTransform ui, Vector2 minOffset, Vector2 maxOffset)
    {
        float minX = ui.rect.width * ui.pivot.x + minOffset.x;
        float maxX = Screen.width - ui.rect.width * (1 - ui.pivot.x) - maxOffset.x;
        float minY = ui.rect.height * ui.pivot.y + minOffset.y;
        float maxY = Screen.height - ui.rect.height * (1 - ui.pivot.y) - maxOffset.y;

        Vector2 screen = new(Mathf.Clamp(ui.transform.position.x, minX, maxX), Mathf.Clamp(ui.transform.position.y, minY, maxY));
        ui.transform.position = screen;
    }
}
