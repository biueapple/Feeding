using System;
using UnityEngine;

//인사 클릭시 다음으로 넘어감
public class ArriveStep : ITradePipe
{
    private readonly VisitorText texting;
    private readonly ITradePipe next;

    public ArriveStep(VisitorText texting, ITradePipe next)
    {
        this.texting = texting;
        this.next = next;
    }

    public void Play()
    {
        texting.Texting(ShopManager.Instance.Dialogue
            (DialogueEvent.Arrive, TradeResult.None, 0));
        texting.OnClick += Next;
    }

    public void Next()
    {
        texting.OnClick -= Next;
        next?.Play();
    }
}
