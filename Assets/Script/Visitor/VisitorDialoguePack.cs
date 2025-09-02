using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Visitor/Dialogue")]
public class VisitorDialoguePack : ScriptableObject
{
    public List<DialogueRule> rules; 
}

[System.Serializable]
public class DialogueRule
{
    public DialogueEvent evt;           //visitor�� ����

    public RuleFilter<TradeType> tradeType;
    public RuleFilter<CategoryMatch> match;
    public RuleFilter<TradeResult> result;
    public Vector2Int attemptRange = new(0, 99);    //���� ���� ����

    [TextArea(2, 4)]
    public List<string> lines;                      //�ĺ� ���� ��

    public int priority = 0;                    //�켱���� (������ �켱)
}

[System.Serializable]
public class RuleFilter<T>
{
    public bool use;
    public T value;
}