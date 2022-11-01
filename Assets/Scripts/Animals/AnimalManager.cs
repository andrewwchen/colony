using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    // singleton instance
    public static AnimalManager Instance;
    // list of all animals available in the game
    [SerializeField] private Animal[] animals;
    // maps animal types to animals
    [HideInInspector] public Dictionary<AnimalType, Animal> typeToAnimal;
    // maps animals to animal types
    [HideInInspector] public Dictionary<Animal, AnimalType> animalToType;

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
        for (int i = 0; i < animals.Length; i++)
        {
            Animal animal = animals[i];
            AnimalType type = animal.type;
            typeToAnimal[type] = animal;
            animalToType[animal] = type;
        }
    }
}
