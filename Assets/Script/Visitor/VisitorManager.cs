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
        lineUP.OnCreateEvent += CreateVisitor;
        lineUP.OnActive += ToActive;
        lineUP.OnLineFirst += OnTradeStart;
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

    private void Start()
    {
        ShopManager.Instance.OnEndSession += Instance_OnEndSession;
    }

    private void Instance_OnEndSession()
    {
        lineUP.Delete();
    }

    private void OnTradeStart(Visitor visitor)
    {
        ShopManager.Instance.StartEncounter(visitor.SO);
    }

    private Visitor CreateVisitor()
    {
        Visitor v = Instantiate(prefab);
        v.SO = allVisitor[Random.Range(0, allVisitor.Count)];
        return v;
    }

    private void ToActive(Visitor visitor)
    {
        visitor.SO = allVisitor[Random.Range(0, allVisitor.Count)];
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            lineUP.Create();

        if (Keyboard.current.tabKey.wasPressedThisFrame)
            lineUP.Delete();
    }
}
