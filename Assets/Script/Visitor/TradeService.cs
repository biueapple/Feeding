using UnityEngine;

public sealed class TradeService 
{
    //���� �Ǹ� test ��� �κ��丮�� ���� ��
    public void CommitSale(Item item, int price)
    {
        //���� ������ ����
        InventoryManager.Instance.PlayerChest.EarnGold(price);
        //InventoryManager.Instance.PlayerChest.InsertItem(item);    
    }

    //���� ���� (������ �ű�°��� shopManager�� �ϳ�)
    public void CommitPurchase(int price)
    {
        if(InventoryManager.Instance.PlayerChest.TryEarnGold(-price))
        {
            //InventoryManager.Instance.PlayerChest.
        }
    }
}
