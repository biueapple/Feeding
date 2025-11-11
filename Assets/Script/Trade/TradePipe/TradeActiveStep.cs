using UnityEngine;

//버튼이나 이런 거래에 필요한 오브젝트들을 활성화 함
public class TradeActiveStep : ITradePipe
{
    private readonly ITradePipe next;

    public TradeActiveStep(ITradePipe next)
    {
        this.next = next;
    }

    public void Play()
    {
        ShopManager.Instance.TradeButton.gameObject.SetActive(true);
        ShopManager.Instance.Numeric.gameObject.SetActive(true);
        if (ShopManager.Instance.TradeService.TradeType == TradeType.Buy)
            UIManager.Instance.TradeSlot.gameObject.SetActive(true);
        Next();
    }

    public void Next()
    {
        next?.Play();
    }
}
