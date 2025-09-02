using UnityEngine;

//�� Ŭ������ ���������� �𸣰����� Condition üũ�� Ŭ������ �����ϱ�
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
