using UnityEngine;

public class DialogueContext
{
    public Visitor visitor;
    public TradeType tradeType;
    public Item item;
    public int offer;               //haggle �� pay
    public int basePrice;           //trade�� final ���� ��
    public int spread;
    public int generosity;
    public int attempt;             //���� ����
    public int maxRound;
    public CategoryMatch match;
    public TradeResult result;
}
