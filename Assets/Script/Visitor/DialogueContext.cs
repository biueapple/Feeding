using UnityEngine;

public class DialogueContext
{
    public Visitor visitor;
    public TradeType tradeType;
    public Item item;
    public int offer;               //haggle 의 pay
    public int basePrice;           //trade의 final 후의 값
    public int spread;
    public int generosity;
    public int attempt;             //현재 라운드
    public int maxRound;
    public CategoryMatch match;
    public TradeResult result;
}
