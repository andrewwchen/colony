using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalInstance : MonoBehaviour
{
    [HideInInspector] public Animal config;
    [HideInInspector] public string displayName;
    [HideInInspector] public int daysUnfed;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Setup(Animal animal, string displayName, int daysUnfed)
    {
        config = animal;
        this.displayName = displayName;
        this.daysUnfed = daysUnfed;
    }
}
