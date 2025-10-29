using UnityEngine;

public class Visitor : MonoBehaviour
{
    [SerializeField]
    private VisitorSO so;
    public VisitorSO SO { get => so; set { so = value; } }
        
}
