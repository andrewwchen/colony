using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    // singleton instance
    public static StructureManager Instance;
    // the -x, -z bottom left corner of the tile plot
    [SerializeField] private Transform corner;
    // the number of rows of tiles in the plot
    private static int rows = 24;
    // the number of columns of tiles in the plot
    private static int cols = 24;
    // the length and width of one tile
    private static float tileSize = .25f;
    // maps from row #, col # to the tile at the point
    private Tile[,] tiles = new Tile[rows,cols];

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
    void Start()
    {
        // initialize the tile plot
        StructureData[] data = DataManager.Instance.gameData.structures;
    }

    // gets the row and column number of the tile that contains the specified point
    public (int row, int col) GetCell(float x, float z)
    {
        int row = Mathf.FloorToInt((z - corner.position.z) % tileSize);
        int col = Mathf.FloorToInt((x - corner.position.x) % tileSize);
        return (row, col);
    }

    // checks if a tile with row and column specified is in the plot
    public bool IsCellValid(int row, int col)
    {
        return row < rows && col < cols && row >= 0 && col >= 0;
    }

    // checks if a tile that contains the specified point is in the plot
    public bool IsCellValid(float x, float z)
    {
        (int row, int col) = GetCell(x, z);
        return IsCellValid(row, col);
    }

    // gets the center point of the tile at the row and column specified
    public (float x, float z) GetCellCenter(int row, int col)
    {
        float centerZ = corner.position.z + row * tileSize + tileSize / 2;
        float centerX = corner.position.x + col * tileSize + tileSize / 2;
        return (centerX, centerZ);
    }

    // gets the center point of the tile that contains the specified point
    public (float x, float z) GetCellCenter(float x, float z)
    {
        (int row, int col) = GetCell(x, z);
        return GetCellCenter(row, col);
    }
    /*
    private StructureDirection GetStructureDirection()
    {

    }



    public GetCells(Structure structure, float x, float z)
    {

    }*/
}
