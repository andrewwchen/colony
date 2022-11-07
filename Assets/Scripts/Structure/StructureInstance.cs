using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureInstance : MonoBehaviour
{
    [SerializeField] private Transform[] animalSpawns;
    [SerializeField] private Transform plantSpawn;
    [HideInInspector] public Structure config;
    [HideInInspector] public StructureDirection direction;
    [HideInInspector] public int row;
    [HideInInspector] public int col;
    [HideInInspector] public (int, int)[] occupied;
    [HideInInspector] public HashSet<AnimalInstance> animals = new HashSet<AnimalInstance>();

    private GameObject plantGameObject;
    [HideInInspector] public PlantData plantData;

    private InventoryManager im;
    private PlantManager pm;

    // Start is called before the first frame update
    void Start()
    {
        im = InventoryManager.Instance;
        pm = PlantManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(Structure structure, StructureDirection direction, int row, int col, (int, int)[] occupied, AnimalData[] animals, PlantData plant)
    {
        config = structure;
        this.direction = direction;
        this.row = row;
        this.col = col;
        this.occupied = occupied;
        if (animals != null)
        {
            foreach (AnimalData data in animals)
            {
                SpawnAnimal(data);
            }
        }

        if (plant != null)
        {
            MakePlant(plant);
        }

        switch (direction)
        {
            case StructureDirection.West:
                transform.Rotate(Vector3.up * 90);
                break;
            case StructureDirection.North:
                transform.Rotate(Vector3.up * 180);
                break;
            case StructureDirection.East:
                transform.Rotate(Vector3.up * 270);
                break;
        }
    }

    public void SpawnAnimal(AnimalData data)
    {
        if (animals.Count >= config.animalLimit)
            return;

        Animal animal = AnimalManager.Instance.typeToAnimal[data.type];

        GameObject go = Instantiate(animal.prefab);
        // go.transform.position = animalSpawn.position;
        AnimalInstance ai = go.GetComponent<AnimalInstance>();
        ai.Setup(animal, data.displayName, data.daysUnfed);
        animals.Add(ai);
    }

    public bool CanPlant()
    {
        return config.isPlot && plantGameObject == null;
    }

    public void MakePlant(PlantData data)
    {
        if (!CanPlant())
            return;
        Plant plant = PlantManager.Instance.typeToPlant[data.type];
        GameObject go = Instantiate(plant.prefab);
        go.transform.position = plantSpawn.position;
        plantGameObject = go;
        plantData = data;
    }

    public void ClearPlant()
    {
        if (plantGameObject != null)
        {
            Destroy(plantGameObject);
            plantData = null;
        }
    }

    public bool CanRemove()
    {
        return animals.Count == 0;
    }

    private void OnDestroy()
    {
        if (config.product != null)
            im.AddItem(config.product);

        if (plantData != null)
        {
            im.AddItem(pm.typeToPlant[plantData.type].product);
            Destroy(plantGameObject);
        }
    }
}
