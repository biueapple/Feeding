using UnityEngine;
using UnityEngine.InputSystem;

public class TestMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Keyboard.current.wKey.isPressed)
        {

        }
    }

    public void OnMove(InputValue value)
    {
        Debug.Log(value.Get<Vector2>());
    }
}
