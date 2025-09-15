using UnityEngine;

public interface IClickable
{
    public void OnClick(RaycastHit hit);
    public void OnDoubleClick(RaycastHit hit);
}

public interface ILongPressable
{
    public void OnLongPress(RaycastHit hit);
}

public interface IDraggable
{
    public void OnDragStart(RaycastHit hit);
    public void OnDrag(RaycastHit hit);
    public void OnDragEnd(RaycastHit hit);
}
