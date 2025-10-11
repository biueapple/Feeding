using UnityEngine;
using UnityEngine.UI;

public class HPbar : MonoBehaviour
{
    [SerializeField]
    private Unit unit;
    //�̰� �̹����� �ٲ�� image�� 
    [SerializeField]
    private Image bar;

    private void Awake()
    {
        unit.OnChangeHP += Unit_OnChangeHP;
    }

    private void Unit_OnChangeHP(Unit unit)
    {
        bar.fillAmount = unit.CurrentHP / unit.StatValue(DerivationKind.HP);
    }
}
