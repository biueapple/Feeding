using UnityEngine;

public sealed class TradeService 
{
    //내가 판매 test 대신 인벤토리가 들어가야 함
    public void CommitSale(Test player, Item item, int price)
    {
        //골드와 아이템 변동
        player.pay += price;
    }

    //내가 구매
    public void CommitPurchase(Test player, Item item, int price)
    {
        player.pay -= price;
    }
}
