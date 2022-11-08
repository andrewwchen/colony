using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimalInstanceMenu : MonoBehaviour
{
    [SerializeField] private Button buy;
    [SerializeField] private Button collect;
    [SerializeField] private Button feed;
    [SerializeField] private Transform animalSpawn;

    [HideInInspector] public AnimalInstance animal;
    [HideInInspector] public Animal animalConfig;
    [HideInInspector] public AnimalData data;

    private InventoryManager im;
    private bool didCollectToday = false;

    // Start is called before the first frame update
    void Start()
    {
        buy.onClick.AddListener(Buy);
        collect.onClick.AddListener(Collect);
        feed.onClick.AddListener(Feed);
        im = InventoryManager.Instance;
        DayManager.Instance.OnEndDay.AddListener(OnEndDay);
    }

    public void Setup(AnimalData data, AnimalType type)
    {
        animalConfig = AnimalManager.Instance.typeToAnimal[type];
        if (data.type == AnimalType.None)
        {
            NoAnimal();
        } else
        {
            Spawn(data);
        }
    }

    private void OnEndDay()
    {
        didCollectToday = false;
        data.daysUnfed += 1;
        if (animal != null && data.daysUnfed > animalConfig.starveDays)
        {
            Destroy(animal.gameObject);
            animal = null;
            data = new AnimalData(AnimalType.None, 0);
            NoAnimal();
        }
    }

    private void NoAnimal()
    {
        buy.gameObject.SetActive(true);
        feed.gameObject.SetActive(false);
        collect.gameObject.SetActive(false);
    }

    private void YesAnimal()
    {
        buy.gameObject.SetActive(false);
        feed.gameObject.SetActive(true);
        collect.gameObject.SetActive(true);
    }

    private void Buy()
    {
        if (animal == null && im.SpendMoney(animalConfig.buyPrice))
        {
            Spawn();
        }
    }

    private void Spawn(AnimalData data = null)
    {
        this.data = data ?? new AnimalData(animalConfig.type, 1);
        GameObject go = Instantiate(animalConfig.prefab);
        go.transform.position = animalSpawn.position;
        go.transform.rotation = animalSpawn.rotation;
        animal = go.GetComponent<AnimalInstance>();
        YesAnimal();
    }

    private void Collect()
    {
        if (!didCollectToday)
        {
            didCollectToday = true;
            Debug.Log("Collected from Animal");
            im.AddItem(animalConfig.product);
        }
    }

    private void Feed()
    {
        if (data.daysUnfed > 0 && im.RemoveItem(im.typeToItem[ItemType.Feed]))
        {
            Debug.Log("Fed Animal");
            data.daysUnfed = 0;
        }
    }



}
