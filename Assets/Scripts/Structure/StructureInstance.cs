using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureInstance : MonoBehaviour
{
    [SerializeField] private Transform[] animalSpawns;
    [SerializeField] private Transform plantSpawn;
    [SerializeField] private Material dryMaterial;
    [SerializeField] private Material wetMaterial;
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
    private Renderer r;
    private bool isWatered = false;

    // Start is called before the first frame update
    void Start()
    {
        im = InventoryManager.Instance;
        pm = PlantManager.Instance;
        r = GetComponent<Renderer>();
        UnwaterPlot();
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

    public bool CanWaterPlot()
    {
        return config.isPlot && !isWatered;
    }

    public bool WaterPlot()
    {
        if (CanWaterPlot())
        {
            isWatered = true;
            r.material = wetMaterial;
            return true;
        }
        return false;
    }

    public void UnwaterPlot()
    {
        isWatered = false;
        r.material = dryMaterial;
    }



    public bool CanSpawnAnimal()
    {
        return animals.Count < config.animalLimit;
    }

    public void SpawnAnimal(AnimalData data)
    {
        if (!CanSpawnAnimal())
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

    public bool MakePlant(PlantData data)
    {
        if (!CanPlant())
            return false;
        Plant plant = PlantManager.Instance.typeToPlant[data.type];
        GameObject go = Instantiate(plant.prefab);
        go.transform.position = plantSpawn.position;
        plantGameObject = go;
        plantData = data;
        return true;
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
            Destroy(plantGameObject);

            Plant plant = pm.typeToPlant[plantData.type];

            if (plant.product != null)
                im.AddItem(plant.product);
        }
    }
}
