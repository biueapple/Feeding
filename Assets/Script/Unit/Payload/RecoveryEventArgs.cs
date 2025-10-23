using System.Collections.Generic;
using UnityEngine;

public class RecoveryEventArgs
{
    public object Healer { get; private set; }
    public Unit Recipient { get; private set; }
    public List<RecoveryPacket> Recovery { get; set; }

    public RecoveryEventArgs(object healer, Unit recipient)
    {
        Healer = healer;
        Recipient = recipient;
        Recovery = new();
    }
}
