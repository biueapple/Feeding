using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public enum TradeFlowState
{
    Idle,
    EncounterIntro,   // 손님 도착~둘러보기 안내
    Trading,          // 플레이어 가격 입력/흥정
    PostResult,       // 거래 결과 멘트 출력 단계
    Goodbye           // 작별 멘트 단계
}

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
    private Button tradeButton;
    public Button TradeButton => tradeButton;
    [SerializeField]
    private NumericInputField numeric;
    public NumericInputField Numeric => numeric;


    private TradeService tradeService;
    public TradeService TradeService { get => tradeService; private set { tradeService = value; } }

    //플레이어가 아이템을 올려놓는 슬롯
    private ItemSlot tradeSlot;
    public ItemSlot TradeSlot => tradeSlot;

    public event Action<TradeService> OnCreateTradeSession;
    public event Action OnEndSession;


    private ITradePipe now = null;
    private ArriveStep arriveStep;
    private BrowseHintStep browseHintStep;
    private TradeActiveStep tradeActiveStep;
    private SubmitStep submitStep;
    private ResolveTradeStep resolveTradeStep;
    private TradeDeactiveStep resultDialogueStep;
    private GoodbyeStep goodbyeStep;

    private void Start()
    {
        UIManager.Instance.TradeSlot.Init(tradeSlot);

        goodbyeStep = new(visitorText, null);
        resultDialogueStep = new(visitorText, goodbyeStep);
        resolveTradeStep = new(resultDialogueStep);
        submitStep = new(resolveTradeStep);
        tradeActiveStep = new(submitStep);
        browseHintStep = new(visitorText, tradeActiveStep);
        arriveStep = new(visitorText, browseHintStep);

        resolveTradeStep.Prv = submitStep;

        now = arriveStep;
    }

    public void StartShop()
    {
        VisitorManager.Instance.VisitorManagerStart();
    }

    //거래 중간에 중단
    public void TerminationTrade()
    {
        VisitorManager.Instance.VisitorManagerEnd();

        if (InventoryManager.Instance.PlayerChest.InsertItem(tradeSlot.Item))
            Debug.Log("trade -> playerchest");
        else
            Debug.Log("trade -> null");

        tradeSlot.Insert(null);
        visitorText.Texting("");
        numeric.gameObject.SetActive(false);

        now = null;
        tradeService = null;
    }


    //만남
    public void StartEncounter(VisitorSO visitor)
    {
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
        }

        OnCreateTradeSession?.Invoke(tradeService);

        now.Play();
    }



    //3 커밋
    public void TryCommit(int acceptedPrice)
    {
        if (tradeService.Commit(acceptedPrice))
        {
            Debug.Log("거래 성공");
        }
        else
        {
            Debug.Log("커밋 실패");
        }

        tradeSlot.Insert(null);
    }


    // 실제로 세션 완전히 마무리하는 함수
    public void FinishSession()
    {
        tradeService = null;

        tradeSlot.Insert(null);
        visitorText.Texting("");

        OnEndSession?.Invoke(); // VisitorManager에서 여기 받아서 다음 StartEncounter 호출
    }







    public string Dialogue(DialogueEvent evt, TradeResult result, int offer)
    {
        if (tradeService == null)
        {
            return string.Empty;
        }

        bool hasCurrent = tradeService.Haggle != null;

        var v = tradeService.Visitor;
        var req = tradeService.Request;
        var it = tradeService.Item != null ? tradeService.Item : null;
        var dlg = v.DialoguePack;

        int generosity = hasCurrent && v != null
            ? Mathf.RoundToInt(v.Generosity * tradeService.Haggle.BaseQuote)
            : 0;

        var ctx = new DialogueContext
        {
            visitorSO = v,
            tradeType = tradeService.TradeType,
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
            return line;
        }
        return string.Empty;
    }

    public void ShowDialogue(DialogueEvent evt, TradeResult result, int offer)
    {
        if (tradeService == null)
        {
            visitorText.Texting("");
            return;
        }

        bool hasCurrent = tradeService.Haggle != null;

        var v = tradeService.Visitor;
        var req = tradeService.Request;
        var it = tradeService.Item != null ? tradeService.Item : null;
        var dlg = v.DialoguePack;

        int generosity = hasCurrent && v != null
            ? Mathf.RoundToInt(v.Generosity * tradeService.Haggle.BaseQuote)
            : 0;

        var ctx = new DialogueContext
        {
            visitorSO = v,
            tradeType = tradeService.TradeType,
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
            visitorText.Texting(line + "\n");
            Debug.Log(evt + " " + line);
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



[Serializable]
public class DialogueCommand
{
    public DialogueEvent Event;
    public TradeResult Result;
    public int Offer;
    public TradeType TradeType;
}