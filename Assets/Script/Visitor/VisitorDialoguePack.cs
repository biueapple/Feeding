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
    public DialogueEvent evt;           //visitor에 있음

    public RuleFilter<TradeType> tradeType;
    public RuleFilter<CategoryMatch> match;
    public RuleFilter<TradeResult> result;
    public Vector2Int attemptRange = new(0, 99);    //라운드 필터 범위

    [TextArea(2, 4)]
    public List<string> lines;                      //후보 문장 들

    public int priority = 0;                    //우선순위 (높은거 우선)
}

[System.Serializable]
public class RuleFilter<T>
{
    public bool use;
    public T value;
}