using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class JobManager : MonoBehaviour
{

    private List<Job> _availableJobs = new List<Job>();
    public List<Worker> _unoccupiedWorkers = new List<Worker>();

    #region MonoBehaviour
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleUnoccupiedWorkers();
    }
    #endregion


    #region Methods

    private void HandleUnoccupiedWorkers()
    {
        while(_unoccupiedWorkers.Count > 0 && _availableJobs.Count > 0)
        {
            Job j = _availableJobs.First();
            Worker w = _unoccupiedWorkers.First();
            j.AssignWorker(w);
            _availableJobs.Remove(j);
            _unoccupiedWorkers.Remove(w);
        }
    }

    public void RegisterWorker(Worker w)
    {
        _unoccupiedWorkers.Add(w);
    }



    public void RemoveWorker(Worker w)
    {
        _unoccupiedWorkers.Remove(w);
        _availableJobs.ToList().Where(j => j._worker == w).ToList().ForEach(j => j._worker = null);
    }
    
    public void AddJobToAvailable(Job j)
    {
        _availableJobs.Add(j);
    }

    #endregion
}
