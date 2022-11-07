using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ChangeControllers : MonoBehaviour
{
    public Transform teleport_controller;
    public Transform grab_controller;
    public Transform sowing_controller;
    public Transform watering_controller;
    public Transform tilling_controller;
    public Transform building_controller;

    int current_controller;

    public XRBaseController left_hand;
    public XRBaseController right_hand;

    // Start is called before the first frame update
    void Start()
    {
        
        left_hand.modelPrefab = teleport_controller;
        right_hand.modelPrefab = grab_controller;
        current_controller = 0;

    }

    void change()
    {
        current_controller++;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (current_controller == 0)
            right_hand.modelPrefab = grab_controller;
        else if (current_controller == 1)
            right_hand.modelPrefab = sowing_controller;
    }
}
