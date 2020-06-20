using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Worker : MonoBehaviour
{
    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    GameManager _gameManager;//Reference to the GameManager
    #endregion

    [Range(0, 100)]
    public float _age; // The age of this worker
    [Range(0,100)]
    public float _happiness; // The happiness of this worker

    public int _consumptionInterval; // Seconds between metabolism ticks
    public int _agingInterval; // Seconds between aging a year

    public int _adultAge = 14;
    public int _retirementAge = 64;

    bool _isOfAge = false;
    bool _isRetired = false;

    float _nextConsumptionTick;
    float _nextAgingTick;

    // Once reached, consume one coresponding consumable item
    // The integers can be regarded as factors to achieve different weights between consumable resource types.
    // For example, a resource with a value of 1 would be required every consumption tick,
    // whereas a resource with a value of 3 would only be required every third tick.
    Dictionary<GameManager.ResourceTypes, int> _consumables;
    // Holds temporary status values
    Dictionary<GameManager.ResourceTypes, int> _needForConsumables;
   
    // Start is called before the first frame update
    void Start()
    {
        _nextConsumptionTick = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(_jobManager == null) _jobManager = FindObjectOfType<JobManager>();
        if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();

        if(Time.time > _nextAgingTick)
        {
            _nextAgingTick += _agingInterval;
            Age();

        }

        if(Time.time > _nextConsumptionTick)
        {
            _nextConsumptionTick += _consumptionInterval;
            Consume();

        }

    }

    // Consume resource
    void Consume()
    {
        // Check every resource with each consumption tick
        foreach (KeyValuePair<GameManager.ResourceTypes, int> entry in _consumables)
        {
            // Is the need for the current consumable reached?
            if(_needForConsumables[entry.Key] >= entry.Value)
            {
                // Is the current consumable available?
                bool inputResourceAvailable = FindObjectOfType<GameManager>().HasResourceInWarehoues(entry.Key);
                // Reset need, increase happiness and decrease resource total if so
                if (inputResourceAvailable)
                {
                    _happiness++;
                    FindObjectOfType<GameManager>().ModifyWarehouseResource(entry.Key, -1);
                    _needForConsumables[entry.Key] = 0;
                }
                // Get unhappy if not
                else
                {
                    _happiness--;
                }
            }
            // Increase need if limit is not yet reached
            else
            {
                _needForConsumables[entry.Key]++;
            }
        }
    }


    // As we are using a pooler, the attributes of the worker are reset when a GameObject is being reused
    public void Rebirth()
    {
        _age = 0;
        _happiness = 50;
        _nextConsumptionTick = Time.time;
        _nextAgingTick = Time.time;
        _isOfAge = false;
        _isRetired = false;
        SetupNeeds();

        _needForConsumables.ToList().ForEach(kvp => _needForConsumables[kvp.Key] = 0);
    }

    // Dictonairies cannot be configured in the Unity editor, so we do it here.
    public void SetupNeeds()
    {

        _consumables = new Dictionary<GameManager.ResourceTypes, int>();
        _consumables.Add(GameManager.ResourceTypes.Clothes, 3);
        _consumables.Add(GameManager.ResourceTypes.Fish, 1);

        _needForConsumables = new Dictionary<GameManager.ResourceTypes, int>();
        _needForConsumables.Add(GameManager.ResourceTypes.Clothes, 0);
        _needForConsumables.Add(GameManager.ResourceTypes.Fish, 0);
    }

    private void Age()
    {
        _age++;
        DeathCheck();
         
        if (!_isOfAge && _age > _adultAge) BecomeOfAge();
        if (!_isRetired && _age > _retirementAge) Retire();
        if (_age > 100) Die();
    }


    public void BecomeOfAge()
    {
        _isOfAge = true;
        _jobManager.RegisterWorker(this);
        _consumables.Add(GameManager.ResourceTypes.Schnapps, 2);
        _needForConsumables.Add(GameManager.ResourceTypes.Schnapps, 0);

    }

    private void Retire()
    {
        _isRetired = true;
        _jobManager.RemoveWorker(this);
    }

    private void Die()
    {
        gameObject.SetActive(false);
        // Unregister from job
        _jobManager.RemoveWorker(this);

    }

    private void DeathCheck()
    {
        // Every year, there is a chance for a worker to die.
        // These chances are modelled based on UK mortality rates for men, see http://www.bandolier.org.uk/booth/Risk/dyingage.html
        // Our island is a bit more harsh than the UK, so the risk of death is increased by a factor.
        int harshnessFactor = 3;
        int risk;

        risk = _age < 4 ? 177
            : _age < 14 ? 4386
            : _age < 24 ? 8333
            : _age < 34 ? 1908
            : _age < 14 ? 1215
            : _age < 44 ? 663
            : _age < 54 ? 279
            : _age < 64 ? 112
            : _age < 74 ? 42
            : _age < 84 ? 15
            : 6;

        if (harshnessFactor/risk > Random.Range(0, 1))
        {
            Die();
        }
    }
}
