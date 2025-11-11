using UnityEngine;

public class TradeDeactiveStep : ITradePipe
{
    private readonly VisitorText texting;
    private readonly ITradePipe next;

    public TradeDeactiveStep(VisitorText texting, ITradePipe next)
    {
        this.texting = texting;
        this.next = next;
    }

    public void Play()
    {
        texting.OnClick += Next;
        ShopManager.Instance.TradeButton.gameObject.SetActive(false);
        ShopManager.Instance.Numeric.gameObject.SetActive(false);
        UIManager.Instance.TradeSlot.gameObject.SetActive(false);
    }

    public void Next()
    {
        texting.OnClick -= Next;
        next?.Play();
    }
}
