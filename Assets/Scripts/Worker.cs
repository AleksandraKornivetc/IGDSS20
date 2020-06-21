using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Worker : MonoBehaviour
{
    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    GameManager _gameManager;//Reference to the GameManager
    #endregion

    public float _age; // The age of this worker
    public float _happiness; // The happiness of this worker
    public GameManager.ResourceTypes[] _consumtion;
    float _birthday;
    public int _fixWorkerCost = 1;
    // Start is called before the first frame update
    void Start()
    {
        _age = 0;
        _birthday = Time.time;

    }

    // Update is called once per frame
    void Update()
    {
        Age();
        bool happy = Consume();
        BeHappy(happy);
    }

    private bool Consume()
    {
        bool availbale = _consumtion.ToList().All(rt => _gameManager.HasResourceInWarehoues(rt));
        if(availbale)
        {
            _gameManager.ModifyWarehouseResource(GameManager.ResourceTypes.Fish, -1);
            _gameManager.ModifyWarehouseResource(GameManager.ResourceTypes.Clothes, -1);
            _gameManager.ModifyWarehouseResource(GameManager.ResourceTypes.Schnapps, -1);
            return true;
        }
        return false;
    }
    private void BeHappy(bool happy)
    {
        if(happy && !_jobManager._unoccupiedWorkers.Contains(this))
        {
            if(_happiness<100) _happiness++;
        }
        else
        {
            if(_happiness > 0) _happiness--;
        }
    }
    private void Age()
    {
        //TODO: Implement a life cycle, where a Worker ages by 1 year every 15 real seconds.
        //When becoming of age, the worker enters the job market, and leaves it when retiring.
        //Eventually, the worker dies and leaves an empty space in his home. His Job occupation is also freed up.

        if ((Time.time - _birthday) % 15 == 0.0f)
        {
            _age++;
            if (_age > 14)
            {
                BecomeOfAge();
            }

            if (_age > 64)
            {
                Retire();
            }

            if (_age > 100)
            {
                Die();
            }
        }
    }
   //It contains a basic structure for a life cycle and interactions with the JobManager.
   //Implement the aging logic, periodic consumption of resources (fish, clothes & schnapps) and a calculation for a worker's happiness value.
   //It should be based on being supplied with resources and having a job.

    public void BecomeOfAge()
    {
        _jobManager.RegisterWorker(this);
    }

    private void Retire()
    {
        _jobManager.RemoveWorker(this);
    }

    private void Die()
    {
        MakeTheJobAvailable();
        Destroy(this.gameObject, 1f);
    }
    private void MakeTheJobAvailable()
    {
        //3) Handle worker deaths: their occupied jobs become available again
        _gameManager._allBuildings.ForEach(b =>
        {
            var job = b._jobs.Where(j => j._worker == this).FirstOrDefault();
            job._worker = null;
            _jobManager._availableJobs.Add(job);
        });
    }
}
