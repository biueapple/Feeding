using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Attribute/FoodAttribute")]
public class FoodAttribute : ItemAttribute
{
    [SerializeField]
    private float healing;  //ȸ���Ǵ� ��

    [SerializeField]
    private Buff buff;

    public void Apply(BuffAdministrator administrator)
    {
        administrator.ApplyBuff(this, buff);
    }
}
