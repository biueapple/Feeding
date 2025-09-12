using UnityEngine;

public class StatModifier   
{
    //여기 필드들을 interface 로 만들고 interface를 장비나 버프가 상속받고 내용을 채우는 느낌으로 해도 좋을듯
    public DerivationKind Kind { get; private set; }    
    public float Value { get; private set; }
    public string Name { get; private set; }

    public StatModifier(DerivationKind kind, float value, string name)
    {
        Kind = kind;
        Value = value;
        Name = name;
    }
}
