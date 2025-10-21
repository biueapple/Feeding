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

    //ħ�뿡�� �Ͼ��
    public IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("���");
    }

    //���ڱ��� �̵��ϱ�
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
        Debug.Log("���ڱ��� �̵�");
    }

    //��� �����ϱ�
    public IEnumerator ToEquip()
    {
        foreach (var slot in InventoryManager.Instance.HeroCloseInterface.Itemslots)
        {
            if (slot.Item == null) continue;
            equipment.TryEquip(slot.Item, out _);
            //���尡 ���� ������
            Debug.Log(slot.ItemName + " ����");
            yield return new WaitForSeconds(0.1f);

        }
        yield return new WaitForSeconds(1);
        Debug.Log("���Ա� �Ϸ�");
    }

    //�������� ������
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
        Debug.Log("�������� ������");
    }

    //������ ����
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
        Debug.Log("������ ����");
    }
}
