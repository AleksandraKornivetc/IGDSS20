using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProductionBuilding : Building
{
    #region Attributes
    public string _name;
    public BuildingType _buildingType;

    public int _availableJobs;
    

    
    public int _outputCount;
    
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
        base.Start();
        // 1) When a production building is built, generate a number of Job instances and register them at the JobManager.
        var jobManager = FindObjectOfType<JobManager>();
        for (var i=0; i<=_availableJobs; i++)
        {
            var job = new Job(this);
            this._jobs.Add(job);
            jobManager._availableJobs.Add(job);
        }
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
            bool inputResourceAvailable = _inputResources.ToList().All(rt => FindObjectOfType<GameManager>().HasResourceInWarehoues(rt));

            // Every second, the efficency value is increased by a value between 0 and 1.
            // Once the building-specific generation interval has been reached, a resource is produced and the efficency value is reset.
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
