using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBarHandler : MonoBehaviour
{
    private Image energyBar;
    private UniversalManipulator um;
    // Start is called before the first frame update
    void Start()
    {
        energyBar = gameObject.GetComponent<Image>();
        um = UniversalManipulator.Instance;

        um.OnUMPerform.AddListener(UpdateBar);
    }

    private void UpdateBar()
    {
        energyBar.fillAmount = um.umEnergy / 100;

        if (um.umEnergy > 50f) energyBar.color = new Color32(54, 240, 151, 255);
        else if (um.umEnergy > 25f) energyBar.color = new Color32(240, 221, 54, 255);
        else energyBar.color = new Color32(240, 54, 54, 255);
    }
}
