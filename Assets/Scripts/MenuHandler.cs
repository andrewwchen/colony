using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuHandler : MonoBehaviour
{
    public Transform cam;
    public GameObject mainMenu;
    public GameObject inventoryMenu;
    public GameObject tooltipMenu;

    // Start is called before the first frame update
    void Start()
    {
        showMainMenu();
    }

    void LateUpdate()
    {
        gameObject.transform.LookAt(gameObject.transform.position + cam.forward);
    }

    public void showMainMenu()
    {
        mainMenu.SetActive(true);
        inventoryMenu.SetActive(false);
        tooltipMenu.SetActive(false);
    }

    public void showInventoryMenu()
    {
        mainMenu.SetActive(false);
        inventoryMenu.SetActive(true);
        tooltipMenu.SetActive(false);
    }

    public void showTooltipMenu()
    {
        mainMenu.SetActive(false);
        inventoryMenu.SetActive(false);
        tooltipMenu.SetActive(true);
    }
}
