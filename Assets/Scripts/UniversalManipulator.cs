using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(AudioSource))]
public class UniversalManipulator : MonoBehaviour
{
    public Transform teleport_controller;
    public Transform removing_controller;
    public Transform planting_controller;
    public Transform watering_controller;
    public Transform tilling_controller;
    public Transform building_controller;

    public XRBaseController left_hand;
    public XRBaseController right_hand;
    private AudioSource source;
    private PlayerInputTranslator pit;

    [SerializeField] private AudioClip triggerClip;

    // the order in which UM modes cycle through. other modes activate when you select an item in the inventory (e.g.: UMMode.Building, UMMode.Planting)
    private static UMMode[] modeCycle = { UMMode.Removing, UMMode.Tilling, UMMode.Watering };

    private UMMode currentMode = modeCycle[0];
    private int currentModeNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        pit = PlayerInputTranslator.Instance;
        left_hand.modelPrefab = teleport_controller;

        SetMode(currentMode);

        // start listeners
        pit.OnLeftTriggerPress.AddListener(OnPressLeftTrigger);
        pit.OnRightTriggerPress.AddListener(OnPressRightTrigger);
        pit.OnRightJoystickTiltRight.AddListener(CycleModeForward);
        pit.OnRightJoystickTiltLeft.AddListener(CycleModeBackward);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPressLeftTrigger()
    {
        source.clip = triggerClip;
        source.Play();
        Debug.Log("performed left");
    }

    public void OnPressRightTrigger()
    {
        Debug.Log("performed right");
    }

    private void CycleModeForward()
    {
        // check if the current mode is in the mode cycle, cycle through the modes forwards
        if (modeCycle[currentModeNum] == currentMode)
            currentModeNum = (currentModeNum + 1) % modeCycle.Length;

        // otheriwse, set the mode back to whatever it was before an item was selected in the inventory and a non-cycle mode 
        SetMode(modeCycle[currentModeNum]);
    }

    private void CycleModeBackward()
    {
        // check if the current mode is in the mode cycle, cycle through the modes backwards
        if (modeCycle[currentModeNum] == currentMode)
            currentModeNum = (currentModeNum + modeCycle.Length - 1) % modeCycle.Length;

        // otheriwse, set the mode back to whatever it was before an item was selected in the inventory and a non-cycle mode 
        SetMode(modeCycle[currentModeNum]);
    }

    public void SetMode(UMMode mode)
    {
        currentMode = mode;
        Debug.Log("Switching UM mode to " + mode);
        switch (mode)
        {
            case UMMode.Building:
                right_hand.modelPrefab = building_controller;
                break;
            case UMMode.Planting:
                right_hand.modelPrefab = planting_controller;
                break;
            case UMMode.Removing:
                right_hand.modelPrefab = removing_controller;
                break;
            case UMMode.Tilling:
                right_hand.modelPrefab = tilling_controller;
                break;
            case UMMode.Watering:
                right_hand.modelPrefab = watering_controller;
                break;
        }
    }
}
