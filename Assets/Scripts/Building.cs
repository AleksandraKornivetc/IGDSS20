using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Building : MonoBehaviour
{
    public string _Type; //The name of the building
    public float _Upkeep; //The money cost per minute
    public float _BuildCostMoney; //placement money cost
    public float _BuildCostPlanks; // placement planks cost
    public Tile _Tile; //Reference to the tile it is built on
    public float _Efficiency; //Calculated based on the surrounding tile types
    public float _ResourceGenerationInterval; //If operating at 100% efficiency, this is the time in seconds it takes for one production cycle to finish
    public int _OutputCount; //The number of output resources per generation cycle(for example the Sawmill produces 2 planks at a time)
    public Tile.TileTypes[] _CanBuiltOnTileTypes; // A restriction on which types of tiles it can be placed on
    public Tile.TileTypes _EfficiencyScalesNeighbor; //A choice if its efficiency scales with a specific type of surrounding tile
    public int _MinNeighbors; //The minimum and maximum number of surrounding tiles its efficiency scales with(0-6)
    public int _MaxNeighbors;
    public GameManager.ResourceTypes[] _InputRes; //A choice for input resource types(0, 1 or 2 types)
    public GameManager.ResourceTypes _OutputRes; //A choice for output resource type

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // However, based on the surrounding tiles this process can take longer.
        // If not enough required empty tiles are available, its efficiency is reduced.
        // For example, if the maximum of surrounding forest tiles is 4, and the actual count is 3, then it will operate at 75 % efficiency.
        // If the count is 1, then it will operate at 25 % efficiency.However, if the count is below the defined minimum, efficiency is set to 0 % instead.
        // An efficiency value of 75 % translates as following: for every second passed, the production cycle is progressed by a value of 0.75.
        var neightbours = _Tile._neighborTiles.FindAll(t => t._type == _EfficiencyScalesNeighbor).Count;
        if (neightbours > _MinNeighbors)
        {
            _Efficiency = neightbours / _MaxNeighbors;
        }
        else
        {
            _Efficiency = 0;
        }
        // In each cycle, the production building retrieves the required resources from the warehouse(1 each).
        // It takes the production building one generation cycle interval in seconds to generate its output.
        bool availableRes = true;
        Array.ForEach<GameManager.ResourceTypes>(_InputRes, res => availableRes = availableRes && FindObjectOfType<GameManager>().HasResourceInWarehoues(res));
        // as _ResourceGenerationInterval is the time for building on 100% effeciency 
        var requiredTime = _Efficiency * _ResourceGenerationInterval + _ResourceGenerationInterval;
        if (availableRes)
        {
            // how to deal with the time???
            FindObjectOfType<GameManager>().AddNewRes(_OutputRes, _OutputCount); 
        }

    }
}
