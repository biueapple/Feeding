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
    //��Ȱ��ȭ�� UI�� ����
    private readonly Queue<StorageUserInterface> storageQueue = new();
    //�����ִ� UI�� ��Ƶδ� ��
    private readonly Dictionary<InventoryInterface, StorageUserInterface> openStorageUI = new();

    [SerializeField]
    private DragSlotUI dragSlot;
    public DragSlotUI DragSlot => dragSlot;

    [SerializeField]
    private ItemSlotUI tradeSlot;
    public ItemSlotUI TradeSlot => tradeSlot;

    //ui�� �ܰ躰�� �������� �� �� ���Ƽ�
    [SerializeField]
    private Transform one;
    [SerializeField]
    private Transform two;
    [SerializeField]
    private Transform thr;


    //
    //�κ��丮 ���õ� �޼ҵ�
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
    }

    public void CloseClosetInterface(ClosetUserInterface closetInterface)
    {
        closetInterface.gameObject.SetActive(false);
        closetInterface.Close();
    }

    public void OpenStorageInterface(InventoryInterface inventory, Vector3 position = default)
    {
        //�̹� �����ִٴ� ��
        if (openStorageUI.ContainsKey(inventory)) return;


        //��Ȳ��ȭ ���·� �ִ� UI�� �ִٸ� �װɷ� ����
        if (!storageQueue.TryDequeue(out StorageUserInterface result))
        {
            //���ٸ� �����ؼ� ����
            result = Instantiate(storageUIPrefab, two);
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
    //
    //
    //


    //
    //���� ������ ���õ� �޼ҵ�
    //
    public void OpenBuffInfo()
    {

    }

    public void CloseBuffInfo()
    {

    }





    public void ClampPosition(RectTransform ui)
    {
        float minX = ui.rect.width * ui.pivot.x;
        float maxX = Screen.width - ui.rect.width * (1 - ui.pivot.x);
        float minY = ui.rect.height * ui.pivot.y;
        float maxY = Screen.height - ui.rect.height * (1 - ui.pivot.y);

        Vector2 screen = new (Mathf.Clamp(ui.transform.position.x, minX, maxX) , Mathf.Clamp(ui.transform.position.y, minY, maxY));
        ui.transform.position = screen;
    }
}
