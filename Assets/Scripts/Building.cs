using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public abstract class Building : MonoBehaviour
{
    public string _name;
    public int _constructionCostMoney;
    public int _constructionCostPlanks;
    public Tile _tile;
    public Tile.TileTypes[] _possibleTiles;


    #region Manager References
    protected GameManager _gameManager;
    JobManager _jobManager; //Reference to the JobManager
    #endregion
    
    #region Workers
    public List<Worker> _workers; //List of all workers associated with this building, either for work or living
    #endregion

    #region Jobs
    public List<Job> _jobs; // List of all available Jobs. Is populated in Start()
    #endregion


    #region Methods   

    // Start is called before the first frame update
    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
    }

    public void WorkerAssignedToBuilding(Worker w)
    {
        _workers.Add(w);
    }

    public void WorkerRemovedFromBuilding(Worker w)
    {
        _workers.Remove(w);
    }

    // Efficiency is defined as the average happiness of all inhabitants
    protected float GetAverageHappiness()
    {
        return _workers.Count == 0 ? 0 : _workers.ToList().Where(w => w !=null).Select(w => w._happiness).DefaultIfEmpty(0f).Average();
    }

    #endregion
}
