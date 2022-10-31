using UnityEngine;

[CreateAssetMenu(fileName = "Structure", menuName = "ScriptableObjects/Structure")]
public class Structure : ScriptableObject
{
    // prefab that is instantiated when the structure is placed
    public GameObject prefab;
    // the number of columns the structure takes up
    public int cols;
    // the number of rows the structure takes up
    public int rows;
}
