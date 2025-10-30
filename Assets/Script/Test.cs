using System;
using UnityEngine;
using UnityEngine.InputSystem;

//��Ʈ ȿ���� �ٸ� ȿ���� ������ ���� �����
public class Test : MonoBehaviour
{
    public Hero hero;
    public Equipment equipment;
    public Item item;
    public EquipmentPart part;
    public Enemy enemy;
    public BuffAdministrator buffAdministrator;
    public Buff_StatModifier calm;


    public VisitorSO visitor;
    public int pay;
    public Item input;
    public Item food;
    public Item potato;

    public Buff buff;
    public Buff bl;
    public Buff po;
    public Buff dot;
    public Buff ele;
    private void Start()
    {
        hero.OnTakeDamageAfter += OnTakeDamageAfter;

        equipment.OnEquipped += OnEquipment;
        equipment.OnUnequipped += OnUnequipment;

        hero.OnAttackAfter += OnAfterDamage;
        enemy.OnAttackAfter += OnAfterDamage;
        buffAdministrator.ApplyBuff(hero, buff);
        //buffAdministrator.ApplyBuff(hero, bl);
        //buffAdministrator.ApplyBuff(hero, bl);
        //buffAdministrator.ApplyBuff(hero, po);
        //buffAdministrator.ApplyBuff(hero, dot);
        //buffAdministrator.ApplyBuff(hero, dot);
        //buffAdministrator.ApplyBuff(hero, ele);
        //buffAdministrator.ApplyBuff(hero, ele);

        //AttackEventArgs args = new (null, hero, true);
        //args.Damages.Add(new DamagePacket(DamageType.True, hero, 10));
        //hero.TakeDamage(args);

        //RecoveryEventArgs re = new(hero, hero);
        //re.Recovery.Add(new RecoveryPacket("hero", 10));
        //hero.Healing(re);


        Inventory inventory = hero.GetComponent<Inventory>();
        InventoryManager.Instance.OnAfterGold += OnAfterGold;
        for(int i = 0; i < inventory.InventoryInterface.Itemslots.Length; i++)
        {
            inventory.InventoryInterface.Itemslots[i].OnBeforeChange += OnBeforeInven;
            inventory.InventoryInterface.Itemslots[i].OnAfterChange += OnAfterInven;
        }

        AdventureManager.Instance.OnAdventureEnded += OnAfterAdventure;
        InventoryManager.Instance.EarnGold(150);

        InventoryManager.Instance.HeroChest.InsertItem(input);
        InventoryManager.Instance.PlayerChest.InsertItem(food);
        InventoryManager.Instance.PlayerChest.InsertItem(potato);
        InventoryManager.Instance.PlayerChest.InsertItem(potato);
        InventoryManager.Instance.PlayerChest.InsertItem(potato);
        InventoryManager.Instance.OnAfterGold += OnAfterGold;

    }
    

    public ShopManager shopManager;

    public void OnEncounterStartButton()
    {
        shopManager.StartEncounter(visitor);
    }

    public void OnEquip()
    {
        equipment.TryEquip(item, out _);
        
    }

    public void OnUnEquip()
    {
        equipment.Unequip(part);

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
            Debug.Log($"���� {d.Value} ���� ü�� {args.Defender.CurrentHP}");
            Debug.Log(d.Sources);
        }
    }

    public void OnBeforeInven(ItemSlot item)
    {
        Debug.Log($"�ٲ�� ���� ������ {item.ItemName}");
    }

    public void OnAfterInven(ItemSlot item)
    {
        Debug.Log($"�ٲ� ���� ������ {item.ItemName}");
    }

    public void OnAfterGold(int gold)
    {
        Debug.Log($"���� ��差 {gold}");
    }

    public void OnAfterAdventure()
    {
        Inventory inventory = hero.GetComponent<Inventory>();
        InventoryManager.Instance.HeroChest.InsertInventory(inventory.InventoryInterface);
    }

    public void OnTakeDamageAfter(AttackEventArgs args)
    {
        Debug.Log($"{args.Attacker} �� {args.Defender} ���� ");
        foreach(var pack in args.Damages)
        {
            Debug.Log($"{pack.Sources} ���Լ� ���� {pack.type} Ÿ���� {pack.Value} �����");
        }
        Debug.Log($"-----------------------------------------------------------------------");
    }

    public void OnRecoveryAfter(RecoveryEventArgs args)
    {
        Debug.Log($"{args.Healer} �� {args.Recipient} ���� ");
        foreach (var pack in args.Recovery)
        {
            Debug.Log($"{pack.Sources} ���Լ� {pack.Value} ȸ��");
        }
        Debug.Log($"-----------------------------------------------------------------------");
    }
}
