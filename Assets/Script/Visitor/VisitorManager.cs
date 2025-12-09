using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VisitorManager : MonoBehaviour
{
    public static VisitorManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        lineUP = new(prefab, parent, createPosition.position, tradePosition.position, spacing, speed);

    }

    [SerializeField]
    private List<VisitorSO> allVisitor;
    [SerializeField]
    private Visitor prefab;
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private Transform createPosition;
    [SerializeField]
    private Transform tradePosition;
    [SerializeField]
    private float spacing = 110;
    [SerializeField]
    private float speed = 200;

    LineUP<Visitor> lineUP;

    public void VisitorManagerStart()
    {
        lineUP.OnCreateEvent += CreateVisitor;
        lineUP.OnActive += ToActive;

        //����
        lineUP.OnLineFirst += OnTradeStart;
        ShopManager.Instance.OnEndSession += Instance_OnEndSession;
        ShopManager.Instance.OnEndSession += Add;
        StartCoroutine(StartCreate(3, 1));
    }

    public void VisitorManagerEnd()
    {
        lineUP.OnCreateEvent -= CreateVisitor;
        lineUP.OnActive -= ToActive;

        //����
        lineUP.OnLineFirst -= OnTradeStart;
        ShopManager.Instance.OnEndSession -= Instance_OnEndSession;
        ShopManager.Instance.OnEndSession -= Add;

        lineUP.AllDelete();
    }

    private IEnumerator StartCreate(int count, int t)
    {
        for (int i = 0; i < count; i++)
        {
            lineUP.Create();
            yield return new WaitForSeconds(t);
        }
    }

    public void Add()
    {
        lineUP.Create();
    }

    private void Instance_OnEndSession()
    {
        lineUP.Delete();
    }

    private void OnTradeStart(Visitor visitor)
    {
        ShopManager.Instance.StartEncounter(visitor.SO);
    }


    //����
    private Visitor CreateVisitor()
    {
        Visitor v = Instantiate(prefab);
        v.SO = allVisitor[UnityEngine.Random.Range(0, allVisitor.Count)];
        v.Init();
        v.transform.localScale = new Vector3(-1, 1, 1);
        return v;
    }

    private void ToActive(Visitor visitor)
    {
        visitor.SO = allVisitor[UnityEngine.Random.Range(0, allVisitor.Count)];
    }
}
