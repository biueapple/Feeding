using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    //모험의 매 프레임 호출과 1초마다 호출 이벤트
    public event Action OnFrame;
    public event Action OnSecond;
    //모험에 이벤트를 넣어서 콜백을 해줘야 할지 모르겠네 그럼 args도 만들어야 하나

    [SerializeField]
    private Transform parent;
    [SerializeField]
    private Transform createPosition;

    [SerializeField]
    private UIMovingTile uiMovingTile;

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
        yield return MoveEnemy(enemy, new Vector2(350, -400));

        BuffAdministrator heroBuffAdministrator = hero.GetComponent<BuffAdministrator>();
        BuffAdministrator enemyBuffAdministrator = enemy.GetComponent<BuffAdministrator>();
        Inventory inventory = hero.GetComponent<Inventory>();
            
        while (hero.CurrentHP > 0)
        {
            float delta = Time.deltaTime;
            //공격속도에 영향을 받는 타이머
            heroAtkTimer += delta * hero.StatValue(DerivationKind.AS);
            enemyAtkTimer += delta * enemy.StatValue(DerivationKind.AS);

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

            //몬스터 사망
            if (enemy.CurrentHP <= 0)
            {
                enemy.LootEntry.Loot(out List<Item> list, out int gold);
                //아이템 획득이랑 골드 획득
                foreach(var i in list)
                {
                    inventory.InventoryInterface.InsertItem(i);
                }
                Debug.Log($"적이 드랍한 골드 양 {gold}");
                inventory.Gold += gold;

                Destroy(enemy.gameObject);

                enemy = CreateEnemy();
                yield return NextEnemy(enemy);
            }

            yield return null;

            OnFrame?.Invoke();

            secondTimer += delta; // ← 매 프레임마다 누적
            // 1초 경과 시 OnSecond 호출
            if (secondTimer >= 1f)
            {
                secondTimer -= 1f; // ← 남은 시간 유지 (정확한 주기 유지)
                OnSecond?.Invoke();
            }
        }

        StartCoroutine(BackEnemy(enemy));
        OnAdventureEnded?.Invoke();
        coroutine = null;
    }

    private IEnumerator NextEnemy(Enemy enemy)
    {
        GameManager.Instance.Hero.SetAnimationBool("Move", true);
        uiMovingTile.StartMovingTile();

        yield return MoveEnemy(enemy, new Vector2(350, -400));
        
        GameManager.Instance.Hero.SetAnimationBool("Move", false);
        uiMovingTile.StopMovingTile();
    }

    private IEnumerator BackEnemy(Enemy enemy)
    {
        enemy.transform.localScale *= new Vector2(-1, 1);
        GameManager.Instance.Hero.SetAnimationBool("Move", true);

        yield return MoveEnemy(enemy, new Vector2(1200, -400));

        GameManager.Instance.Hero.SetAnimationBool("Move", false);
        Destroy(enemy.gameObject);
    }

    private IEnumerator MoveEnemy(Enemy enemy, Vector2 target)
    {
        Vector2 position = target;//new Vector2(350, -400);
        while (Vector2.Distance(enemy.transform.localPosition, position) > 0.1f)
        {
            enemy.transform.localPosition = Vector2.MoveTowards(enemy.transform.localPosition, position, 300 * Time.deltaTime);
            yield return null;
        }
    }

    //350 -400
    private Enemy CreateEnemy()
    {
        var e = WorldContext.Instance.CurrentVillage.Enemies;
        Enemy enemy = Instantiate(e[UnityEngine.Random.Range(0, e.Count)]);

        enemy.transform.SetParent(parent);
        enemy.transform.position = createPosition.position; 
        
        return enemy;
    }
}

