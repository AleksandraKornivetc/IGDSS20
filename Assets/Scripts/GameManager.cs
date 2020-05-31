using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region Map generation
    public Texture2D heightmap;
    private Tile[,] _tileMap; //2D array of all spawned tiles
    public GameObject[] tiles; // Array of all available tile prefabs
    public float terrainExtremity; // Determines how pronounced differences of the heightmap are
    float tileWidth;
    float tileHeight;
    int width;
    int height;
    #endregion

    #region Buildings
    public GameObject[] _buildingPrefabs; //References to the building prefabs
    public int _selectedBuildingPrefabIndex = 0; //The current index used for choosing a prefab to spawn from the _buildingPrefabs list
    #endregion


    #region Resources
    private Dictionary<ResourceTypes, float> _resourcesInWarehouse = new Dictionary<ResourceTypes, float>(); //Holds a number of stored resources for every ResourceType

    //A representation of _resourcesInWarehouse, broken into individual floats. Only for display in inspector, will be removed and replaced with UI later
    [SerializeField]
    private float _ResourcesInWarehouse_Fish;
    [SerializeField]
    private float _ResourcesInWarehouse_Wood;
    [SerializeField]
    private float _ResourcesInWarehouse_Planks;
    [SerializeField]
    private float _ResourcesInWarehouse_Wool;
    [SerializeField]
    private float _ResourcesInWarehouse_Clothes;
    [SerializeField]
    private float _ResourcesInWarehouse_Potato;
    [SerializeField]
    private float _ResourcesInWarehouse_Schnapps;
    #endregion

   
    #region Enumerations
    public enum ResourceTypes { None, Fish, Wood, Planks, Wool, Clothes, Potato, Schnapps }; //Enumeration of all available resource types. Can be addressed from other scripts by calling GameManager.ResourceTypes
    #endregion

    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {
        tileWidth = tiles[0].GetComponent<Renderer>().bounds.size.z;
        tileHeight = tiles[0].GetComponent<Renderer>().bounds.size.x;
        width = heightmap.width;
        height = heightmap.height;
        _tileMap = new Tile[width, height];
        GenerateMap();
        Debug.Log(_tileMap);
        // Horizontal bounds can only be calculated after map generation
        FindObjectOfType<MouseManager>().DetermineHorizontalBounds();
        PopulateResourceDictionary();
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
        UpdateInspectorNumbersForResources();
    }
    #endregion

    #region Methods
    //Makes the resource dictionary usable by populating the values and keys
    void PopulateResourceDictionary()
    {
        _resourcesInWarehouse.Add(ResourceTypes.None, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Fish, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Wood, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Planks, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Wool, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Clothes, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Potato, 0);
        _resourcesInWarehouse.Add(ResourceTypes.Schnapps, 0);
    }

    //Sets the index for the currently selected building prefab by checking key presses on the numbers 1 to 0
    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _selectedBuildingPrefabIndex = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _selectedBuildingPrefabIndex = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _selectedBuildingPrefabIndex = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _selectedBuildingPrefabIndex = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _selectedBuildingPrefabIndex = 4;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _selectedBuildingPrefabIndex = 5;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _selectedBuildingPrefabIndex = 6;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _selectedBuildingPrefabIndex = 7;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _selectedBuildingPrefabIndex = 8;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _selectedBuildingPrefabIndex = 9;
        }
    }

    //Updates the visual representation of the resource dictionary in the inspector. Only for debugging
    void UpdateInspectorNumbersForResources()
    {
        _ResourcesInWarehouse_Fish = _resourcesInWarehouse[ResourceTypes.Fish];
        _ResourcesInWarehouse_Wood = _resourcesInWarehouse[ResourceTypes.Wood];
        _ResourcesInWarehouse_Planks = _resourcesInWarehouse[ResourceTypes.Planks];
        _ResourcesInWarehouse_Wool = _resourcesInWarehouse[ResourceTypes.Wool];
        _ResourcesInWarehouse_Clothes = _resourcesInWarehouse[ResourceTypes.Clothes];
        _ResourcesInWarehouse_Potato = _resourcesInWarehouse[ResourceTypes.Potato];
        _ResourcesInWarehouse_Schnapps = _resourcesInWarehouse[ResourceTypes.Schnapps];
    }

    //Checks if there is at least one material for the queried resource type in the warehouse
    public bool HasResourceInWarehoues(ResourceTypes resource)
    {
        return _resourcesInWarehouse[resource] >= 1;
    }

    //Is called by MouseManager when a tile was clicked
    //Forwards the tile to the method for spawning buildings
    public void TileClicked(int height, int width)
    {
        Tile t = _tileMap[width, height];

        PlaceBuildingOnTile(t);
    }

    //Checks if the currently selected building type can be placed on the given tile and then instantiates an instance of the prefab
    private void PlaceBuildingOnTile(Tile t)
    {
        //if there is building prefab for the number input
        if (_selectedBuildingPrefabIndex < _buildingPrefabs.Length)
        {
            //TODO: check if building can be placed and then istantiate it

        }
    }

    //Returns a list of all neighbors of a given tile
    private List<Tile> FindNeighborsOfTile(Tile t)
    {
        List<Tile> result = new List<Tile>();
        int xOffset = 0;
        if (t._coordinateHeight % 2 != 0)
            xOffset = 1;

        // On the same row.
        if (t._coordinateWidth - 1 >= 0)
            result.Add(_tileMap[t._coordinateWidth-1, t._coordinateHeight]);
        if (t._coordinateWidth + 1 <= width-1)
            result.Add(_tileMap[t._coordinateWidth + 1, t._coordinateHeight]);

        // 2 in row above
        if (t._coordinateWidth + xOffset - 1 >= 0 && t._coordinateHeight + 1 <= height-1)
            result.Add(_tileMap[t._coordinateWidth + xOffset - 1, t._coordinateHeight + 1]);
        if (t._coordinateWidth + xOffset <= width-1 && t._coordinateHeight + 1 <= height-1)
            result.Add(_tileMap[t._coordinateWidth + xOffset, t._coordinateHeight + 1]);

        // 2 in row below
        if (t._coordinateWidth + xOffset - 1 >= 0 && t._coordinateHeight - 1 >=0)
            result.Add(_tileMap[t._coordinateWidth + xOffset - 1, t._coordinateHeight - 1]);
        if (t._coordinateHeight - 1 >= 0 && t._coordinateWidth + xOffset <= width-1)
        result.Add(_tileMap[t._coordinateWidth + xOffset, t._coordinateHeight - 1]);

        // Debug.Log(t._type + " h " + t._coordinateHeight + " w " + t._coordinateWidth);

        return result;
    }

    private void GenerateMap()
    {
        if (heightmap == null || tiles == null || tiles.Length == 0)
        {
            Debug.Log("Aborting, references not configured");
            return;
        }
        int w = heightmap.width;
        int h = heightmap.height;
        for (int i = 0; i < h; i++)
        {
            for (int j = 0; j < w; j++)
            {
                float grey = heightmap.GetPixel(i, j).maxColorComponent;
                int tileIndex;

                if (grey > 0.8) tileIndex = 6;
                else if (grey > 0.6) tileIndex = 5;
                else if (grey > 0.4) tileIndex = 4;
                else if (grey > 0.2) tileIndex = 3;
                else if (grey > 0) tileIndex = 2;
                else tileIndex = 1;

                Vector3 newTileTransform = new Vector3(
                    // static value 0.75f should be calculated based on tile height and width instead, but this works too.
                    i * tileHeight * 0.75f,
                    grey * terrainExtremity,
                    j * tileWidth + (i % 2 * tileWidth / 2));

                GameObject newTile = Instantiate(tiles[tileIndex]) as GameObject;
                newTile.transform.Translate(newTileTransform, Space.World);
                Tile tile = new Tile();
                tile._coordinateHeight = i;
                tile._coordinateWidth = j;
                tile._type = (Tile.TileTypes)tileIndex;
                _tileMap[j, i] = tile;
            }
        }
        foreach(var tile in _tileMap)
        {
            tile._neighborTiles = FindNeighborsOfTile(tile);
        }
    }
    #endregion
}
