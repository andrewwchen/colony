using System.Collections.Generic;
using UnityEngine;

public class StructureManager : MonoBehaviour
{
    // singleton instance
    public static StructureManager Instance;

    // the -x, -z bottom left corner of the tile plot
    [SerializeField] private Transform corner;
    // a highlight the size of a single tile that is shown or hidden
    [SerializeField] private GameObject highlightPrefab;
    // list of all structures available in the game
    public Structure[] structures;

    // the number of rows of tiles in the plot
    private static int rows = 64;
    // the number of columns of tiles in the plot
    private static int cols = 64;
    // the length and width of one tile
    private static float tileSize = .5f;

    // maps structure types to structures
    private Dictionary<StructureType, Structure> typeToStructure = new Dictionary<StructureType, Structure>();
    // maps structures to structure types
    private Dictionary<Structure, StructureType> structureToType = new Dictionary<Structure, StructureType>();
    // maps from row #, col # to the tile at the point
    private GameObject[,] highlights = new GameObject[rows,cols];
    // keeps track of cells that are currently occupied by structures;
    private Dictionary<(int, int), StructureInstance> occupied = new Dictionary<(int, int), StructureInstance>();
    // keeps track of cells that are currently highlighted;
    private HashSet<(int, int)> highlighted = new HashSet<(int, int)>();
    // keeps track of structures existing in the scene;
    private HashSet<StructureInstance> currentStructures = new HashSet<StructureInstance>();
    // reference to the XRRig;
    private Transform rig;
    // reference to the Plot type Structure
    private Structure plotStructure;

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
        for (int i = 0; i < structures.Length; i++)
        {
            Structure structure = structures[i];
            StructureType type = structure.type;
            typeToStructure.Add(type, structure);
            structureToType.Add(structure, type);
        }

        // initializing layout based on saved data
        StructureData[] structureData = DataManager.Instance.gameData.structures;
        for (int i = 0; i < structureData.Length; i++)
        {
            StructureData data = structureData[i];
            Structure structure = typeToStructure[data.type];

            MakePlacement(structure, GetCellCenter(data.row, data.col), data.direction, data.animals);
        }

