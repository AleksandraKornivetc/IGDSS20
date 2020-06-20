using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class WorkerPooler : MonoBehaviour
{
    public static WorkerPooler instance;

    List<Worker> pooledObjects;
    public Worker poolObject;
    public int poolSize;

    void Start()
    {
        pooledObjects = new List<Worker>();
        for (int i = 0; i < poolSize; i++)
        {
            Worker w = (Worker)Instantiate(poolObject);
            w.transform.parent = this.gameObject.transform;
            w.gameObject.SetActive(false);
            w.SetupNeeds();
            pooledObjects.Add(w);
        }
    }

    public Worker GetWorker(int startingAge)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].gameObject.activeInHierarchy)
            {
                Worker w = pooledObjects[i];
                w.Rebirth();
                w._age = startingAge;
                w.gameObject.SetActive(w);
                return w;
            }
        }
        // Expan pool if no active workers are available anymore
        Worker newO = (Worker)Instantiate(poolObject);
        pooledObjects.Add(newO);
        return newO;
    }

    public List<Worker> GetActiveWorkers()
    {
        return pooledObjects.ToList().Where(w => w.gameObject.activeInHierarchy).ToList();
    }

    void Update()
    {

    }

    private void Awake()
    {
        instance = this;
    }
}
