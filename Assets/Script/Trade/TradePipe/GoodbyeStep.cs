using UnityEngine;

public class GoodbyeStep : ITradePipe
{
    private readonly VisitorText texting;
    private readonly ITradePipe next;

    public GoodbyeStep(VisitorText texting, ITradePipe next)
    {
        this.texting = texting;
        this.next = next;
    }

    public void Play()
    {
        texting.Texting(ShopManager.Instance.Dialogue
            (DialogueEvent.Goodbye, TradeResult.None, 0));
        texting.OnClick += Next;
    }

    public void Next()
    {
        texting.OnClick -= Next;
        ShopManager.Instance.FinishSession();
        next?.Play();
    }
}
