using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Texture2D heightmap;
    public GameObject[] tiles;
    // Determines how pronounced differences of the heightmap are
    public float terrainExtremity;

    float tileWidth;
    float tileHeight;

    // Start is called before the first frame update
    void Start()
    {
        tileWidth = tiles[0].GetComponent<Renderer>().bounds.size.z;
        tileHeight = tiles[0].GetComponent<Renderer>().bounds.size.x;
        GenerateMap();

    }

    // Update is called once per frame
    void Update()
    {


    }

    private void GenerateMap()
    {
        if (heightmap == null || tiles == null || tiles.Length == 0)
        {
            Debug.Log("Aborting, references not configured");
            return;
        }
        int w = heightmap.width;
        int h = heightmap.height;
        for(int i = 0; i < h; i++)
        {
            for(int j = 0; j < w; j++)
            {
                float grey = heightmap.GetPixel(i, j).maxColorComponent;
                int tileIndex;

                if (grey > 0.8) tileIndex = 0;
                else if (grey > 0.6) tileIndex = 1;
                else if (grey > 0.4) tileIndex = 2;
                else if (grey > 0.2) tileIndex = 3;
                else if (grey > 0) tileIndex = 4;
                else tileIndex = 5;

                Vector3 newTileTransform = new Vector3(
                    // static value 0.75f should be calculated based on tile height and width instead, but this works too.
                    i * tileHeight * 0.75f,
                    grey * terrainExtremity,
                    j * tileWidth + (i % 2 * tileWidth / 2));

                GameObject newTile = Instantiate(tiles[tileIndex]) as GameObject;
                newTile.transform.Translate(newTileTransform, Space.World);

            } 
        }

    }
    

}