        // initialize the highlights
        float y = corner.position.z;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject go = Instantiate(highlightPrefab);
                go.transform.position = GetCellCenter(r, c);
                go.SetActive(false);
                highlights[r, c] = go;
            }
        }

        // get reference to XRRig transform
        rig = GameObject.FindGameObjectWithTag("XRRig").transform;

        // get reference to the plot structure
        plotStructure = typeToStructure[StructureType.Plot];
    }

    // gets the row and column number of the tile that contains the specified point
    private (int row, int col) GetCell(Vector3 pos)
    {
        int row = Mathf.FloorToInt((pos.z - corner.position.z) / tileSize);
        int col = Mathf.FloorToInt((pos.x - corner.position.x) / tileSize);
        return (row, col);
    }

    // checks if a tile with row and column specified is in the plot and unoccupied
    private bool IsCellValid(int row, int col)
    {
        return row < rows && col < cols && row >= 0 && col >= 0 && !occupied.ContainsKey((row, col));
    }

    // check if any of the tiles are not valid
    private bool AreCellsValid((int,int)[] cells)
    {
        foreach ((int row, int col) in cells)
        {
            if (!IsCellValid(row, col))
                return false;
        }
        return true;
    }

    // gets the center point of the tile at the row and column specified
    private Vector3 GetCellCenter(int row, int col)
    {
        float centerZ = corner.position.z + row * tileSize + tileSize / 2;
        float centerX = corner.position.x + col * tileSize + tileSize / 2;
        return new Vector3 (centerX, corner.position.y, centerZ);
    }

    // gets the center point of the list of cells
    private Vector3 GetCellsCenter((int, int)[] cells)
    {
        int minRow = int.MaxValue;
        int maxRow = int.MinValue;
        int minCol = int.MaxValue;
        int maxCol = int.MinValue;

        foreach ((int r, int c) in cells)
        {
            minRow = Mathf.Min(minRow, r);
            maxRow = Mathf.Max(maxRow, r);
            minCol = Mathf.Min(minCol, c);
            maxCol = Mathf.Max(maxCol, c);
        }
        float avgRow = minRow + (maxRow - minRow) / 2;
        float avgCol = minCol + (maxCol - minCol) / 2;

        float centerZ = corner.position.z + avgRow * tileSize + tileSize / 2;
        float centerX = corner.position.x + avgCol * tileSize + tileSize / 2;
        return new Vector3(centerX, corner.position.y, centerZ);
    }

    // Gets the direction that a structure should be facing based on the player's orientation
    private StructureDirection GetStructureDirection()
    {
        if (Vector3.Angle(rig.forward, Vector3.forward) <= 45)
            return StructureDirection.South;
        else if (Vector3.Angle(rig.forward, Vector3.right) <= 45)
            return StructureDirection.West;
        else if (Vector3.Angle(rig.forward, -Vector3.forward) <= 45)
            return StructureDirection.North;
        else
            return StructureDirection.East;
    }

    // gets all the cells a building may occupy if it is placed at the specified row and column
    private (int row, int col)[] GetCells(Structure structure, int row, int col, StructureDirection? direction = null)
    {
        (int, int)[] cells = new (int, int)[structure.rows * structure.cols];
        StructureDirection dir = direction ?? GetStructureDirection();
        int cornerRow;
        int cornerCol;
        int numRows;
        int numCols;
        switch (dir)
        {
            case StructureDirection.South:
                cornerRow = row;
                cornerCol = col;
                numRows = structure.rows;
                numCols = structure.cols;
                break;
            case StructureDirection.West:
                cornerRow = row - structure.cols + 1;
                cornerCol = col;
                numRows = structure.cols;
                numCols = structure.rows;
                break;
            case StructureDirection.North:
                cornerRow = row - structure.rows + 1;
                cornerCol = col - structure.cols + 1;
                numRows = structure.rows;
                numCols = structure.cols;
                break;
            default:
                cornerRow = row;
                cornerCol = col - structure.rows + 1;
                numRows = structure.cols;
                numCols = structure.rows;
                break;
        }
        int i = 0;
        for (int r = cornerRow; r < cornerRow + numRows; r++)
        {
            for (int c = cornerCol; c < cornerCol + numCols; c++)
            {
                cells[i] = (r, c);
                i++;
            }
        }
        return cells;
    }

    // remove all tile highlights
    public void Unhover()
    {
        foreach ((int r, int c) in highlighted)
            highlights[r, c].SetActive(false);
        highlighted.Clear();
    }

    // produce tile highlights for structure placement
    public void HoverPlacement(Structure structure, Vector3 pos)
    {
        Unhover();
        (int row, int col) = GetCell(pos);
        (int, int)[] cells = GetCells(structure, row, col);
        bool canPlace = AreCellsValid(cells);

        foreach ((int r, int c) in cells)
        {
            if (IsCellValid(r,c))
            {
                highlights[r, c].SetActive(true);
                highlighted.Add((r, c));
            }
        }
    }

    // place the structure at a position if able
    public bool MakePlacement(Structure structure, Vector3 pos, StructureDirection? direction = null, AnimalData[] animals = null, PlantData plant = null)
    {
        Unhover();
        (int row, int col) = GetCell(pos);
        (int, int)[] cells = GetCells(structure, row, col, direction);
        bool canPlace = AreCellsValid(cells);

        if (!canPlace)
            return false;

        Vector3 center = GetCellsCenter(cells);

        GameObject go = Instantiate(structure.prefab);
        go.transform.position = center;
        StructureInstance si = go.GetComponent<StructureInstance>();
        si.Setup(structure, direction ?? GetStructureDirection(), row, col, cells, animals, plant);

        foreach ((int, int) c in cells)
            occupied.Add(c, si);

        currentStructures.Add(si);

        return true;
    }

    // produce tile highlights for structure removal
    public void HoverRemoval(Vector3 pos)
    {
        Unhover();
        (int, int) cell = GetCell(pos);
        if (!occupied.ContainsKey(cell))
            return;

        StructureInstance si = occupied[cell];

        if (!si.CanRemove())
            return;

        foreach ((int r, int c) in si.occupied)
        {
            highlights[r, c].SetActive(true);
            highlighted.Add((r, c));
        }
    }

    // remove the structure from a position if able
    public bool MakeRemoval(Vector3 pos)
    {
        Unhover();
        (int, int) cell = GetCell(pos);
        if (!occupied.ContainsKey(cell))
            return false;

        StructureInstance si = occupied[cell];

        if (!si.CanRemove())
            return false;

        foreach ((int, int) c in si.occupied)
            occupied.Remove(c);

        currentStructures.Remove(si);

        Destroy(si.gameObject);
        return true;
    }

    // produce tile highlights for seed planting
    public void HoverPlant(Vector3 pos)
    {
        Unhover();
        (int, int) cell = GetCell(pos);
        if (!occupied.ContainsKey(cell))
            return;

        StructureInstance si = occupied[cell];

        if (!si.CanPlant())
            return;

        foreach ((int r, int c) in si.occupied)
        {
            highlights[r, c].SetActive(true);
            highlighted.Add((r, c));
        }
    }

    // plant the plant at the plot if able
    public bool MakePlant(Plant plant, Vector3 pos)
    {
        Unhover();
        (int, int) cell = GetCell(pos);
        if (!occupied.ContainsKey(cell))
            return false;

        StructureInstance si = occupied[cell];

        if (!si.CanPlant())
            return false;

        PlantType type = PlantManager.Instance.plantToType[plant];

        PlantData pd = new PlantData(type, 0);

        return si.MakePlant(pd);
    }

    public void HoverTill(Vector3 pos)
    {
        HoverPlacement(plotStructure, pos);
    }

    public bool MakeTill(Vector3 pos)
    {
        return MakePlacement(plotStructure, pos);
    }

    public void HoverWater(Vector3 pos)
    {
        Unhover();
        (int, int) cell = GetCell(pos);
        if (!occupied.ContainsKey(cell))
            return;

        StructureInstance si = occupied[cell];

        if (!si.CanWaterPlot())
            return;

        foreach ((int r, int c) in si.occupied)
        {
            highlights[r, c].SetActive(true);
            highlighted.Add((r, c));
        }
    }

    public bool MakeWater(Vector3 pos)
    {
        Unhover();
        (int, int) cell = GetCell(pos);
        if (!occupied.ContainsKey(cell))
            return false;

        StructureInstance si = occupied[cell];

        if (!si.CanWaterPlot())
            return false;

        return si.WaterPlot();
    }



    // converts inventory into a serializable format for data saving
    public StructureData[] Serialize()
    {
        StructureData[] structureData = new StructureData[currentStructures.Count];
        int i = 0;
        foreach (StructureInstance si in currentStructures)
        {
            structureData[i] = new StructureData(si);
            i += 1;
        }
        return structureData;
    }
}
