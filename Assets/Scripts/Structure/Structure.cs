using UnityEngine;

[CreateAssetMenu(fileName = "Structure", menuName = "ScriptableObjects/Structure")]
public class Structure : ScriptableObject
{
    // type of the structure
    public StructureType type;
    // prefab that is instantiated when the structure is placed
    public GameObject prefab;
    // the number of columns the structure takes up
    public int cols;
    // the number of rows the structure takes up
    public int rows;
    // whether this structure is a stable that holds animals
    public bool isStable;
    // the type of animal this stable can hold
    public Animal animal;
    // the number of animals this stable can hold
    public int animalLimit;
    // whether this structure is a plant that grows into another stage
    public bool doesGrow;
    // the number of days until this structure grows into its next phase; -1
    public int growDays;
    // what this plant grows into
    public Structure nextStage;
    // the item that drops when this structure is destroyed
    public Item product;
}
