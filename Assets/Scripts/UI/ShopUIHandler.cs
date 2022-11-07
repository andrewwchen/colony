using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopUIHandler : MonoBehaviour
{
    public Transform cam;
    public GameObject mainMenu;
    public GameObject inventoryMenu;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.SetActive(true);
        inventoryMenu.SetActive(false);
    }

    void LateUpdate()
    {
        gameObject.transform.LookAt(gameObject.transform.position + cam.forward);
    }

    public void toggleDisplay()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void showMainMenu()
    {
        mainMenu.SetActive(true);
        inventoryMenu.SetActive(false);
    }

    public void showInventoryMenu()
    {
        mainMenu.SetActive(false);
        inventoryMenu.SetActive(true);
    }
}
