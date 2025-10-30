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

    //���⿡ ������ ������ �Ͼ ���� �ܰ踦 �־ ���۽�Ű��
    public event Action OnAdventureEnded;
    //������ �� ������ ȣ��� 1�ʸ��� ȣ�� �̺�Ʈ
    public event Action OnFrame;
    public event Action OnSecond;
    //���迡 �̺�Ʈ�� �־ �ݹ��� ����� ���� �𸣰ڳ� �׷� args�� ������ �ϳ�

    private Coroutine coroutine = null;
    public IEnumerator StartAdventure(Hero hero)
    {
        yield return new WaitForSeconds(1);
        coroutine ??= StartCoroutine(RunAdventure(hero));
    }

    public IEnumerator RunAdventure(Hero hero)
    {
        float heroAtkTimer = 0;
        float enemyAtkTimer = 0;
        float secondTimer = 0f;

        Enemy enemy = CreateEnemy();

        BuffAdministrator heroBuffAdministrator = hero.GetComponent<BuffAdministrator>();
        BuffAdministrator enemyBuffAdministrator = enemy.GetComponent<BuffAdministrator>();
        Inventory inventory = hero.GetComponent<Inventory>();
            
        while (hero.CurrentHP > 0)
        {
            float delta = Time.deltaTime;
            //���ݼӵ��� ������ �޴� Ÿ�̸�
            heroAtkTimer += delta * hero.StatValue(DerivationKind.AS);
            enemyAtkTimer += delta * enemy.StatValue(DerivationKind.AS);

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

            //���� ���
            if (enemy.CurrentHP <= 0)
            {
                enemy.LootEntry.Loot(out List<Item> list, out int gold);
                //������ ȹ���̶� ��� ȹ��
                foreach(var i in list)
                {
                    inventory.InventoryInterface.InsertItem(i);
                }
                Debug.Log($"���� ����� ��� �� {gold}");
                InventoryManager.Instance.EarnGold(gold);

                Destroy(enemy.gameObject);
                enemy = CreateEnemy();
            }

            yield return null;

            OnFrame?.Invoke();

            secondTimer += delta; // �� �� �����Ӹ��� ����
            // 1�� ��� �� OnSecond ȣ��
            if (secondTimer >= 1f)
            {
                secondTimer -= 1f; // �� ���� �ð� ���� (��Ȯ�� �ֱ� ����)
                OnSecond?.Invoke();
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

