using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HousingBuilding : Building
{

    public int _childSpawnInterval;

    int _nextChildSpawnTick;

    protected override void Start()
    {
        base.Start();
        _nextChildSpawnTick = (int) Time.time;

        // Creating a building creates 2 new workers
        // The workers start as adults
        for (int i = 0; i < 2; i++)
        {
            _workers.Add(WorkerPooler.instance.GetWorker(16));
        }
    }

    protected override void Update()
    {
        base.Update();
        //New instructions can be called after the base's.
    }

    // Attempt to spawn child
    private void SpawnChild()
    {
        if(Time.time > _nextChildSpawnTick)
        {
            _nextChildSpawnTick += _childSpawnInterval;
            if(GetAverageHappiness() >= 100 && _workers.Count < 10) _workers.Add(WorkerPooler.instance.GetWorker(0));
        }

    }


}
