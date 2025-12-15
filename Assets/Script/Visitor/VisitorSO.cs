using System.Collections.Generic;
using UnityEngine;

public enum DialogueEvent
{
    Arrive,         //도착
    BrowseHint,     //선호/비선호 힌트
    OfferAsked,     //플레이어 첫 제시
    OfferEvaluated, //Accept 직후
    Concede,        //방문자 양보 의사
    DealSuccess,    //거래 성사
    DealMaintain,   //재흥정
    DealFail,       //거래 실패
    MaxRoundsReached,   //라운드 최대치 넘음
    Goodbye,        //작별
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
    private string visitorID;                                       //id 고정값
    public string VisitorID => visitorID;

    [SerializeField]
    private string visitorNameKey;                                     //이름
    public string VisitorNameKey => visitorNameKey;

    [SerializeField]
    private AnimationClip idleAnimation;                             //대기 애니메이션
    public AnimationClip IdleAnimation => idleAnimation;
    [SerializeField]
    private AnimationClip walkAnimation;                             //걷기 애니메이션
    public AnimationClip WalkAnimation => walkAnimation;

    [SerializeField]
    private List<ItemCategory> preferred;                           //선호 카테고리
    public IReadOnlyList<ItemCategory> Preferred => preferred;

    [SerializeField]
    private List<ItemCategory> disliked;                            //비선호 카테고리
    public IReadOnlyList<ItemCategory> Disliked => disliked;

    [SerializeField]
    private VisitorDialoguePack dialoguePack;
    public VisitorDialoguePack DialoguePack => dialoguePack;

    [Header("Price Policy")]                                                            //가격
    [Range(0, 0.5f), SerializeField] private float baseMargin = 0.08f;                  //기본 허용 범위
    public float BaseMargin => baseMargin;
    [Range(-0.5f, 0.5f), SerializeField] private float preferMarginBonus = 0.02f;       //선호 마진 범위
    public float PreferMarginBonus => preferMarginBonus;
    [Range(-0.5f, 0.5f), SerializeField] private float dislikeMarginPenalty = -0.03f;   //비선호 마진 축소
    public float DislikeMarginPenalty => dislikeMarginPenalty;

    [Header("Haggle, Refusal")]                                                         //흥정
    [Range(0, 1f), SerializeField]
    private float generosity = 0.1f;                                                    //가격이 얼마나 빗나가도 거래를 이어나갈지 (흥정을 계속할지)
    public float Generosity => generosity;

    [Range(1, 5), SerializeField] private int maxRounds = 2;                            //흥정 횟수
    public int MaxRounds => maxRounds;
    [Range(-1, 1f), SerializeField] private float concedePerRound = 0.25f;               //라운드당 양보율
    public float ConcedePerRound => concedePerRound;
}
