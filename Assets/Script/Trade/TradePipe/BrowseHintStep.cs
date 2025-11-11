using UnityEngine;

//대사 출력 거래에 힌트를 줌
public class BrowseHintStep : ITradePipe
{
    private readonly VisitorText texting;
    private readonly ITradePipe next;

    public BrowseHintStep(VisitorText texting, ITradePipe next)
    {
        this.texting = texting;
        this.next = next;
    }

    public void Play()
    {
        texting.Texting(ShopManager.Instance.Dialogue
            (DialogueEvent.BrowseHint, TradeResult.None, 0));
        texting.OnClick += Next;
    }

    public void Next()
    {
        texting.OnClick -= Next;
        next?.Play();
    }
}
