using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    #region Attributes
    public string _name;
    public BuildingType _buildingType;
    public int _upkeep;
    public int _constructionCostMoney;
    public int _constructionCostPlanks;
    public Tile _tile;

    float _efficiencyValue; // Calculated, so not public
    float _nextGenerationTick;

    public int _generationInterval;
    public int _outputCount;
    public Tile.TileTypes[] _possibleTiles;
    public Tile.TileTypes _scalingNeighborTile; // Does not scale if null
    [Range(0, 6)]
    public int _minScalingNeighborTiles;
    [Range(0, 6)]
    public int _maxScalingNeighborTiles;
    public GameManager.ResourceTypes[] _inputResources;
    public GameManager.ResourceTypes _outputResource;
    #endregion

    private void Start()
    {
        _nextGenerationTick = Time.time;
    }

    private void Update()
    {
        // Update every second
        if (Time.time > _nextGenerationTick)
        {
            _nextGenerationTick++;
            // Count relevant neighbor tiles. Developed tiles _do_ count towards this.
            int numRelevantNeighbors = _tile._neighborTiles.FindAll(t => t._type == _scalingNeighborTile).Count;
            // If the number of relevant neighbor tiles match or exceed the minimum, add the proportial progress to the efficiency value
            // On the other hand, if the maximum number of relevant neighbor tiles is 0, just add 1 to the efficency value
            _efficiencyValue +=
                _maxScalingNeighborTiles == 0
                ? 1 : numRelevantNeighbors >= _minScalingNeighborTiles
                ? (float)Mathf.Min(numRelevantNeighbors, _maxScalingNeighborTiles) / (float)_maxScalingNeighborTiles : 0;
            // Check if there is at least one of each required input resource in the warehouse
            bool inputResourceAvailable = true;
            foreach (GameManager.ResourceTypes rt in _inputResources)
            {
                if (!FindObjectOfType<GameManager>().HasResourceInWarehoues(rt))
                {
                    inputResourceAvailable = false;
                    break;
                }
            }
            // Check if the generation interval has been reached
            if (_efficiencyValue > _generationInterval && inputResourceAvailable)
            {
                // Set back the efficiency value with each generation
                _efficiencyValue -= _generationInterval;
                FindObjectOfType<GameManager>().ModifyWarehouseResource(_outputResource, _outputCount);

            }
        }
    }


    #region Enumerations
    public enum BuildingType { Empty, Fishery, Lumberjack, Sawmill, SheepFarm, FrameworkKnitters, PotatoFarm, SchnappsDistillery };
    #endregion

}
