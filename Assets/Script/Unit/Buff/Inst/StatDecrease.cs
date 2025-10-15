using System.Collections.Generic;
using UnityEngine;

////Buff_StatModifier로 통합 사용하지 않는 코드
//[CreateAssetMenu(menuName = "RPG/Debuff/StatDecrease")]
//public class StatDecrease : Buff
//{
//    [SerializeField]
//    private DerivationKind type;

//    [SerializeField, Header("- 없이 입력")]
//    private float value;

//    public override string Description => "{name}\n{type}이 {value} 만큼 상승합니다.\n남은시간 {duration}";
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
