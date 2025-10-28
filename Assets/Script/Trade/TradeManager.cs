using UnityEngine;

public class TradeManager : MonoBehaviour
{
    public static TradeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }

    //PriceModifierHub 가격의 변동을 적용시키는 클래스


}
