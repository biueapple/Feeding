using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DayCycleManager : MonoBehaviour
{
    public static DayCycleManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(DayCycle());
    }

    private IEnumerator DayCycle()
    {
        //�Ϸ縦 �ݺ�
        while (true)
        {
            yield return Morning();
            yield return Dinner();
            yield return Night();
        //            DayCount++;
        }
    }

    //��ħ �����ϸ� ȣ��
    public event Action OnMorningStart;
    //��� ������ �Ŀ� ȣ��
    public event Action OnEquipAction;
    //���� �Ϸ� �� �Ŀ� ȣ��
    public event Action OnAdventure;
    //�ŷ� ������ �Ŀ� ȣ��
    public event Action OnTrade;
    //���� ������ �� ȣ��
    public event Action OnNight;

    [SerializeField]
    private Hero hero;
    //��ħ ����
    //��� ì�ܼ� ���� ����
    public IEnumerator Morning()
    {
        OnMorningStart?.Invoke();

        yield return hero.WakeUp();
        yield return hero.MoveToChest();

        //��� �����Ϸ� ����
        yield return InventoryManager.Instance.RunEquipPhase();
        OnEquipAction?.Invoke();

        yield return hero.OutHome();
        yield return hero.InDungeon();


        //�����Ϸ� ����
        OnAdventure?.Invoke();
        yield return AdventureManager.Instance.StartAdventure(hero);
        bool next = false;
        void action()
        {
            next = true;
            AdventureManager.Instance.OnAdventureEnded -= action;
        }

        AdventureManager.Instance.OnAdventureEnded += action; 

        //���� �����ϸ� �ŷ� ����
        OnTrade?.Invoke();
        //yield return new WaitForSeconds(1);
        ShopManager.Instance.StartEncounter(ShopManager.Instance.CreateVisitor());

        yield return new WaitUntil(() => next);
    }

    //���� ����
    public IEnumerator Dinner()
    {
        //���� ������ �ŷ��� ������ ��
        ShopManager.Instance.TerminationTrade();
        //��簡 �����ؾ� ��
        yield return hero.OutDungeon();
        yield return hero.InHome();

        //���ڿ� ������ �ֱ�
        yield return InventoryManager.Instance.RunUnequipPhase();

        yield return hero.MoveToTable();

        //��Ա�
        yield return IngestionManager.Instance.Ingestion();

        yield return hero.MoveToBad();

        //�ڱ�
        yield return Sleep();

        yield return null;
    }

    //���� �ڰ� �� ������(���� �ƹ��͵� ����) ����
    public IEnumerator Night()
    {
        OnNight?.Invoke();

        Debug.Log("Ŭ���ϸ� �������� �Ѿ");
        yield return WaitForPlayerClick();
    }
    //��ħ ����


    //����
    public IEnumerator Sleep()
    {
        yield return new WaitForSeconds(1);
        RecoveryEventArgs args = new("Sleep", hero);
        args.Recovery.Add(new RecoveryPacket("Sleep", 20));
        hero.Recovery(args);
    }

    private IEnumerator WaitForPlayerClick()
    {
        yield return new WaitUntil(() => Keyboard.current.anyKey.isPressed);

        //UIManager.Instance.HideNextButton();
    }
}
