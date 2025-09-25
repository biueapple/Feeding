using System;
using UnityEngine;

//o
[CreateAssetMenu(menuName = "RPG/Debuff/DotType_Bleeding")]
public class Bleeding : Dot
{
    public override string Description => "���ݽø��� ���ø�ŭ ������� �����ϴ�.";
    public override string BuildDescription(BuffInstance inst)
    {
        string s = base.BuildDescription(inst);
        s = s.Replace("{type}", DamageType.Physical.ToString());
        return s;
    }

    public override void Apply(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator.Owner == null)
            return;

        void action(AttackEventArgs args)
        {
            Debug.Log($"bleeding���� ���� ���� {inst.Duration}");
            administrator.Owner.CurrentHP -= inst.Duration;
            Debug.Log(inst.Duration / 2);
            if (inst.Tick(inst.Duration / 2))
            {
                Debug.Log("���� ����");
                administrator.RemoveBuff(this);
                administrator.Owner.OnAfterAttack -= action;
            }
        }

        administrator.Owner.OnAfterAttack += action;
    }

    public override void Reapply(BuffAdministrator administrator, BuffInstance inst)
    {

    }

    public override void Remove(BuffAdministrator administrator, BuffInstance inst)
    {
        if (administrator.Owner == null) return;
        administrator.Owner.RemoveStatModifier(BuffID);
    }

    public override BuffInstance CreateInstance(BuffAdministrator administrator)
    {
        return new BuffInstance(this, administrator);
    }

    
}
