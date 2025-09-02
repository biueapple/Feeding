using UnityEngine;

public enum ModifierScope
{
    Item, 
    VIsitor,
    Shop,
    Global,

}


public abstract class PriceModifierSO : ScriptableObject, IPriceModifier
{
    [SerializeField]
    private ModifierScope scope;
    public ModifierScope Scope => scope;
    [SerializeField]
    private string stackingKey = "";    //��������� �ߺ� ���
    [SerializeField]
    private string displayName = "Modifier";

    public string DisplayName => displayName;

    public string StackingKey => stackingKey;

    public abstract (PriceOP op, float value) Evaluate(in PriceContext ctx);
    public abstract bool IsApplicable(in PriceContext ctx);
}