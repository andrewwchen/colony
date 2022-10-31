using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class UniversalManipulator : MonoBehaviour
{
    private AudioSource source;
    [SerializeField] private AudioClip triggerClip;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        PlayerInputTranslator.Instance.OnLeftTriggerPress.AddListener(OnPressTrigger);
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
    }
}
