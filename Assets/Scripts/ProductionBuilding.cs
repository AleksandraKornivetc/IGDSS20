using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ProductionBuilding : Building
{
    #region Attributes
    public BuildingType _buildingType;
    public int _upkeep;
    public int _jobCount;

    float _efficiencyValue;
    float _nextGenerationTick;

    public int _generationInterval;
    public int _outputCount;
    public Tile.TileTypes _scalingNeighborTile; // Does not scale if null
    [Range(0, 6)]
    public int _minScalingNeighborTiles;
    [Range(0, 6)]
    public int _maxScalingNeighborTiles;
    public GameManager.ResourceTypes[] _inputResources;
    public GameManager.ResourceTypes _outputResource;
    #endregion

    protected override void Start()
    {
        base.Start();
        _nextGenerationTick = Time.time;
        InitJobs();
    }

    protected override void Update()
    {
        base.Update();
        // Update every second
        if (Time.time > _nextGenerationTick)
        {
            _nextGenerationTick++;
            // Count relevant neighbor tiles. Developed tiles _do_ count towards this.
            int numRelevantNeighbors = _tile._neighborTiles.FindAll(t => t._type == _scalingNeighborTile).Count;

            // If the number of relevant neighbor tiles match or exceed the minimum, add the proportial progress to the efficiency value
            // On the other hand, if the maximum number of relevant neighbor tiles is 0, just add 1 to the efficency value
            float _efficiencyGain =
                _maxScalingNeighborTiles == 0
                ? 1 : numRelevantNeighbors >= _minScalingNeighborTiles
                ? (float)Mathf.Min(numRelevantNeighbors, _maxScalingNeighborTiles) / (float)_maxScalingNeighborTiles : 0;

            // The efficiency gain is then further modified by average worker happiness
            // To avoid stalling, the maximum effect of unhappiness is capped at 1/4
            float happinessRatio = GetAverageHappiness() < 25 ? 0.25f : GetAverageHappiness() / 100;
            // Lastly, the percentage of occupied jobs factors into the efficiency gain
            float employmentRatio = _workers.Count / _jobCount;
            
            _efficiencyValue += _efficiencyGain * employmentRatio * happinessRatio;
            // Check if there is at least one of each required input resource in the warehouse
            bool inputResourceAvailable = _inputResources.ToList().All(rt => _gameManager.HasResourceInWarehoues(rt));

            // Every second, the efficency value is increased by a value between 0 and 1.
            // Once the building-specific generation interval has been reached, a resource is produced and the efficency value is reset.
            // Check if the generation interval has been reached
            if (_efficiencyValue > _generationInterval && inputResourceAvailable)
            {
                // Set back the efficiency value with each generation
                _efficiencyValue -= _generationInterval;
                Debug.Log(_outputResource);
                _gameManager.ModifyWarehouseResource(_outputResource, _outputCount);

            }
        }
    }

    private void InitJobs()
    {
        _jobs = new List<Job>();
        for(int i = 0; i < _jobCount; i++)
        {
            Job j = new Job(this);
            _jobs.Add(j);
            FindObjectOfType<JobManager>().AddJobToAvailable(j);
        }
    }


    #region Enumerations
    public enum BuildingType { Empty, Fishery, Lumberjack, Sawmill, SheepFarm, FrameworkKnitters, PotatoFarm, SchnappsDistillery };
    #endregion

}
