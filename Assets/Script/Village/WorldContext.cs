using System;
using UnityEngine;

public class WorldContext : MonoBehaviour
{
    public static WorldContext Instance { get; private set; }
    public VillageSO CurrentVillage { get; private set; }
    public event Action<VillageSO> OnVillageChanged;

    private void Awake()
    {
        Instance = this;
    }

    public void SetVillage(VillageSO villageSO)
    {
        if (CurrentVillage == villageSO) return;
        CurrentVillage = villageSO;
        OnVillageChanged?.Invoke(villageSO);
    }
}
