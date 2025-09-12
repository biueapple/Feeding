using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Stat")]
public class Stat : ScriptableObject
{
    [SerializeField]
    private List<StatMapping> stats;
    public IReadOnlyList<StatMapping> Stats => stats;


}
