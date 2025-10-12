using System;
using System.Collections;
using UnityEngine;

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
        //하루를 반복
        //while (true)
        //{
        //yield return Morning();
        //yield return Dinner();
        //yield return Night();
        //            DayCount++;
        //}
        yield return null;
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

    [SerializeField]
    private Hero hero;
    //아침 시작
    //장비를 챙겨서 모험 시작
    public IEnumerator Morning()
    {
        OnMorningStart?.Invoke();
        //장비 장착하러 가기
        yield return InventoryManager.Instance.RunEquipPhase();
        OnEquipAction?.Invoke();

        //모험하러 가기
        OnAdventure?.Invoke();
        yield return AdventureManager.Instance.StartAdventure(hero);
        void action()
        {
            StartCoroutine(Dinner());
            //거래를 멈추는 무언가가 있어야 하는데
            ShopManager.Instance.TerminationTrade();
            AdventureManager.Instance.OnAdventureEnded -= action;
        }

        AdventureManager.Instance.OnAdventureEnded += action; 

        //모험 시작하면 거래 시작
        OnTrade?.Invoke();
        //yield return new WaitForSeconds(1);
        ShopManager.Instance.StartEncounter(ShopManager.Instance.CreateVisitor());
    }

    //모험 끝남
    public IEnumerator Dinner()
    {
        //모험 끝나면 거래가 끝나야 함
        //용사가 복귀해야 함

        //용사가 복귀하면 아이템을 상자애 넣고 밥을 먹어야 함

        yield return null;
    }

    //잠을 자고 밤 페이지(현재 아무것도 없음) 시작
    public IEnumerator Night()
    {
        OnNight?.Invoke();

        yield return null;
    }
    //아침 시작
}
