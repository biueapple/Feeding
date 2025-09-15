using System;
using UnityEngine;
using UnityEngine.InputSystem;

//이제 UI 만들자
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
    public Item input;
    public Buff buff;
    private void Start()
    {
        //unit.OnBeforeAttack += OnAttackEvent;
        equipment.OnEquipped += OnEquipment;
        equipment.OnUnequipped += OnUnequipment;

        hero.OnAfterAttack += OnAfterDamage;
        enemy.OnAfterAttack += OnAfterDamage;
        buffAdministrator.AddDayBuff(buff);

        Inventory inventory = hero.GetComponent<Inventory>();
        inventory.InventoryInterface.OnAfterGold += OnAfterGold;
        for(int i = 0; i < inventory.InventoryInterface.Itemslots.Length; i++)
        {
            inventory.InventoryInterface.Itemslots[i].OnBeforeChange += OnBeforeInven;
            inventory.InventoryInterface.Itemslots[i].OnAfterChange += OnAfterInven;
        }

        AdventureManager.Instance.OnAdventureEnded += OnAfterAdventure;
        InventoryManager.Instance.PlayerChest.EarnGold(150);

        InventoryManager.Instance.HeroChest.InsertItem(input);
        InventoryManager.Instance.PlayerChest.OnAfterGold += OnAfterGold;
    }


    public ShopManager shopManager;

    public void OnEncounterStartButton()
    {
        shopManager.StartEncounter(visitor, itemCollector.Items);
    }

    public void OnTradeStartButton()
    {
        shopManager.StartTrade(pay);
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
        Debug.Log($"창고로 아이템 옮기기 끝");

        Debug.Log("용사 가방 아이템들");
        inventory.InventoryInterface.Print();
        Debug.Log("용사 상자 아이템들");
        InventoryManager.Instance.HeroChest.Print();
    }
}
