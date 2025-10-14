using System.Collections.Generic;
using UnityEngine;

public class Buffbar : MonoBehaviour
{
    [SerializeField]
    private BuffAdministrator buffAdministrator;

    [SerializeField]
    private BuffIcon prefab;

    private Dictionary<BuffInstance, BuffIcon> list = new ();
    private Queue<BuffIcon> queue = new();

    private void Awake()
    {
        buffAdministrator.OnCreateInstanceAfter += BuffAdministrator_OnAfterApply;
        buffAdministrator.OnDeleteInstanceAfter += BuffAdministrator_OnAfterRemove;
    }

    private void BuffAdministrator_OnAfterApply(BuffInstance instance)
    {
        Debug.Log(instance.Buff.BuffName);
        BuffIcon icon = CreateBuffIcon();
        icon.Init(instance);
        list.Add(instance, icon);
    }

    private void BuffAdministrator_OnAfterRemove(BuffInstance buff)
    {
        if(list.TryGetValue(buff, out var value))
        {
            DeleteBuffIcon(value);
            list.Remove(buff);
        }
    }

    private BuffIcon CreateBuffIcon()
    {
        if (queue.TryDequeue(out BuffIcon result))
        {
            result.gameObject.SetActive(true);
            return result;
        }
            
        return Instantiate(prefab, transform);
    }

    private void DeleteBuffIcon(BuffIcon icon)
    {
        queue.Enqueue(icon);
        icon.gameObject.SetActive(false);
    }
}
