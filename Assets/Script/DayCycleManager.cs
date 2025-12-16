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
        WorldContext.Instance.OnVillageChanged += Instance_OnVillageChanged;
        //StartCoroutine(DayCycle());
    }

    private void Instance_OnVillageChanged(VillageSO obj)
    {
        StartCoroutine(DayCycle());
    }

    private IEnumerator DayCycle()
    {
        //하루를 반복
        yield return Morning();
        yield return Dinner();
        yield return Night();
    }

    //아침 시작하면 호출
    public event Action OnMorningStart;
    //장비 장착한 후에 호출
    public event Action OnEquipAction;
    //모험 하러 간 후에 호출
    public event Action OnAdventure;
    //거래 시작한 후에 호출
    public event Action OnTrade;
    //밤이 시작할 때 호출
    public event Action OnNight;
    //내일 시작할 때 호출
    public event Action OnNextDay;

    [SerializeField]
    private Hero hero;
    //아침 시작
    //장비를 챙겨서 모험 시작
    public IEnumerator Morning()
    {
        OnMorningStart?.Invoke();

        yield return hero.WakeUp();
        yield return hero.MoveToChest();

        //장비 장착하러 가기
        yield return InventoryManager.Instance.RunEquipPhase();
        OnEquipAction?.Invoke();

        yield return hero.OutHome();
        yield return hero.InDungeon();


        //모험하러 가기
        OnAdventure?.Invoke();
        yield return AdventureManager.Instance.StartAdventure(hero);
        bool next = false;
        void action()
        {
            next = true;
            AdventureManager.Instance.OnAdventureEnded -= action;
        }

        AdventureManager.Instance.OnAdventureEnded += action;

        //모험 시작하면 거래 시작
        OnTrade?.Invoke();
        yield return new WaitForSeconds(1);
        //ShopManager.Instance.StartEncounter(ShopManager.Instance.CreateVisitor());
        ShopManager.Instance.StartShop();

        yield return new WaitUntil(() => next);
    }

    //모험 끝남
    public IEnumerator Dinner()
    {
        //모험 끝나면 거래가 끝나야 함
        ShopManager.Instance.TerminationTrade();
        //용사가 복귀해야 함
        yield return hero.OutDungeon();
        yield return hero.InHome();

        //상자에 아이템 넣기
        yield return InventoryManager.Instance.RunUnequipPhase();

        yield return hero.MoveToTable();

        //밥먹기
        yield return IngestionManager.Instance.Ingestion();

        yield return hero.MoveToBad();

        //자기
        yield return Sleep();

        yield return null;
    }

    //잠을 자고 밤 페이지(현재 아무것도 없음) 시작
    public IEnumerator Night()
    {
        OnNight?.Invoke();

        //Debug.Log("아무키나 누르면 다음날로 넘어감");
        //yield return WaitForPlayerClick();
        yield return null;
        OnNextDay?.Invoke();
    }
    //아침 시작


    //수면
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
