using UnityEngine;

public abstract class TooltipElementRenderer : MonoBehaviour
{
    public abstract TooltipElementType Type { get; }
    public abstract void Bind(TooltipElementModel model);
    public virtual void OnDespawn() { }
}
