using System.Collections.Generic;
using UnityEngine;

public enum DialogueEvent
{
    Arrive,         //����
    BrowseHint,     //��ȣ/��ȣ ��Ʈ
    OfferAsked,     //�÷��̾� ù ����
    OfferEvaluated, //Accept ����
    Concede,        //�湮�� �纸 �ǻ�
    DealSuccess,    //�ŷ� ����
    DealMaintain,   //������
    DealFail,       //�ŷ� ����
    MaxRoundsReached,   //���� �ִ�ġ ����
    Goodbye,        //�ۺ�
}

public enum CategoryMatch
{
    Preferred,
    Disliked,
    Neutral,

}

[CreateAssetMenu(menuName = "Visitor/Visitor")]
public class VisitorSO : ScriptableObject
{
    [SerializeField, HideInInspector]
    private string visitorID;                                       //id ������
    public string VisitorID => visitorID;

    [SerializeField]
    private string visitorName;                                     //�̸�
    public string VisitorName => visitorName;

    [SerializeField]
    private Sprite portrait;                                        //�̹���
    public Sprite Portrait => portrait;

    [SerializeField]
    private List<ItemCategory> preferred;                           //��ȣ ī�װ�
    public IReadOnlyList<ItemCategory> Preferred => preferred;

    [SerializeField]
    private List<ItemCategory> disliked;                            //��ȣ ī�װ�
    public IReadOnlyList<ItemCategory> Disliked => disliked;

    [SerializeField]
    private VisitorDialoguePack dialoguePack;
    public VisitorDialoguePack DialoguePack => dialoguePack;

    [Header("Price Policy")]                                                            //����
    [Range(0, 0.5f), SerializeField] private float baseMargin = 0.08f;                  //�⺻ ��� ����
    public float BaseMargin => baseMargin;
    [Range(-0.5f, 0.5f), SerializeField] private float preferMarginBonus = 0.02f;       //��ȣ ���� ����
    public float PreferMarginBonus => preferMarginBonus;
    [Range(-0.5f, 0.5f), SerializeField] private float dislikeMarginPenalty = -0.03f;   //��ȣ ���� ���
    public float DislikeMarginPenalty => dislikeMarginPenalty;

    [Header("Haggle, Refusal")]                                                         //����
    [Range(0, 1f), SerializeField]
    private float generosity = 0.1f;                                                    //������ �󸶳� �������� �ŷ��� �̾���� (������ �������)
    public float Generosity => generosity;

    [Range(1, 5), SerializeField] private int maxRounds = 2;                            //���� Ƚ��
    public int MaxRounds => maxRounds;
    [Range(-1, 1f), SerializeField] private float concedePerRound = 0.25f;               //����� �纸��
    public float ConcedePerRound => concedePerRound;
}
