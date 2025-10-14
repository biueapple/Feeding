using UnityEngine;

public class RecoveryPacket
{
    public float OriginalValue { get; private set; }
    public float Value { get; set; }
    public string Sources { get; private set; }

    public RecoveryPacket(string sources, float value)
    {
        OriginalValue = value;
        Value = value;
        Sources = sources;
    }
}
