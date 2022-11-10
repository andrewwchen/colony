using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

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

    public float umEnergy = 100f;
    private static float plantingEnergyCost = 5f; // change these later to what we want
    private static float tillingEnergyCost = 5f;
    private static float wateringEnergyCost = 7f;

    private AudioSource source;
    private PlayerInputTranslator pit;

    private static float rayDistance = 10f;

    [SerializeField] private AudioClip errorClip;
    [SerializeField] private AudioClip switchClip;
    [SerializeField] private AudioClip buildingClip;
    [SerializeField] private AudioClip plantingClip;
    [SerializeField] private AudioClip removingClip;
    [SerializeField] private AudioClip tillingClip;
    [SerializeField] private AudioClip wateringClip;
    [SerializeField] private AudioClip rechargeClip;
    [SerializeField] private AudioClip wristMenuClip;
    [SerializeField] private AudioClip closeClip;
    [SerializeField] private MenuUIHandler wristMenu;

    // the order in which UM modes cycle through. other modes activate when you select an item in the inventory (e.g.: UMMode.Building, UMMode.Planting)
    private static UMMode[] modeCycle = { UMMode.Removing, UMMode.Tilling, UMMode.Watering };

    private UMMode currentMode = modeCycle[0];
    private int currentModeNum = 0;
    private Item currentItem;
    private StructureManager sm;
    private InventoryManager im;

    [HideInInspector] public UnityEvent OnUMPerform;

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
                        PlaySound(buildingClip);
                        im.RemoveItem(currentItem);
                    }
                    else PlaySound(errorClip);
                }
                //PlaySound(errorClip);
                break;
            case UMMode.Planting:
                if (im.inventory[currentItem] > 0 && Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    if (umEnergy > plantingEnergyCost && sm.MakePlant(currentItem.plantable, hit.point))
                    {
                        PlaySound(plantingClip);
                        im.RemoveItem(currentItem);
                        umEnergy -= plantingEnergyCost;
                        OnUMPerform?.Invoke();
                    }
                    else PlaySound(errorClip);
                }
                //PlaySound(errorClip);
                break;
            case UMMode.Removing:
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    if (sm.MakeRemoval(hit.point))
                    {
                        PlaySound(removingClip);
                    }
                    else PlaySound(errorClip);
                }
                //PlaySound(errorClip);
                break;
            case UMMode.Tilling:
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    if (umEnergy > tillingEnergyCost && sm.MakeTill(hit.point))
                    {
                        PlaySound(tillingClip);
                        umEnergy -= tillingEnergyCost;
                        OnUMPerform?.Invoke();
                    }
                    else PlaySound(errorClip);
                }
                //PlaySound(errorClip);
                break;
            case UMMode.Watering:
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, rayDistance))
                {
                    if (umEnergy > wateringEnergyCost && sm.MakeWater(hit.point))
                    {
                        PlaySound(wateringClip);
                        umEnergy -= wateringEnergyCost;
                        OnUMPerform?.Invoke();
                    }
                    else PlaySound(errorClip);
                }
                //PlaySound(errorClip);
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

    private void PlaySound(AudioClip c)
    {
        source.clip = c;
        source.Play();
    }

    public void OnPressLeftTrigger()
    {
    }

    private void ToggleWristMenu()
    {
        PlaySound(wristMenu.gameObject.activeSelf ? closeClip : wristMenuClip);
        wristMenu.toggleDisplay();
    }

    private void CycleModeForward()
    {
        // check if the current mode is in the mode cycle, cycle through the modes forwards
        if (modeCycle[currentModeNum] == currentMode)
            currentModeNum = (currentModeNum + 1) % modeCycle.Length;

        // otheriwse, set the mode back to whatever it was before an item was selected in the inventory and a non-cycle mode was set
        SetMode(modeCycle[currentModeNum]);
        PlaySound(switchClip);
    }

    private void CycleModeBackward()
    {
        // check if the current mode is in the mode cycle, cycle through the modes backwards
        if (modeCycle[currentModeNum] == currentMode)
            currentModeNum = (currentModeNum + modeCycle.Length - 1) % modeCycle.Length;

        // otheriwse, set the mode back to whatever it was before an item was selected in the inventory and a non-cycle mode was set
        SetMode(modeCycle[currentModeNum]);
        PlaySound(switchClip);
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

    public void resetEnergy()
    {
        PlaySound(rechargeClip);
        umEnergy = 100f;
        OnUMPerform?.Invoke();
    }
}
