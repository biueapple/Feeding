using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        tradeSlot = new();
    }

    [SerializeField]
    private PriceModifierHub hub;
    [SerializeField]
    private ItemCollector itemCollector;
    [SerializeField]
    private VisitorText visitorText;
    [SerializeField]
    private NumericInputField numeric;


    private TradeService tradeService;

    public event Action<TradeService> OnCreateTradeSession;
    public event Action OnEndSession;

    private void Start()
    {
        UIManager.Instance.TradeSlot.Init(tradeSlot);
    }

    //플레이어가 아이템을 올려놓는 슬롯
    private ItemSlot tradeSlot;

    public void StartShop()
    {
        VisitorManager.Instance.VisitorManagerStart();
    }

    //거래 중간에 중단
    public void TerminationTrade()
    {
        VisitorManager.Instance.VisitorManagerEnd();

        //currentRequest = null;
        //currentVisitorSO = null;
        if(InventoryManager.Instance.PlayerChest.InsertItem(tradeSlot.Item))
        {
            Debug.Log("trade -> playerchest");
        }
        else
        {
            Debug.Log("trade -> null");
        }
        tradeSlot.Insert(null);
        visitorText.Texting(""); 
        numeric.gameObject.SetActive(false);
    }

    public IEnumerator Progress()
    {
        bool next = false;

        void Next() { next = true; }
        visitorText.OnClick += Next;

        //인사
        Emit(DialogueEvent.Arrive, TradeResult.None, 0, tradeService.TradeType);
        yield return new WaitUntil(() => next);
        next = false;

        //힌트
        Emit(DialogueEvent.BrowseHint, TradeResult.None, 0, tradeService.TradeType);
        yield return new WaitUntil(() => next);
        next = false;

        //반응
    }

    //만남
    public void StartEncounter(VisitorSO visitor)
    {
        numeric.gameObject.SetActive(true);

        if (InventoryManager.Instance.PlayerChest.Count() < 3 || UnityEngine.Random.value > 0.5f)
        {
            tradeService = new SellTrade(hub);
            tradeService.Encounter(visitor);
        }
        else
        {
            Item GetItem()
            {
                return tradeSlot.Item;
            }
            tradeService = new BuyTrade(hub, GetItem);
            tradeService.Encounter(visitor);
            UIManager.Instance.TradeSlot.gameObject.SetActive(true);
        }

        OnCreateTradeSession?.Invoke(tradeService);
        StartCoroutine(Progress());
    }

    //1 거래시작
    public void StartTrade()
    {
        if(tradeService == null) { Debug.Log("만남이 없는데 어떻게 거래를 해"); return; }

        visitorText.Texting("");
        int offer = numeric.GetNumericValue();

        var result = tradeService.Trade(offer);

        switch (result)
        {
            case HaggleResult.Accept:
                TryCommit(offer);
                Debug.Log("수락");
                break;
            case HaggleResult.Counter:
                //ui
                Emit(DialogueEvent.DealMaintain, TradeResult.Maintenance, offer, tradeService.TradeType);
                Debug.Log("재흥정 ");
                break;
            case HaggleResult.Reject:
                if (tradeService.Haggle.Attempt >= tradeService.Haggle.MaxRound)
                    Emit(DialogueEvent.MaxRoundsReached, TradeResult.Failed, offer, tradeService.TradeType);
                else
                    Emit(DialogueEvent.DealFail, TradeResult.Failed, offer, tradeService.TradeType);
                EndSession(false, "흥정실패");
                break;
        }

    }

    //3 커밋
    public void TryCommit(int acceptedPrice)
    {
        if (tradeService.Commit(acceptedPrice))
        {
            //ui
            Emit(DialogueEvent.DealSuccess, TradeResult.Success, acceptedPrice, tradeService.TradeType);
            EndSession(true, "거래 성공");
        }
        else
        {
            Emit(DialogueEvent.DealFail, TradeResult.Failed, acceptedPrice, tradeService.TradeType);
        }
        tradeSlot.Insert(null);
    }

    private void EndSession(bool success, string reason = "")
    {
        //ui
        visitorText.Texting("");
        Emit(DialogueEvent.Goodbye, success ? TradeResult.Success : TradeResult.Failed, 0, tradeService.TradeType);
        UIManager.Instance.TradeSlot.gameObject.SetActive(false);
        OnEndSession?.Invoke();
        numeric.gameObject.SetActive(false);
    }


















    private void Emit(DialogueEvent evt, TradeResult result, int offer, TradeType tradeType)
    {
        bool hasCurrent = tradeService.Haggle != null;

        var v = tradeService.Visitor;
        var req = tradeService.Request;
        var it = tradeService.Item != null ? tradeService.Item : null;
        var dlg = v.DialoguePack;

        int generosity = hasCurrent && v != null ? Mathf.RoundToInt(v.Generosity * tradeService.Haggle.BaseQuote) : 0;


        var ctx = new DialogueContext
        {
            visitorSO = v,
            tradeType = tradeType,
            item = it,
            offer = offer,
            basePrice = hasCurrent ? tradeService.Haggle.BaseQuote : 0,
            spread = hasCurrent ? tradeService.Haggle.Spread : 0,
            generosity = generosity,
            attempt = hasCurrent ? tradeService.Haggle.Attempt : 0,
            maxRound = hasCurrent ? tradeService.Haggle.MaxRound : 0,
            result = result,
            match = req.ComputeMatch(it)
        };

        string line = DialogueService.Pick(dlg, evt, ctx);
        if (!string.IsNullOrEmpty(line))
        {
            //ui에 띄우기
            visitorText.Texting(line + "\n");
            Debug.Log(evt + line);
        }
    }
}


public sealed class TradeSession
{
    public TradeRequest Request { get; set; }
    public VisitorSO VisitorSO { get; set; }
    public Item Item { get; set; }
    public ItemCategory Category { get; set; }
    public PriceQuote PriceQuote { get; set; }
    public HaggleSession Haggle { get; set; }
}