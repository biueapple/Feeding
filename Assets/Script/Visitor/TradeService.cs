using UnityEngine;

public sealed class TradeService 
{
    //���� �Ǹ� test ��� �κ��丮�� ���� ��
    public void CommitSale(Test player, Item item, int price)
    {
        //���� ������ ����
        player.pay += price;
    }

    //���� ����
    public void CommitPurchase(Test player, Item item, int price)
    {
        player.pay -= price;
    }
}
