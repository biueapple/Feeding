using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField]
    private Hero hero;
    public Hero Hero => hero;
    
    [SerializeField]
    private ItemCollector itemCollector;
    public IReadOnlyList<Item> ItemCollector => itemCollector.Items;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public Coroutine RunCoroutine(IEnumerator enumerator)
    {
        return StartCoroutine(enumerator);
    }

    public void StopCoroutineExtern(Coroutine enumerator)
    {
        StopCoroutine(enumerator);
    }
}
