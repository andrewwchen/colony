using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PlantManager : MonoBehaviour
{
    // singleton instance
    public static PlantManager Instance;
    // list of all plants available in the game
    public Plant[] plants;
    // maps plant types to plants
    [HideInInspector] public Dictionary<PlantType, Plant> typeToPlant = new Dictionary<PlantType, Plant>();
    // maps plants to plant types
    [HideInInspector] public Dictionary<Plant, PlantType> plantToType = new Dictionary<Plant, PlantType>();

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
    private void Start()
    {
        // initializing dictionaries based on default values
        for (int i = 0; i < plants.Length; i++)
        {
            Plant plant = plants[i];
            PlantType type = plant.type;
            typeToPlant.Add(type, plant);
            plantToType[plant] = type;
        }
    }
}
