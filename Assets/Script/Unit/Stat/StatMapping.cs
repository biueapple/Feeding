using UnityEngine;

[System.Serializable]
public class StatMapping
{
    [SerializeField]
    private DerivationStat derivation;
    public DerivationStat Derivation => derivation;
    [SerializeField]
    private float figure;
    public float Figure => figure;
}
