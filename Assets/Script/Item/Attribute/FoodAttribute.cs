using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Attribute/FoodAttribute")]
public class FoodAttribute : ItemAttribute
{
    [SerializeField]
    private float healing;  //회복되는 양

    [SerializeField]
    private Buff buff;

    public void Apply(BuffAdministrator administrator)
    {
        administrator.ApplyBuff(this, buff);
    }
}
