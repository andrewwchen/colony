using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(AudioSource))]
public class UniversalManipulator : MonoBehaviour
{
    // singleton instance
    public static UniversalManipulator Instance;

    public GameObject removing_controller;
    public GameObject planting_controller;
    public GameObject watering_controller;
    public GameObject tilling_controller;
    public GameObject building_controller;

    private AudioSource source;
    private PlayerInputTranslator pit;

    private static float rayDistance = 10f;

    [SerializeField] private AudioClip triggerClip;
    [SerializeField] private MenuUIHandler wristMenu;

    // the order in which UM modes cycle through. other modes activate when you select an item in the inventory (e.g.: UMMode.Building, UMMode.Planting)
    private static UMMode[] modeCycle = { UMMode.Removing, UMMode.Tilling, UMMode.Watering };

    private UMMode currentMode = modeCycle[0];
    private int currentModeNum = 0;
    private Item currentItem;
    private StructureManager sm;
    private InventoryManager im;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        pit = PlayerInputTranslator.Instance;
        sm = StructureManager.Instance;
        im = InventoryManager.Instance;
        
        SetMode(currentMode);

        // start listeners
        pit.OnLeftTriggerPress.AddListener(OnPressLeftTrigger);
        pit.OnLeftPrimaryButtonPress.AddListener(ToggleWristMenu);
        pit.OnRightTriggerPress.AddListener(Perform);
        pit.OnRightJoystickTiltRight.AddListener(CycleModeForward);
        pit.OnRightJoystickTiltLeft.AddListener(CycleModeBackward);
    }

    private void Perform()
    {
        RaycastHit hit;
        switch (currentMode)
        {
            case UMMode.Building:
                if (im.inventory[currentItem] > 0 && Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    if (sm.MakePlacement(currentItem.placeable, hit.point, new AnimalData[0], new PlantData(PlantType.None, 0)))
                    {
                        im.RemoveItem(currentItem);
                    }
                }
                break;
            case UMMode.Planting:
                if (im.inventory[currentItem] > 0 && Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    if (sm.MakePlant(currentItem.plantable, hit.point))
                    {
                        im.RemoveItem(currentItem);
                    }
                }
                break;
            case UMMode.Removing:
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    sm.MakeRemoval(hit.point);
                }
                break;
            case UMMode.Tilling:
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    sm.MakeTill(hit.point);
                }
                break;
            case UMMode.Watering:
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    sm.MakeWater(hit.point);
                }
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        switch (currentMode)
        {
            case UMMode.Building:
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    sm.HoverPlacement(currentItem.placeable, hit.point);
                }
                break;
            case UMMode.Planting:
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    sm.HoverPlant(hit.point);
                }
                break;
            case UMMode.Removing:
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    sm.HoverRemoval(hit.point);
                }
                break;
            case UMMode.Tilling:
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    sm.HoverTill(hit.point);
                }
                break;
            case UMMode.Watering:
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    sm.HoverWater(hit.point);
                }
                break;
        }
    }

    public void OnPressLeftTrigger()
    {
        source.clip = triggerClip;
        source.Play();
    }

    private void ToggleWristMenu()
    {
        wristMenu.toggleDisplay();
    }

    private void CycleModeForward()
    {
        // check if the current mode is in the mode cycle, cycle through the modes forwards
        if (modeCycle[currentModeNum] == currentMode)
            currentModeNum = (currentModeNum + 1) % modeCycle.Length;

        // otheriwse, set the mode back to whatever it was before an item was selected in the inventory and a non-cycle mode was set
        SetMode(modeCycle[currentModeNum]);
    }

    private void CycleModeBackward()
    {
        // check if the current mode is in the mode cycle, cycle through the modes backwards
        if (modeCycle[currentModeNum] == currentMode)
            currentModeNum = (currentModeNum + modeCycle.Length - 1) % modeCycle.Length;

        // otheriwse, set the mode back to whatever it was before an item was selected in the inventory and a non-cycle mode was set
        SetMode(modeCycle[currentModeNum]);
    }

    public void SetMode(UMMode mode, Item item = null)
    {
        currentMode = mode;
        currentItem = item;
        Debug.Log("Switching UM mode to " + mode);
        switch (mode)
        {
            case UMMode.Building:
                sm.Unhover();
                building_controller.SetActive(true);
                planting_controller.SetActive(false);
                removing_controller.SetActive(false);
                tilling_controller.SetActive(false);
                watering_controller.SetActive(false);
                break;
            case UMMode.Planting:
                sm.Unhover();
                building_controller.SetActive(false);
                planting_controller.SetActive(true);
                removing_controller.SetActive(false);
                tilling_controller.SetActive(false);
                watering_controller.SetActive(false);
                break;
            case UMMode.Removing:
                sm.Unhover();
                building_controller.SetActive(false);
                planting_controller.SetActive(false);
                removing_controller.SetActive(true);
                tilling_controller.SetActive(false);
                watering_controller.SetActive(false);
                break;
            case UMMode.Tilling:
                sm.Unhover();
                building_controller.SetActive(false);
                planting_controller.SetActive(false);
                removing_controller.SetActive(false);
                tilling_controller.SetActive(true);
                watering_controller.SetActive(false);
                break;
            case UMMode.Watering:
                sm.Unhover();
                building_controller.SetActive(false);
                planting_controller.SetActive(false);
                removing_controller.SetActive(false);
                tilling_controller.SetActive(false);
                watering_controller.SetActive(true);
                break;
        }
    }
}
