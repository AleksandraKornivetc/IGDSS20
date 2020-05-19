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


    #region Enumerations
    public enum BuildingType { Empty, Fishery, Lumberjack, Sawmill, SheepFarm, FrameworkKnitters, PotatoFarm, SchnappsDistillery };
    #endregion

}
