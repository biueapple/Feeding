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
    //���� ���� interface ����� buffStatModifier �����
    //statModifier�� equipment �� buffAdministrator �� ������


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
        Debug.Log($"name {item.ItemName} ����");
    }

    public void OnUnequipment(Item item)
    {
        Debug.Log($"name {item.ItemName} ����");
    }

    public void OnAfterDamage(AttackEventArgs args)
    {
        Debug.Log($"{args.Attacker} �� {args.Defender} ���� ");
        foreach(var d in args.Damages)
        {
            Debug.Log($"type: {d.type}, ����� {d.OriginalValue}");
            Debug.Log($"���� {d.Value}");
            Debug.Log(d.Sources);
        }
    }
}
