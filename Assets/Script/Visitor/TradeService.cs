using UnityEngine;

public sealed class TradeService 
{
    //내가 판매 test 대신 인벤토리가 들어가야 함
    public void CommitSale(Item item, int price)
    {
        //골드와 아이템 변동
        InventoryManager.Instance.PlayerChest.EarnGold(price);
        //InventoryManager.Instance.PlayerChest.InsertItem(item);    
    }

    //내가 구매 (아이템 옮기는것은 shopManager가 하네)
    public void CommitPurchase(int price)
    {
        if(InventoryManager.Instance.PlayerChest.TryEarnGold(-price))
        {
            //InventoryManager.Instance.PlayerChest.
        }
    }
}
