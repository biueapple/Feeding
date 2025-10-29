using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//그냥 줄세우기 하려고 만든 클래스
public class LineUP<T> where T : MonoBehaviour
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Vector2 createPosition;
    private readonly Vector2 destination;
    private readonly float interval;
    private readonly float speed;

    private readonly List<T> line = new();

    private readonly List<T> active = new();
    private readonly Queue<T> deactive = new();

    public event Func<T> OnCreateEvent;
    public event Action<T> OnActive;
    public event Action<T> OnLineFirst;


    public LineUP(T prefab, Transform parent, Vector2 createPosition, Vector2 destination, float interval, float speed)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.createPosition = createPosition;
        this.destination = destination;
        this.interval = interval;
        this.speed = speed;
        end = destination;
    }

    public void Create()
    {
        T obj = CreateObject();
        GameManager.Instance.StartCoroutine(Move(obj));
    }

    Coroutine coroutine;
    public void Delete()
    {
        if (coroutine != null) { GameManager.Instance.StopCoroutineExtern(coroutine); }
        
        coroutine = GameManager.Instance.StartCoroutine(Push());
    }

    private IEnumerator Push()
    {
        if (line.Count == 0) yield break;

        DeleteObject(line[0]);
        line.RemoveAt(0);

        Vector2 dest = destination;
        for(int i = 0; i < line.Count; i++)
        {
            while (Vector2.Distance(line[i].transform.position, dest) > 0.1f)
            {
                line[i].transform.position = Vector2.MoveTowards(line[i].transform.position, dest, Time.deltaTime * speed);

                yield return null;
            }

            if (i == 0)
                OnLineFirst?.Invoke(line[0]);

            dest.x += interval;
        }
        end = dest;
        coroutine = null;
    }

    private Vector2 end;
    private IEnumerator Move(T obj)
    {
        while(Vector2.Distance(obj.transform.position, end) > 0.1f)
        {
            obj.transform.position = Vector2.MoveTowards(obj.transform.position, end, Time.deltaTime * speed);

            yield return null;
        }
        line.Add(obj);
        
        if (line.Count == 1)
            OnLineFirst?.Invoke(line[0]);

        end.x += interval;
    }

    private T CreateObject()
    {
        T obj;
        if (deactive.Count > 0)
        {
            obj = deactive.Dequeue();
            OnActive?.Invoke(obj);
        }
        else
        {
            obj = OnCreateEvent?.Invoke();
            if(obj == null)
                obj = GameObject.Instantiate(prefab);
            obj.transform.SetParent(parent);
        }
            

        obj.transform.position = createPosition;
        obj.gameObject.SetActive(true);
        active.Add(obj);
        return obj;
    }

    private void DeleteObject(T obj)
    {
        active.Remove(obj);
        deactive.Enqueue(obj);
        obj.gameObject.SetActive(false);
    }
}
