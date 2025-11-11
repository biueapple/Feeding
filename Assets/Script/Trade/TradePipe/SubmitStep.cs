using UnityEngine;

//Trade 버튼을 누르면 다음 단계가 진행되도록 
public class SubmitStep : ITradePipe
{
    private readonly ITradePipe next;

    public SubmitStep(ITradePipe next)
    {
        this.next = next;
    }

    public void Play()
    {
        ShopManager.Instance.TradeButton.onClick.AddListener(Next);
    }

    public void Next()
    {
        if (ShopManager.Instance.TradeService.TradeType == TradeType.Buy && 
            ShopManager.Instance.TradeSlot.Item == null)
            return;

        ShopManager.Instance.TradeButton.onClick.RemoveListener(Next);
        next?.Play();
    }
}
