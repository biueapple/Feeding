using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "StatCollector")]
public class StatCollector : ScriptableObject
{
    [SerializeField]
    private List<DerivationStat> derivationStats = new();
    public IReadOnlyList<DerivationStat> DerivationStats => derivationStats;
    public void AddDerivationStat(DerivationStat stat) { if (derivationStats.Contains(stat)) return; derivationStats.Add(stat); }

    [SerializeField]
    private List<FoundationStat> foundationStats = new();
    public IReadOnlyList<FoundationStat> FoundationStats => foundationStats;
    public void AddFoundationStat(FoundationStat stat) { if (foundationStats.Contains(stat)) return; foundationStats.Add(stat); }
}
