using System.Collections.Generic;
using UnityEngine;

public class AttackEventArgs
{
    public Unit Attacker { get; private set; }
    public Unit Defender { get; private set; }
    public List<DamagePacket> Damages { get; set; }
    public bool IsExtraAttack { get; private set; }
    public bool IsCritical { get; private set; }

    public AttackEventArgs(Unit attacker, Unit defender, bool isExtraAttack)
    {
        Attacker = attacker;
        Defender = defender;
        Damages = new();
        IsExtraAttack = isExtraAttack;
        IsCritical = attacker.Critical;
    }
}
