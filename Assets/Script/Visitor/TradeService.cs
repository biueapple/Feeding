using UnityEngine;

public sealed class TradeService 
{
    //내가 판매 test 대신 인벤토리가 들어가야 함
    public void CommitSale(ItemSlot slot, int price)
    {
        //골드와 아이템 변동
        InventoryManager.Instance.PlayerChest.EarnGold(price);
        slot.Insert(null);
        //InventoryManager.Instance.PlayerChest.InsertItem(item);    
    }

    //내가 구매
    public void CommitPurchase(int price)
    {
        if(InventoryManager.Instance.PlayerChest.TryEarnGold(-price))
        {
            //InventoryManager.Instance.PlayerChest.
        }
    }
}
