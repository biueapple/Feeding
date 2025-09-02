using UnityEngine;

public class DamagePacket
{
    public float OriginalValue { get; private set; }
    public float Value { get; set; }
    public DamageType type;
    public string Sources { get; private set; }

    public DamagePacket(DamageType type, string sources, float value)
    {
        OriginalValue = value;
        Value = value;
        this.type = type;
        Sources = sources;
    }
}
