using System.Collections.Generic;
using UnityEngine;

public class RecoveryEventArgs
{
    public Unit Healer { get; private set; }
    public Unit Recipient { get; private set; }
    public List<RecoveryPacket> Recovery { get; set; }

    public RecoveryEventArgs(Unit healer, Unit recipient)
    {
        Healer = healer;
        Recipient = recipient;
        Recovery = new();
    }
}
