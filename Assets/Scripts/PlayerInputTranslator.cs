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

    public UnityEvent OnLeftTriggerPress;
    public UnityEvent OnRightTriggerPress;

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

    
}
