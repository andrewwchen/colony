using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerInputTranslator : MonoBehaviour
{
    //singleton instance
    public static PlayerInputTranslator Instance;
    private static float tiltThreshold = 0.75f;

    [HideInInspector] public UnityEvent OnLeftTriggerPress;
    [HideInInspector] public UnityEvent OnRightTriggerPress;
    [HideInInspector] public UnityEvent OnLeftPrimaryButtonPress;
    [HideInInspector] public UnityEvent OnRightJoystickTiltRight;
    [HideInInspector] public UnityEvent OnRightJoystickTiltLeft;

    private bool isRightJoystickTiltingRight = false;
    private bool isRightJoystickTiltingLeft = false;

    private void Awake()
    {
        // Managing DataManager persistence between scenes
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PressLeftTrigger(InputAction.CallbackContext ctx)
    {
        // Check for the phase, you want to execute buttons logic only on the performed phase
        if (!ctx.performed) return;
        // your logic
        OnLeftTriggerPress.Invoke();
    }

    public void PressRightTrigger(InputAction.CallbackContext ctx)
    {
        // Check for the phase, you want to execute buttons logic only on the performed phase
        if (!ctx.performed) return;
        // your logic
        OnRightTriggerPress.Invoke();
    }

    public void PressLeftPrimaryButton(InputAction.CallbackContext ctx)
    {
        // Check for the phase, you want to execute buttons logic only on the performed phase
        if (!ctx.performed) return;
        // your logic
        OnLeftPrimaryButtonPress.Invoke();
    }

    public void MoveRightPrimary2DAxis(InputAction.CallbackContext ctx)
    {
        Vector2 tilt = ctx.ReadValue<Vector2>();

        if (tilt.x > tiltThreshold)
        {   
            if (!isRightJoystickTiltingRight)
            {
                OnRightJoystickTiltRight?.Invoke();
                isRightJoystickTiltingRight = true;
            }
        } else
        {
            isRightJoystickTiltingRight = false;
        }

        if (tilt.x < -tiltThreshold)
        {
            if (!isRightJoystickTiltingLeft)
            {
                OnRightJoystickTiltLeft?.Invoke();
                isRightJoystickTiltingLeft = true;
            }
        }
        else
        {
            isRightJoystickTiltingLeft = false;
        }
    }

}
