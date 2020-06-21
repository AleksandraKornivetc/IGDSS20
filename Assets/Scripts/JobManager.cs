using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class JobManager : MonoBehaviour
{

    public List<Job> _availableJobs = new List<Job>();
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
        _availableJobs.ForEach(a => {
            if (_unoccupiedWorkers.Count > 0)
                {
                    a.AssignWorker(_unoccupiedWorkers.FirstOrDefault());
                    _unoccupiedWorkers.RemoveAt(0);
                }
            });
    }

    public void RegisterWorker(Worker w)
    {
        _unoccupiedWorkers.Add(w);
    }



    public void RemoveWorker(Worker w)
    {
        _unoccupiedWorkers.Remove(w);
    }

    #endregion
}
