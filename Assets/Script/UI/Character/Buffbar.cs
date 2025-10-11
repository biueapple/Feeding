using System.Collections.Generic;
using UnityEngine;

public class Buffbar : MonoBehaviour
{
    [SerializeField]
    private BuffAdministrator buffAdministrator;

    [SerializeField]
    private BuffIcon prefab;

    private Dictionary<Buff, BuffIcon> list = new ();
    private Queue<BuffIcon> queue = new();

    private void Awake()
    {
        buffAdministrator.OnAfterApply += BuffAdministrator_OnAfterApply;
        buffAdministrator.OnAfterRemove += BuffAdministrator_OnAfterRemove;
    }

    private void BuffAdministrator_OnAfterApply(Buff buff)
    {
        Debug.Log(buff.BuffName);
        BuffIcon icon = CreateBuffIcon();
        icon.Init(buff);
        list.Add(buff, icon);
    }

    private void BuffAdministrator_OnAfterRemove(Buff buff)
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
