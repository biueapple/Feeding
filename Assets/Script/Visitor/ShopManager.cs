using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField]
    private PriceModifierHub hub;
    [SerializeField]
    private TradeService tradeService;

    private PricingService pricing;
    private TradeSession current;

    private void Awake()
    {
        pricing = new PricingService(hub);
        tradeService = new();
    }

    public PriceQuote Quote(Item item, Visitor visitor, TradeType tradeType) => pricing.GetQuote(item, visitor, tradeType);

    //1 거래시작
    public void StartTrade(TradeRequest req, Item selected, Visitor visitor, HaggleSession haggle)
    {
        var margin = req.Margin(selected);
        var quote = Quote(selected, visitor, req.TradeType);
        haggle.Start(quotedPrice: quote.FinalPrice, spread: CalcSpread(selected, visitor, margin), maxRound: visitor.MaxRounds, concedePerRound: visitor.ConcedePerRound);

        current = new (req, selected, visitor, quote, haggle);

        //ui
        Debug.Log("내가 준 물건 " + current.Item);
        Debug.Log("상대가 원하는 물건 " + current.Request.Summary);
        foreach (var m in quote.Steps)
        {
            Debug.Log("적용된 모드 " + m.Name);
            Debug.Log("변동치 " + m.Value);
            Debug.Log("적용 후 " + m.SubtotalAfter);
        }
        Debug.Log("내가 준 물건의 기본 가격 " + quote.BasePrice);
        Debug.Log("내가 준 물건의 현재 가격 " + quote.FinalPrice);
    }

    //2 라운드 입력
    public void OnPlyaerOffer(int offer)
    {
        if (current == null) return;

        var (result, counter) = current.Haggle.EvaluateOffer(current.Request.TradeType, offer);

        switch(result)
        {
            case HaggleResult.Accept:
                TryCommit(offer); 
                break;
            case HaggleResult.Counter:
                //ui
                Debug.Log("흥정 " + counter);
                break;
            case HaggleResult.Reject:
                EndSession(false, "흥정실패");
                break;
        }
    }

    //3 커밋
    public Test test;
    public void TryCommit(int acceptedPrice)
    {
        var r = current.Request;
        var item = current.Item;

        //내가 사기
        if(r.TradeType == TradeType.Sell)
        {
            if(test.item != null && test.pay < acceptedPrice)
            {
                Debug.Log("인벤창이나 돈이 모자름");
                return;
            }
            tradeService.CommitPurchase(test, item, acceptedPrice);
        }
        //내가 팔기
        else
        {
            if(test.item == null)
            {
                Debug.Log("아이템이 없음");
                return;
            }
            tradeService.CommitSale(test, item, acceptedPrice);
        }

        //ui
        EndSession(true);
    }

    private void EndSession(bool success, string reason = "")
    {
        //ui
        Debug.Log(success + " " + reason);
        current = null;
    }

    private int CalcSpread(Item item, Visitor visitor, int margin)
    {
        return Mathf.Max(1, Mathf.RoundToInt(item.Price + margin + visitor.Generosity));
    }
}


public sealed class TradeSession
{
    public readonly TradeRequest Request;
    public readonly Item Item;
    public readonly Visitor Visitor;
    public readonly PriceQuote PriceQuote;
    public readonly HaggleSession Haggle;
    public TradeSession(TradeRequest req, Item item, Visitor visitor, PriceQuote quote, HaggleSession haggle)
    {
        Request = req;
        Item = item;
        Visitor = visitor;
        PriceQuote = quote;
        Haggle = haggle;
    }
}