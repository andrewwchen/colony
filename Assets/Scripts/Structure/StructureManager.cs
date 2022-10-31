using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    public static int rows = 16;
    public static int cols = 16;
    public Tile[,] tiles = new Tile[rows,cols];
    public StructureData[] structures;

    // Start is called before the first frame update
    void Start()
    {
        structures = DataManager.Instance.gameData.structures;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
