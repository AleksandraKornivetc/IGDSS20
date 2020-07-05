using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class NavigationManager : MonoBehaviour
{
    public int[,] GeneratePotentialField(Tile[,] map, Building b)
    {
        int[,] result = new int[map.GetLength(0), map.GetLength(1)];

        Tile buildingTile = b._tile;
        int x = buildingTile._coordinateWidth;
        int y = buildingTile._coordinateHeight;
        
        GeneratePotentialFieldInner(result, buildingTile, buildingTile);

        /*
        //Debugging only, print weights of all fields
        for (int i = 0; i < map.GetLength(1); i++)
        {
            string row = "";
            for (int j = 0; j < map.GetLength(0); j++)
            {
                row += result[j, i] + " ";
            }
            Debug.Log(row);
        }
        */

        return result;
    }

    private void GeneratePotentialFieldInner(int[,] field, Tile currentTile, Tile initialTile)
    {
        int currentCost = field[currentTile._coordinateWidth, currentTile._coordinateHeight];
        foreach (Tile neighboringTile in currentTile._neighborTiles)
        {
            int neighborX = neighboringTile._coordinateWidth;
            int neighborY = neighboringTile._coordinateHeight;
            int neighborCost = currentCost + GetTraversalCost(neighboringTile);
            // Check if value for the current neighboring tile has already been set
            // Ignore the initial Tile on which the building is placed
            if (!initialTile.Equals(neighboringTile) && (field[neighborX, neighborY] == 0 || field[neighborX, neighborY] > neighborCost))
            {
                field[neighborX, neighborY] = neighborCost;
                GeneratePotentialFieldInner(field, neighboringTile, initialTile);
            }
        }
    }

    public Queue<Tile> CalculatePathToBuilding(Tile startingTile, Building b)
    {
        Queue<Tile> path = new Queue<Tile>();
        Tile targetTile = b._tile;
        Tile currentTile = startingTile;
        int[,] potentialField = b.GetPotentialField();

        while (!currentTile.Equals(targetTile))
        {
            // Order neighboring tiles by the target buildings potential field values, select lowest value tile as next tile.
            currentTile = currentTile._neighborTiles.ToList().OrderBy(t => potentialField[t._coordinateWidth, t._coordinateHeight]).First();
            path.Enqueue(currentTile);
        }
        return path;
    }

    private int GetTraversalCost(Tile t)
    {
        switch (t._type)
        {
            case Tile.TileTypes.Water:
                return 30;
            case Tile.TileTypes.Sand:
                return 2;
            case Tile.TileTypes.Grass:
                return 1;
            case Tile.TileTypes.Forest:
                return 2;
            case Tile.TileTypes.Stone:
                return 1;
            case Tile.TileTypes.Mountain:
                return 3;
            default:
                return 0;
        }
    }
}
