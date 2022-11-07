using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(AudioSource))]
public class UniversalManipulator : MonoBehaviour
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
    private AudioSource source;

    [SerializeField] private AudioClip triggerClip;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();

        left_hand.modelPrefab = teleport_controller;
        right_hand.modelPrefab = grab_controller;
        current_controller = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPressTrigger()
    {
        source.clip = triggerClip;
        source.Play();
        Debug.Log("performed");
        change_controller();
    }

    void change_controller()
    {
        current_controller++;
        if (current_controller == 0)
            right_hand.modelPrefab = grab_controller;
        else if (current_controller == 1)
            right_hand.modelPrefab = sowing_controller;


    }
}
