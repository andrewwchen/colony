using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// TODO: add speedup with modifier key, add smoothing

/// <summary> This class provides first person free-flight movement for desktop. </summary>
public class DesktopRigController : MonoBehaviour
{
    public InputActionReference lateralMoveAction;
    public float lookSpeed = 1;
    public float moveSpeed = 30;
    private Transform rig;
    private int num = 0;
    private Camera cam;

    public static bool IsLooking { get; private set; } = false;

    private CharacterController characterController = null;

    private bool isCreating = false;
    private bool isDeleting = false;
    private bool isPlanting = false;


    void Start()
    {
        rig = transform;
        cam = GetComponent<Camera>();

        characterController = rig.GetComponentInChildren(typeof(CharacterController)) as CharacterController;
    }

    void Update()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Vector3 angles = rig.localEulerAngles;
            angles.y += lookSpeed * Mouse.current.delta.x.ReadValue();
            angles.x -= lookSpeed * Mouse.current.delta.y.ReadValue();
            rig.rotation = Quaternion.Euler(angles);

            Vector3 direction = new Vector3(lateralMoveAction.action.ReadValue<Vector2>().x, 0, lateralMoveAction.action.ReadValue<Vector2>().y);
            StartMove(direction);
        }

        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Structure s = StructureManager.Instance.structures[Mathf.Min(num, StructureManager.Instance.structures.Length - 1)];
        Plant p = PlantManager.Instance.plants[Mathf.Min(num, PlantManager.Instance.plants.Length-1)];
        if (Keyboard.current.cKey.isPressed)
        {
            if (Physics.Raycast(ray, out hit))
            {
                StructureManager.Instance.HoverPlacement(s, hit.point);
            }

            if (!isCreating)
            {
                isCreating = true;

            }
        }
        else
        {
            if (isCreating)
            {
                isCreating = false;
                if (Physics.Raycast(ray, out hit))
                {
                    StructureManager.Instance.MakePlacement(s, hit.point, new AnimalData[0], new PlantData(PlantType.None, 0));
                }
            }
        }

        if (Keyboard.current.dKey.isPressed)
        {
            if (Physics.Raycast(ray, out hit))
            {
                StructureManager.Instance.HoverRemoval(hit.point);
            }

            if (!isDeleting)
            {
                isDeleting = true;

            }
        }
        else
        {
            if (isDeleting)
            {
                isDeleting = false;
                if (Physics.Raycast(ray, out hit))
                {
                    StructureManager.Instance.MakeRemoval(hit.point);
                }
            }
        }

        if (Keyboard.current.pKey.isPressed)
        {
            if (Physics.Raycast(ray, out hit))
            {
                StructureManager.Instance.HoverPlant(hit.point);
            }

            if (!isPlanting)
            {
                isPlanting = true;

            }
        }
        else
        {
            if (isPlanting)
            {
                isPlanting = false;
                if (Physics.Raycast(ray, out hit))
                {
                    StructureManager.Instance.MakePlant(p, hit.point);
                }
            }
        }


        if (Keyboard.current.digit0Key.isPressed)
        {
            num = 0;
        }
        else if (Keyboard.current.digit1Key.isPressed)
        {
            num = 1;
        }
        else if (Keyboard.current.digit2Key.isPressed)
        {
            num = 2;
        }
        else if (Keyboard.current.digit3Key.isPressed)
        {
            num = 3;
        }
        else if (Keyboard.current.digit4Key.isPressed)
        {
            num = 4;
        }
        else if (Keyboard.current.digit5Key.isPressed)
        {
            num = 5;
        }
        else if (Keyboard.current.digit6Key.isPressed)
        {
            num = 6;
        }
        else if (Keyboard.current.digit7Key.isPressed)
        {
            num = 7;
        }
        else if (Keyboard.current.digit8Key.isPressed)
        {
            num = 8;
        }
        else if (Keyboard.current.digit9Key.isPressed)
        {
            num = 9;
        }


        void StartMove(Vector3 direction)
        {
            direction = Quaternion.Euler(rig.localEulerAngles) * direction;

            characterController.Move(direction * Time.deltaTime);
            if (Keyboard.current.wKey.isPressed)
            {
                characterController.Move(rig.rotation * Vector3.forward * moveSpeed * Time.deltaTime);
            }
        }
    }
}
