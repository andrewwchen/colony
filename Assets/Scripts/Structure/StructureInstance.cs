using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureInstance : MonoBehaviour
{
    public AnimalInstanceMenu[] animalMenus;
    [SerializeField] private Transform plantSpawn;
    [SerializeField] private Material dryMaterial;
    [SerializeField] private Material wetMaterial;
    [HideInInspector] public Structure config;
    [HideInInspector] public StructureDirection direction;
    [HideInInspector] public int row;
    [HideInInspector] public int col;
    [HideInInspector] public (int, int)[] occupied;

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
        DayManager.Instance.OnEndDay.AddListener(OnEndDay);
    }

    public void Setup(Structure structure, StructureDirection direction, int row, int col, (int, int)[] occupied, AnimalData[] animals, PlantData plant)
    {
        config = structure;
        this.direction = direction;
        this.row = row;
        this.col = col;
        this.occupied = occupied;

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

        for (int i = 0; i < animalMenus.Length; i++)
        {
            AnimalInstanceMenu aim = animalMenus[i];
            AnimalData data = i < animals.Length ? animals[i] : new AnimalData(AnimalType.None, 0);
            aim.Setup(data, config.animal.type);
        }

        if (plant.type != PlantType.None)
        {
            SetPlant(plant);
        }
    }

    private void OnEndDay()
    {
        if (config.isPlot && plantData.type != PlantType.None)
        {
            plantData.age += 1;

            Plant plantConfig = PlantManager.Instance.typeToPlant[plantData.type];

            if (plantConfig.doesGrow && (!plantConfig.needsWater || isWatered) && plantData.age >= plantConfig.growDays)
            {
                Plant newConfig = PlantManager.Instance.typeToPlant[plantConfig.nextStage];
                PlantData newData = new PlantData(newConfig.type, 0);
                ClearPlant();
                SetPlant(newData);
            }

        }
        if (config.isPlot)
        {
            UnwaterPlot();
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
        if (config.isPlot)
        {
            isWatered = false;
            r.material = dryMaterial;
        }
    }

    public bool CanPlant()
    {
        return config.isPlot && plantGameObject == null;
    }

    public bool MakePlant(PlantData data)
    {
        if (!CanPlant())
            return false;
        SetPlant(data);
        return true;
    }

    private void SetPlant(PlantData data)
    {
        Plant plant = PlantManager.Instance.typeToPlant[data.type];
        Debug.Log(data.type);
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
            plantData = new PlantData(PlantType.None, 0);
        }
    }

    public bool CanRemove()
    {
        foreach (AnimalInstanceMenu aim in animalMenus)
        {
            if (aim.data.type != AnimalType.None)
            {
                return false;
            }
        }
        return true;
    }

    private void OnDestroy()
    {
        if (config.product != null)
            im.AddItem(config.product);

        if (plantData.type != PlantType.None)
        {
            Destroy(plantGameObject);

            Plant plant = pm.typeToPlant[plantData.type];

            if (plant.product != null)
                im.AddItem(plant.product);
        }
    }
}
