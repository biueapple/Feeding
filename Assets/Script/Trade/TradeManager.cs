using UnityEngine;

public class TradeManager : MonoBehaviour
{
    public static TradeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }

    //PriceModifierHub ������ ������ �����Ű�� Ŭ����


}
