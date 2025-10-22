using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hero : Unit
{
    [SerializeField]
    private float speed = 300;
    [SerializeField]
    private Vector3[] chestRoute;
    [SerializeField]
    private Vector3[] outRoute;
    [SerializeField]
    private Vector3[] inDungeon;
    [SerializeField]
    private Vector3[] tableRoute;


    //ħ�뿡�� �Ͼ��
    public IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("���");
    }

    //���ڱ��� �̵��ϱ�
    public IEnumerator MoveToChest()
    {

        yield return Move(chestRoute);
        //yield return new WaitForSeconds(1);
        Debug.Log("���ڱ��� �̵�");
    }

    //�������� ������
    public IEnumerator OutHome()
    {
        yield return Move(outRoute);
        //yield return new WaitForSeconds(1);
        Debug.Log("�������� ������");
    }

    //������ ����
    public IEnumerator InDungeon()
    {
        yield return Move(inDungeon);
        //yield return new WaitForSeconds(1);
        Debug.Log("������ ����");
    }

    public IEnumerator OutDungeon()
    {
        Vector3[] outDungeon = inDungeon.Reverse().ToArray();
        yield return Move(outDungeon);
        //yield return new WaitForSeconds(1);
        Debug.Log("�������� ������");
    }

    public IEnumerator InHome()
    {
        Vector3[] inHome = outRoute.Reverse().ToArray();
        yield return Move(inHome);
        //yield return new WaitForSeconds(1);
        Debug.Log("������ ��ȯ");
    }

    public IEnumerator MoveToTable()
    {
        yield return Move(tableRoute);
        //yield return new WaitForSeconds(1);
        Debug.Log("��Ź���� �̵�");
    }

    public IEnumerator MoveToBad()
    {
        List<Vector3> list = new()
        {
            transform.localPosition,
            chestRoute[0]
        };
        yield return Move(list.ToArray());
        //yield return new WaitForSeconds(1);
        Debug.Log("ħ��� �̵�");
    }

    private IEnumerator Move(Vector3[] route)
    {
        if (route == null || route.Length == 0) yield break;

        transform.localPosition = route[0];
        int index = 1;
        
        while(index < route.Length)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, route[index], Time.deltaTime * speed);
            yield return null;

            if (Vector2.Distance(transform.localPosition, route[index]) < 0.1f)
            {
                index++;
            }
        }
    }
}
