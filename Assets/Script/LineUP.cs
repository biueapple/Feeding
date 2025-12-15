using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�׳� �ټ���� �Ϸ��� ���� Ŭ����
public class LineUP<T> where T : Visitor
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Vector2 createPosition;
    private readonly Vector2 destination;
    private readonly float interval;
    private readonly List<T> line = new();

    private readonly List<T> active = new();
    private readonly Queue<T> deactive = new();

    public event Func<T> OnCreateEvent;
    public event Action<T> OnActive;
    public event Action<T> OnLineFirst;

    // 다음 대기열 위치를 추적하기 위한 변수
    private Vector2 nextEndPosition;

    public LineUP(T prefab, Transform parent, Vector2 createPosition, Vector2 destination, float interval, float speed)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.createPosition = createPosition;
        this.destination = destination;
        this.interval = interval;
    }

    public void Create()
    {
        T obj = CreateObject();

        // Visitor에게 이동 명령을 내립니다.
        // 이동이 끝나면(onComplete) 라인 리스트에 추가하고 이벤트를 호출합니다.
        obj.Move(nextEndPosition, () =>
        {
            line.Add(obj);
            if (line.Count == 1)
            {
                OnLineFirst?.Invoke(line[0]);
            }
        });

        // 다음 사람이 설 위치 갱신
        nextEndPosition.x += interval;
    }

    Coroutine coroutine;
    public void Delete()
    {
        if (line.Count == 0) return;

        // 1. 맨 앞 손님 제거
        T firstObj = line[0];
        line.RemoveAt(0);
        DeleteObject(firstObj);

        // 2. 나머지 손님들을 한 칸씩 앞으로 당기기 (Push)
        UpdateLinePositions();
    }

    public void AllDelete()
    {
        while (line.Count > 0)
        {
            DeleteObject(line[0]);
            line.RemoveAt(0);
        }
        // 위치 초기화
        nextEndPosition = destination;
    }

    // 기존의 Coroutine Push 대신, 각 Visitor에게 새로운 위치로 가라고 명령합니다.
    private void UpdateLinePositions()
    {
        Vector2 currentTarget = destination;

        for (int i = 0; i < line.Count; i++)
        {
            T obj = line[i];

            // i == 0인 경우(이제 맨 앞이 된 사람) 도착 시 OnLineFirst 호출
            if (i == 0)
            {
                obj.Move(currentTarget, () => OnLineFirst?.Invoke(obj));
            }
            else
            {
                obj.Move(currentTarget);
            }

            currentTarget.x += interval;
        }

        // 다음에 들어올 사람의 위치를 현재 줄의 맨 끝 다음으로 갱신
        nextEndPosition = currentTarget;
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
            // OnCreateEvent가 Visitor 타입을 반환한다고 가정
            obj = OnCreateEvent?.Invoke();

            if (obj == null)
            {
                obj = GameObject.Instantiate(prefab);
                obj.Init(); // Visitor 초기화 호출
            }

            obj.transform.SetParent(parent);
            obj.transform.localScale = Vector3.one; // 스케일 안전장치
        }

        // Visitor는 localPosition을 기준으로 움직이므로 로컬 좌표 설정
        obj.transform.localPosition = createPosition;
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
