using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

//어떤 도트딜을 얼마나 추가할지 없다면 만들기
//이 도트대미지는 stack이 대미지이며 du는 지속시간을 의미
//이미 걸려있는 상황에서 다시 걸리게 되면 du를 초기화 stack을 쌓는 형식과 du를 놔누고 stack을 쌓는 방법
//혹은 그냥 새 dot를 추가해서 2개의 도트 대미지를 가지게 되는 경우 이렇게 있을거 같은데 stack을 쌓는 방법은
//결국 du를 어떻게 할것이냐가 문제
/* gpt가 알려준거
| 정책                        | 설명                                                                     | 언제 쓰기 좋음             |
| --------------------------- | ----------------------------------------------------------------------- | --------------            |
| **A. Stack+Refresh**        | `stacks += x; remaining = baseDuration`                                 | 독/화상(시간 기반)           |
| **B. Stack+Keep**           | `stacks += x; remaining = max(remaining, minRefresh?)`                  | 출혈(행동틱)               |
| **C. Stack+Extend(capped)** | `stacks += x; remaining = min(remaining + extendPerApply, maxDuration)` | 화상/연료 개념              |
| **D. Strongest Wins**       | `stacks,potency,dur = max rule`                                         | 감전(취약)처럼 폭주 방지    |
| **E. Per-Source Merge UI**  | 내부 다중 인스턴스, 표시는 합산                                           | MMO/크레딧 중요한 게임       |
*/
//이런걸 원한게 아니였는데 
//감전은 이미 걸려있는 상태에서 다시 걸면 이미 걸려있는 대미지를 전부 받고 새것이 걸린다던가
//독은 뭔가 느린 이미지가 있는데 대미지보단 스택쪽에서 뭔가가 있는게
//출혈은 반대로 빠르게 스택보단 대미지쪽에
//화상은 그 중간 느낌이 아예 중첩이 아닌 여러개의 화상이 걸릴 수 있게 한다던가
//그렇다면 언제 대미지를 받는지도 생각해야 하는데
//출혈은 공격마다 대미지를 받도록 일단 만들었는데
//독은 매 턴마다
//그럼 감전과 화상은?
//화상은 공격을 받을 때마다로 하자
//감전도 매 턴마다 받도록 하자
//근데 턴이 어딨냐 이 게임은 턴개념이 없는데
//턴대신 1초마다로 해야겠네

//받을때마다 새로 넣어서 여러개가 동시에 존재할 수 있는 도트댐
[CreateAssetMenu(menuName = "RPG/Debuff/DotType_DOT")]
public class Dot : Buff
{
    public override string BuffID => "DOT";
    [SerializeField]
    protected DamageType type;
    [SerializeField]
    protected int stack;
    public virtual int Stack { get => stack; }

    public override string BuildDescription(BuffInstance inst)
    {
        string s = base.BuildDescription(inst);
        s = s.Replace("{stack}", inst.Stacks.ToString());
        s = s.Replace("{type}", type.ToString());
        return s;
    }

    public override void Apply(object caster, BuffAdministrator target, BuffInstance inst)
    {
        if (target == null || target.Owner == null || inst == null)
            return;

        void action()
        {
            float damage = inst.Stacks;
            Debug.Log($"dot로 인한 피해 {damage}");
            AttackEventArgs args = new(caster as Unit, target.Owner, false);
            args.Damages.Add(new DamagePacket(type, "DOT", damage));
            target.Owner.TakeDamage(args);
            if (inst.Tick(1))
            {
                Debug.Log("dot 끝남");
                target.RemoveBuff(inst);
            }
        }

        target.SubscribeOnSecond(inst, action);
    }

    //여긴 list에 여러개일 수 있음
    public override void Reapply(object caster, BuffAdministrator target, List<BuffInstance> list)
    {
        if (target == null || list == null)
            return;

        //Reapply를 호출할때는 CreateInstance 하지 않고 호출하기에 직접 만들어서 넣어주기
        BuffInstance inst = CreateInstance(caster, target);
        target.AddInstance(caster, inst);
    }

    public override void Remove(BuffAdministrator administrator, BuffInstance inst)
    {

    }

    public override BuffInstance CreateInstance(object caster, BuffAdministrator target)
    {
        BuffInstance inst = base.CreateInstance(caster, target);
        
        inst.Duration = Duration;
        inst.AddStack(Stack);
        return inst;
    }
}
