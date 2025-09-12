using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Hero hero;
    public Equipment equipment;
    public ItemCollector itemCollector;
    public Item item;
    public EquipmentPart part;
    public Enemy enemy;
    public BuffAdministrator buffAdministrator;
    public StatRise calm;


    public Visitor visitor;
    public int pay;

    public Buff buff;
    private void Start()
    {
        //unit.OnBeforeAttack += OnAttackEvent;
        equipment.OnEquipped += OnEquipment;
        equipment.OnUnequipped += OnUnequipment;

        hero.OnAfterAttack += OnAfterDamage;
        enemy.OnAfterAttack += OnAfterDamage;
        buffAdministrator.AddDayBuff(buff);

        InventoryManager.Instance.OnAfterGold += OnAfterGold;
        for(int i = 0; i < InventoryManager.Instance.Inven.Length; i++)
        {
            InventoryManager.Instance.Inven[i].OnBeforeChange += OnBeforeInven;
            InventoryManager.Instance.Inven[i].OnAfterChange += OnAfterInven;
        }
    }

    public ShopManager shopManager;
    public Item target;

    public void OnEncounterStartButton()
    {
        shopManager.StartEncounter(visitor, itemCollector.Items);
    }

    public void OnTradeStartButton()
    {
        shopManager.StartTrade(target, pay);
    }

    public void OnTradeOffer()
    {
        //shopManager.OnPlyaerOffer(pay);
    }

    public void OnEquip()
    {
        equipment.TryEquip(item, out _);
        
    }

    public void OnUnEquip()
    {
        equipment.Unequip(part);

    }

    public AdventureManager adventureManager;
    public void OnStartAdventure()
    {
        adventureManager.StartAdventure(hero);
        //hero.BasicAttack(enemy);
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
            Debug.Log($"피해 {d.Value} 남은 체력 {args.Defender.CurrentHP}");
            Debug.Log(d.Sources);
        }
    }

    public void OnBeforeInven(ItemSlot item)
    {
        Debug.Log($"바뀌기 전의 아이템 {item.ItemName}");
    }

    public void OnAfterInven(ItemSlot item)
    {
        Debug.Log($"바뀐 후의 아이템 {item.ItemName}");
    }

    public void OnAfterGold(int gold)
    {
        Debug.Log($"현재 골드량 {gold}");
    }
}
