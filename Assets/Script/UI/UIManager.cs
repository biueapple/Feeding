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

    //UI�� ���� clamp �ؼ� ȭ���� ������ �ʵ��� �ϰ� UI�� �巡�׷� ������ �� �ֵ��� �߰��ؾ� ��

    public void OpenStorageInterface(InventoryInterface inventory, Vector3 position = default)
    {
        //�̹� �����ִٴ� ��
        if (openStorageUI.ContainsKey(inventory)) return;


        //��Ȳ��ȭ ���·� �ִ� UI�� �ִٸ� �װɷ� ����
        if (!storageQueue.TryDequeue(out StorageUserInterface result))
        {
            //���ٸ� �����ؼ� ����
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
