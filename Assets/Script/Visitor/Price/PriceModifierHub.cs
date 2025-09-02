using System.Collections.Generic;
using UnityEngine;

public sealed class PriceModifierHub : MonoBehaviour
{
    [SerializeField]
    private List<PriceModifierSO> activeEvents = new();        //���� �ε� ���
    public IEnumerable<IPriceModifier> ActivePriceEvents => activeEvents;

    public void Activate(PriceModifierSO e)
    {
        if (!activeEvents.Contains(e)) activeEvents.Add(e);
    }
    public void Deactivate(PriceModifierSO e)
    {
        activeEvents.Remove(e);
    }

    //���� ���� ����
    public void Tick()
    {
        activeEvents.RemoveAll(e => e is PriceModifierSO ev && !ev.IsApplicable(new PriceContext(null, null, TradeType.Buy)));
    }

    public IEnumerable<IPriceModifier> GetApplicable(PriceContext ctx)
    {
        foreach (var m in activeEvents)
            if (m.IsApplicable(ctx))
                yield return m;
    }
}
