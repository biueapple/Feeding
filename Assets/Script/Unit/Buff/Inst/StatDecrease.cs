using System.Collections.Generic;
using UnityEngine;

////Buff_StatModifier�� ���� ������� �ʴ� �ڵ�
//[CreateAssetMenu(menuName = "RPG/Debuff/StatDecrease")]
//public class StatDecrease : Buff
//{
//    [SerializeField]
//    private DerivationKind type;

//    [SerializeField, Header("- ���� �Է�")]
//    private float value;

//    public override string Description => "{name}\n{type}�� {value} ��ŭ ����մϴ�.\n�����ð� {duration}";
//    public override string BuildDescription(BuffInstance inst)
//    {
//        string s = base.BuildDescription(inst);
//        s = s.Replace("{type}", type.ToString());
//        s = s.Replace("{value}", value.ToString());
//        return s;
//    }

//    public override void Apply(BuffAdministrator administrator, BuffInstance inst)
//    {
//        if (administrator.Owner == null)
//            return;
//        administrator.Owner.AddStatModifier(new StatModifier(type, -value, BuffName), BuffID);
//    }

//    public override void Remove(BuffAdministrator administrator, List<BuffInstance> list)
//    {
//        if (administrator.Owner == null) return;
//        administrator.Owner.RemoveStatModifier(BuffID);
//    }

//    public override BuffInstance CreateInstance(BuffAdministrator administrator)
//    {
//        return new BuffInstance(this, administrator);
//    }

//    public override void Reapply(BuffAdministrator administrator, List<BuffInstance> list)
//    {

//    }
//}
