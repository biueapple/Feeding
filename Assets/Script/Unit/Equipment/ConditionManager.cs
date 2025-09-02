using UnityEngine;

//이 클래스를 유지할지는 모르겠지만 Condition 체크를 클래스로 수정하기
public class ConditionManager : MonoBehaviour
{
    public static ConditionManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    public void AddLevelLimit(Equipment equipment, Unit unit)
    {
        equipment.AddCondition(new LevelEquipCondition(unit));
    }

    public void AddStrengthLimit(Equipment equipment, int limit)
    {
        
    }
}
