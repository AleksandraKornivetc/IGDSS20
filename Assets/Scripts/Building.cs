using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public string type; //The name of the building
    public float upkeep; //The money cost per minute
    public float buildCostMoney; //placement money cost
    public float buildCostPlanks; // placement planks cost
    public Tile tile; //Reference to the tile it is built on
    public float efficiency; //Calculated based on the surrounding tile types
    public int resourceGenerationInterval; //If operating at 100% efficiency, this is the time in seconds it takes for one production cycle to finish
    public int outputCount; //The number of output resources per generation cycle(for example the Sawmill produces 2 planks at a time)
    public bool canBuiltOnTileTypes; // A restriction on which types of tiles it can be placed on
    public float efficiencyScalesNeighbor; //A choice if its efficiency scales with a specific type of surrounding tile
    public int minNeighbors; //The minimum and maximum number of surrounding tiles its efficiency scales with(0-6)
    public int maxNeighbors;
    public int inputRes; //A choice for input resource types(0, 1 or 2 types)
    public int outputRes; //A choice for output resource type

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
