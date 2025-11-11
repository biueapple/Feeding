using UnityEngine;

//현재 입력한 가격과 아이템을 참고하여 거래가 성공인지 실패인지 판단
public class ResolveTradeStep : ITradePipe
{
    private readonly ITradePipe next;
    public ITradePipe Prv { get; set; }

    public ResolveTradeStep(ITradePipe next)
    {
        this.next = next;
    }

    public void Play()
    {
        var shop = ShopManager.Instance;
        int offer = shop.Numeric.GetNumericValue();
        var result = shop.TradeService.Trade(offer);

        switch (result)
        {
            case HaggleResult.Accept:
                shop.TryCommit(offer); // 여기서 대사/후처리 step으로 이어지게 설계
                shop.ShowDialogue(DialogueEvent.DealSuccess, TradeResult.Success, offer);
                break;

            case HaggleResult.Counter:
                // 여기서 다시 SubmitStep으로 돌아가고 싶으면 next를 SubmitStep 체인으로 연결
                shop.ShowDialogue(DialogueEvent.DealMaintain, TradeResult.Maintenance, offer);
                Prv?.Play();
                return;

            case HaggleResult.Reject:
                // 실패 대사 + 세션 종료 파이프 연결
                if (shop.TradeService.Haggle.Attempt >= shop.TradeService.Haggle.MaxRound)
                    shop.ShowDialogue(DialogueEvent.MaxRoundsReached, TradeResult.Failed, offer);
                else
                    shop.ShowDialogue(DialogueEvent.DealFail, TradeResult.Failed, offer);
                break;
        }

        next?.Play();
    }
}
