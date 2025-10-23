using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Attribute/FoodAttribute")]
public class FoodAttribute : ItemAttribute
{
    [SerializeField]
    private float healing;  //ȸ���Ǵ� ��

    [SerializeField]
    private Buff buff;

    public void Apply(Hero hero)
    {
        RecoveryEventArgs args = new(this, hero);
        args.Recovery.Add(new RecoveryPacket(this, healing));
        hero.Healing(args);
        if(hero.TryGetComponent<BuffAdministrator>(out var administrator))
        {
            administrator.ApplyBuff(this, buff);
        }
    }
}
