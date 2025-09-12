using UnityEngine;

public class StatModifier   
{
    //���� �ʵ���� interface �� ����� interface�� ��� ������ ��ӹް� ������ ä��� �������� �ص� ������
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
