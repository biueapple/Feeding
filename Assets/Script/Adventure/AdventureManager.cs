using System;
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

    //여기에 모험이 끝나면 일어날 다음 단계를 넣어서 동작시키기
    public event Action OnAdventureEnded;
    //모험에 이벤트를 넣어서 콜백을 해줘야 할지 모르겠네 그럼 args도 만들어야 하나

    private Coroutine coroutine = null;
    public void StartAdventure(Hero hero)
    {
        coroutine ??= StartCoroutine(RunAdventure(hero));
    }

    IEnumerator RunAdventure(Hero hero)
    {
        float heroAtkTimer = 0;
        float enemyAtkTimer = 0;

        Enemy enemy = CreateEnemy();

        BuffAdministrator heroBuffAdministrator = hero.GetComponent<BuffAdministrator>();
        BuffAdministrator enemyBuffAdministrator = enemy.GetComponent<BuffAdministrator>();
        Inventory inventory = hero.GetComponent<Inventory>();
            
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
                    inventory.InventoryInterface.InsertItem(i);
                }
                Debug.Log($"적이 드랍한 골드 양 {gold}");
                inventory.InventoryInterface.EarnGold(gold);

                Destroy(enemy.gameObject);
                enemy = CreateEnemy();
            }
        }

        OnAdventureEnded?.Invoke();
        coroutine = null;
    }

    private Enemy CreateEnemy()
    {
        return Instantiate(enemies[0]);
    }
}

