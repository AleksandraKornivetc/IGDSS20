using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Worker : MonoBehaviour
{
    #region Manager References
    JobManager _jobManager; //Reference to the JobManager
    GameManager _gameManager; //Reference to the GameManager
    NavigationManager _navigationManager; //Reference to the NavigationManager
    #endregion

    // Movement
    public int _movementSpeed = 10; // The speed with which the worker moves between tiles
    private Queue<Tile> _pathToWalk;
    public HousingBuilding _home;
    public ProductionBuilding _workplace;
    private bool _enRoute;
    public float _timeUntilDeparture = 5; // Number of seconds to stay both at home and at work
    private float _nextDepartureTick;


    // Lifecycle
    [Range(0, 100)]
    public float _age; // The age of this worker
    [Range(0, 100)]
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
        if (_jobManager == null) _jobManager = FindObjectOfType<JobManager>();
        if (_gameManager == null) _gameManager = FindObjectOfType<GameManager>();
        if (_navigationManager == null) _navigationManager = FindObjectOfType<NavigationManager>();

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

        StartWalkingIfPossible();
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
                    Mathf.Clamp(_happiness, 0, 100);
                    FindObjectOfType<GameManager>().ModifyWarehouseResource(entry.Key, -1);
                    _needForConsumables[entry.Key] = 0;
                }
                // Get unhappy if not
                else
                {
                    _happiness--;
                    Mathf.Clamp(_happiness, 0, 100);
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
        // Clear buildings
        _home = null;
        _workplace = null;
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

    private void StartWalkingIfPossible()
    {
        // Check if it is time to leave for/from work
        // AND if currently not walking
        // AND if both a home and a workplace are present
        if(Time.time > _nextDepartureTick && !_enRoute && _workplace != null && _home != null)
        {
            // Check if worker is closer to home or work - basically, if he is at work or at home
            if(Vector3.Distance(this.transform.position, _home.transform.position) > Vector3.Distance(this.transform.position, _workplace.transform.position))
            {
                // Decide to go home
                _pathToWalk = _navigationManager.CalculatePathToBuilding(GetCurrentTile(), _home);
                Debug.Log("going home");
            }
            else
            {
                // Decide to go to work
                _pathToWalk = _navigationManager.CalculatePathToBuilding(GetCurrentTile(), _workplace);
                Debug.Log("going to work");
            }
            // Take the first tile of the path and pass it to the move function.
            // The remaining tiles will be visited by calls each time a movement-coroutine finishes
            Debug.Log(_pathToWalk.Count);
            _enRoute = true;
            MoveToTile(_pathToWalk.Dequeue());
        }
    }

    // The current tile the worker is standing on is determined by proximity
    private Tile GetCurrentTile()
    {
        Tile result = null; 
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        FindObjectsOfType<Tile>().ToList().ForEach(t => {
            float dist = Vector3.Distance(t.transform.position, this.transform.position);
            if (dist < minDist)
            {
                result = t;
                minDist = dist;
            }
        });
        return result;
    }



    // Just used for the translation of one tile to the next, not for pathfinding
    private void MoveToTile(Tile t)
    {
        // Face towards destination before walking
        // This should probably be replaced with a rotation along the y axis once actual walking and terrain-handling has been implemented.
        this.transform.LookAt(t.transform);
        // Pack parameters for coroutine
        object[] parameters = new object[2] { this.transform, t.transform.position };
        StartCoroutine("MoveToPosition", parameters);
    }

    // Movement is done via coroutine so nothing is blocked
    private IEnumerator MoveToPosition(object[] parameters)
    {
        // Unpack parameters
        Transform transform = (Transform) parameters[0];
        Vector3 position = (Vector3) parameters[1];

        var currentPos = transform.position;
        var t = 0f;
        while (t < 1)
        {
            t += Time.deltaTime / 10;
            transform.position = Vector3.Lerp(currentPos, position, t);
            yield return null;
        }
        // Start movement to next tile after reaching the target tile
        if (_pathToWalk.Count > 0)
        {
            MoveToTile(_pathToWalk.Dequeue());
        }
        else
        {
            _enRoute = false;
            _nextDepartureTick = Time.time + _timeUntilDeparture;
        }
    }
}
