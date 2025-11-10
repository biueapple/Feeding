using System.Linq;
using UnityEngine;

public static class DialogueService
{
    public static string Pick(VisitorDialoguePack pack, DialogueEvent evt, DialogueContext ctx)
    {
        var candidates = pack.rules
            .Where(r => r.evt == evt)
            .Where(r => !r.tradeType.use || r.tradeType.value == ctx.tradeType)
            .Where(r => !r.match.use || r.match.value == ctx.match)
            .Where(r => !r.result.use || r.result.value == ctx.result)
            .Where(r => ctx.attempt >= r.attemptRange.x && ctx.attempt <= r.attemptRange.y)
            .OrderByDescending(r => r.priority)
            .ToList();

        Debug.Log(candidates.Count);
        if (candidates.Count == 0) { return string.Empty; }

        int topPriority = candidates[0].priority;
        var top = candidates.Where(c => c.priority == topPriority).ToList();
        var rule = top[Random.Range(0, top.Count)];

        if (rule.lines == null || rule.lines.Count == 0) return string.Empty;
        string line = rule.lines[Random.Range(0, rule.lines.Count)];
        return Fill(line, ctx);
    }

    private static string Fill(string line, DialogueContext c)
    {
        int diff = c.offer - (c.basePrice + c.spread);
        line = line.Replace("{visitor}", c.visitorSO.VisitorName);
        line = line.Replace("{item}", c.item != null ? c.item.ItemName : "ªÛ«∞");
        line = line.Replace("{category}", c.category.ToString());
        line = line.Replace("{price}", c.offer.ToString());
        line = line.Replace("{base}", c.basePrice.ToString());
        line = line.Replace("{spread}", c.spread.ToString());
        line = line.Replace("{gen}", c.generosity.ToString());
        line = line.Replace("{attempt}", (c.attempt + 1).ToString());
        line = line.Replace("{max}", c.maxRound.ToString());
        line = line.Replace("{diff}", diff.ToString());
        line = line.Replace("{type}", c.tradeType.ToString());
        return line;
    }
}
