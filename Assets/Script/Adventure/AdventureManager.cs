using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureManager : MonoBehaviour
{
    public static AdventureManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public List<Enemy> enemies = new();
    public float BasedTimer = 3;

    public void StartAdventure(Hero hero)
    {
        StartCoroutine(RunAdventure(hero));
    }

    IEnumerator RunAdventure(Hero hero)
    {
        float heroAtkTimer = 0;
        float enemyAtkTimer = 0;

        Enemy enemy = CreateEnemy();

        BuffAdministrator heroBuffAdministrator = hero.GetComponent<BuffAdministrator>();
        BuffAdministrator enemyBuffAdministrator = enemy.GetComponent<BuffAdministrator>();
        
            
        while (hero.CurrentHP > 0)
        {
            yield return null;
            float dalta = Time.deltaTime;
            //공격속도에 영향을 받는 타이머
            heroAtkTimer += dalta * hero.StatValue(DerivationKind.AS);
            enemyAtkTimer += dalta * enemy.StatValue(DerivationKind.AS);

            //공격 기회
            if(heroAtkTimer >= BasedTimer)
            {
                hero.BasicAttack(enemy);
                heroAtkTimer = 0;
            }
            if(enemyAtkTimer >= BasedTimer)
            {
                enemy.BasicAttack(hero);
                enemyAtkTimer = 0;
            }

            //버프들의 시간 지남
            if(heroBuffAdministrator != null)
                heroBuffAdministrator.TimeTick(dalta);
            if(enemyBuffAdministrator != null)
                enemyBuffAdministrator.TimeTick(dalta);

            //몬스터 사망
            if(enemy.CurrentHP <= 0)
            {
                enemy.LootEntry.Loot(out List<Item> list, out int gold);
                //아이템 획득이랑 골드 획득
                foreach(var i in list)
                {
                    InventoryManager.Instance.InsertItem(i, out _);
                }
                InventoryManager.Instance.EarnGold(gold);

                Destroy(enemy.gameObject);
                enemy = CreateEnemy();
            }
        }
    }

    private Enemy CreateEnemy()
    {
        return Instantiate(enemies[0]);
    }
}
