using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Unit unit;
    public Equipment equipment;
    public ItemCollector itemCollector;
    public Item item;
    public EquipmentPart part;
    public Enemy enemy;
    public BuffAdministrator buffAdministrator;
    public StatRise calm;


    public Visitor visitor;
    public int pay;

    public DroughtEvent drought;
    //버프 설명 interface 만들고 buffStatModifier 만들기
    //statModifier를 equipment 와 buffAdministrator 에 나누기


    private void Start()
    {
        //unit.OnBeforeAttack += OnAttackEvent;
        equipment.OnEquipped += OnEquipment;
        equipment.OnUnequipped += OnUnequipment;

        unit.OnAfterTakeDamage += OnAfterDamage;
        enemy.OnAfterTakeDamage += OnAfterDamage;
    }

    public ShopManager shopManager;
    public Item need;
    public Item target;
    public void OnTradeStartButton()
    {
        shopManager.StartTrade(new ItemTradeRequest(TradeType.Buy, visitor, itemCollector.Items), target, visitor, new HaggleSession());
    }

    public void OnTradeOffer()
    {
        shopManager.OnPlyaerOffer(pay);
    }

    public void OnEquip()
    {
        equipment.TryEquip(item, out _);
        
    }

    public void OnUnEquip()
    {
        equipment.Unequip(part);

    }
    public void OnButton()
    {
        unit.Attack(enemy, null, false);
        //enemy.Attack(unit, null, false);
    }

    public void OnAttackEvent(AttackEventArgs args)
    {
        foreach(var d in args.Damages)
        {
            Debug.Log($"type: {d.type}, value: {d.Value}");
        }
    }

    public void OnEquipment(Item item)
    {
        Debug.Log($"name {item.ItemName} 장착");
    }

    public void OnUnequipment(Item item)
    {
        Debug.Log($"name {item.ItemName} 해제");
    }

    public void OnAfterDamage(AttackEventArgs args)
    {
        Debug.Log($"{args.Attacker} 가 {args.Defender} 에게 ");
        foreach(var d in args.Damages)
        {
            Debug.Log($"type: {d.type}, 대미지 {d.OriginalValue}");
            Debug.Log($"피해 {d.Value}");
            Debug.Log(d.Sources);
        }
    }
}
