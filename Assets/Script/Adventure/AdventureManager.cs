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
            //���ݼӵ��� ������ �޴� Ÿ�̸�
            heroAtkTimer += dalta * hero.StatValue(DerivationKind.AS);
            enemyAtkTimer += dalta * enemy.StatValue(DerivationKind.AS);

            //���� ��ȸ
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

            //�������� �ð� ����
            if(heroBuffAdministrator != null)
                heroBuffAdministrator.TimeTick(dalta);
            if(enemyBuffAdministrator != null)
                enemyBuffAdministrator.TimeTick(dalta);

            //���� ���
            if(enemy.CurrentHP <= 0)
            {
                enemy.LootEntry.Loot(out List<Item> list, out int gold);
                //������ ȹ���̶� ��� ȹ��
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
