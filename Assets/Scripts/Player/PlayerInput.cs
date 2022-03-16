using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] PlayerController motor = null;

    private Controls controls;
    private Controls Controls
    {
        get
        {
            if (controls != null) { return controls; }
            return controls = new Controls();
        }
    }
    public void OnMove(CallbackContext context)
    {
        if (motor == null) return;
        var move = context.ReadValue<Vector2>();
        motor.SetMove(move);
    }
    public void OnLook(CallbackContext context)
    {
        if (motor == null) return;
        var look = context.ReadValue<Vector2>();
        motor.SetLook(look);
    }
    public void OnMouseLook(CallbackContext context)
    {
        if (motor == null) return;
        var look = context.ReadValue<Vector2>();

        var mousePlayerDelta = Camera.main.ScreenToWorldPoint(look) - transform.position;
        motor.SetLook((Vector2)mousePlayerDelta);
    }
}
