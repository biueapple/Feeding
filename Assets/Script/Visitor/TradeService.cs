using UnityEngine;

public sealed class TradeService 
{
    //���� �Ǹ� test ��� �κ��丮�� ���� ��
    public void CommitSale(ItemSlot slot, int price)
    {
        //���� ������ ����
        InventoryManager.Instance.PlayerChest.EarnGold(price);
        slot.Insert(null);
        //InventoryManager.Instance.PlayerChest.InsertItem(item);    
    }

    //���� ����
    public void CommitPurchase(int price)
    {
        if(InventoryManager.Instance.PlayerChest.TryEarnGold(-price))
        {
            //InventoryManager.Instance.PlayerChest.
        }
    }
}
