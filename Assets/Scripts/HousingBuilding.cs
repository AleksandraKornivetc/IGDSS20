using UnityEngine;
using System.Collections;
using System.Linq;

public class HousingBuilding : Building
{
    const int _maxWorkesrs = 10;
    // Use this for initialization
    float _startTime;
    void Start()
    {
        base.Start();
        _startTime = Time.time;
        _workers.Add(new Worker());
        _workers.Add(new Worker());
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _nextGenerationTick)
        {
            _nextGenerationTick++;
            GetOffspring();

        }
    }
    //The Residence can spawn and hold up to 10 workers at a time, and spawns 2 grown workers when built.
    //At 100% efficiency, a new child worker is spawned every 30 seconds.The efficiency should depend on the average happiness of the workers living there.
    private bool CheckFreeOccupancy()
    {
        if (this._workers.Count > 10) return false;
        return true;
    }
   
    private void GetOffspring()
    {
        if((Time.time - _startTime) % 30 == 0 && CalculateEfficiency() == 100 && CheckFreeOccupancy())
            {
                this._workers.Add(new Worker());
            }
    }
}
