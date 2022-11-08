using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class DayManager : MonoBehaviour
{
    public int day;
    public Transform directionalLightPivot;
    [HideInInspector] public int time = 0;

    // Start is called before the first frame update
    void Start()
    {
        day = DataManager.Instance.gameData.day;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
