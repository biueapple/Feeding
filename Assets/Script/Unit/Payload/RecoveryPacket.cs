using UnityEngine;

public class RecoveryPacket
{
    public float OriginalValue { get; private set; }
    public float Value { get; set; }
    public object Sources { get; private set; }

    public RecoveryPacket(object sources, float value)
    {
        OriginalValue = value;
        Value = value;
        Sources = sources;
    }
}
