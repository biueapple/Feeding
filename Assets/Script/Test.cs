using System;
using UnityEngine;
using UnityEngine.InputSystem;

//세트 효과나 다른 효과들 보여줄 내용 만들기
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

    public void OnAfterAdventure()
    {
        Inventory inventory = hero.GetComponent<Inventory>();
        InventoryManager.Instance.HeroChest.InsertInventory(inventory.InventoryInterface);
    }

    public void OnTakeDamageAfter(AttackEventArgs args)
    {
        Debug.Log($"{args.Attacker} 가 {args.Defender} 에게 ");
        foreach(var pack in args.Damages)
        {
            Debug.Log($"{pack.Sources} 에게서 나온 {pack.type} 타입의 {pack.Value} 대미지");
        }
        Debug.Log($"-----------------------------------------------------------------------");
    }

    public void OnRecoveryAfter(RecoveryEventArgs args)
    {
        Debug.Log($"{args.Healer} 가 {args.Recipient} 에게 ");
        foreach (var pack in args.Recovery)
        {
            Debug.Log($"{pack.Sources} 에게서 {pack.Value} 회복");
        }
        Debug.Log($"-----------------------------------------------------------------------");
    }
}
