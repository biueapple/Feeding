using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : Unit
{
    private Equipment equipment;
    [SerializeField]
    private float speed = 300;
    [SerializeField]
    private Vector3[] chestRoute;
    [SerializeField]
    private Vector3[] outRoute;
    [SerializeField]
    private Vector3[] inDungeon;

    private void Awake()
    {
        equipment = GetComponent<Equipment>();    
    }

    //침대에서 일어나기
    public IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("기상");
    }

    //상자까지 이동하기
    public IEnumerator MoveToChest()
    {
        if (chestRoute == null || chestRoute.Length == 0) yield break;

        transform.localPosition = chestRoute[0];
        int index = 1;
        while (index < chestRoute.Length)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, chestRoute[index], Time.deltaTime * speed);
            yield return null;

            if (Vector2.Distance( transform.localPosition, chestRoute[index]) < 0.1f)
            {
                index++;
            }
        }
        yield return new WaitForSeconds(1);
        Debug.Log("상자까지 이동");
    }

    //장비를 장착하기
    public IEnumerator ToEquip()
    {
        foreach (var slot in InventoryManager.Instance.HeroCloseInterface.Itemslots)
        {
            if (slot.Item == null) continue;
            equipment.TryEquip(slot.Item, out _);
            //사운드가 나면 좋을듯
            Debug.Log(slot.ItemName + " 장착");
            yield return new WaitForSeconds(0.1f);

        }
        yield return new WaitForSeconds(1);
        Debug.Log("옷입기 완료");
    }

    //집밖으로 나가기
    public IEnumerator OutHome()
    {
        if (outRoute == null || outRoute.Length == 0) yield break;

        transform.localPosition = outRoute[0];
        int index = 1;
        while (index < outRoute.Length)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, outRoute[index], Time.deltaTime * speed);
            yield return null;

            if (Vector2.Distance(transform.localPosition, outRoute[index]) < 0.1f)
            {
                index++;
            }
        }
        yield return new WaitForSeconds(1);
        Debug.Log("집밖으로 나가기");
    }

    //던전에 들어가기
    public IEnumerator InDungeon()
    {
        if (inDungeon == null || inDungeon.Length == 0) yield break;

        transform.localPosition = inDungeon[0];
        int index = 1;
        while (index < inDungeon.Length)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, inDungeon[index], Time.deltaTime * speed);
            yield return null;

            if (Vector2.Distance(transform.localPosition, inDungeon[index]) < 0.1f)
            {
                index++;
            }
        }
        yield return new WaitForSeconds(1);
        Debug.Log("던전에 들어가기");
    }
}
