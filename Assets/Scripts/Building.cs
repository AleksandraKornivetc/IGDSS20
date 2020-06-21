using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Building : MonoBehaviour
{
    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    GameManager _gameManager;
    #endregion
    
    #region Workers
    public List<Worker> _workers; //List of all workers associated with this building, either for work or living
    #endregion

    #region Jobs
    public List<Job> _jobs; // List of all available Jobs. Is populated in Start()
    #endregion

    public int _upkeep;
    public int _constructionCostMoney;
    public int _constructionCostPlanks;
    public float _nextGenerationTick;
    public Tile _tile;
    public Tile.TileTypes[] _possibleTiles;
    public int _generationInterval;
    public float _efficiencyValue; // Calculated, so not public

    public void Start()
    {
        _nextGenerationTick = Time.time;
        _gameManager._allBuildings.Add(this);
    }

    #region Methods   
    public void WorkerAssignedToBuilding(Worker w)
    {
        _workers.Add(w);
    }

    public void WorkerRemovedFromBuilding(Worker w)
    {
        _workers.Remove(w);
    }
    public float CalculateEfficiency()
    {
        // calculate _efficiencyValue
        return _workers.Select(s => s._happiness).ToList().Average();
    }
    public float CalculateJobEfficiency()
    {
        //The efficiency of a production building depends on the number of jobs it provides and how many of those are currently occupied.
        return _jobManager._availableJobs.Where(a => a._building == this).ToList().Count / _jobs.Count;
    }
    #endregion
}
