using UnityEngine;

[CreateAssetMenu(fileName = "Plant", menuName = "ScriptableObjects/Plant")]
public class Plant : ScriptableObject
{
    // type of the plant
    public PlantType type;
    // prefab that is instantiated on top of the tilled field
    public GameObject prefab;
    // whether this plant grows into another stage
    public bool doesGrow;
    // whether this plant requires water to grow into another stage
    public bool needsWater;
    // the number of days until this structure grows into its next phase
    public int growDays;
    // what this plant grows into
    public PlantType nextStage;
    // the item that this plant produces when destroyed
    public Item product;
}
