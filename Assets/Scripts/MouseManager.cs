using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
	public float speed = 100.0f;
    // there are limits for the minimum and maximum zoom
    public float minFov = 15.0f;
 	public float maxFov = 90.0f;
 	public float sensitivity = 10.0f;
    public Terrain myTerrain;
    Vector3 mapMinBounds;
    Vector3 mapMaxBounds;
    public float nonPassibleBorderWidth = 10.0f;
    public GameObject[] tiles;

    private void Start()
    {
        // Define the borders of the terrain
        myTerrain = Terrain.activeTerrain;
        var myTerrainTransform = myTerrain.transform;
        mapMinBounds = new Vector3(myTerrainTransform.position.x, 0, myTerrainTransform.position.z);
        mapMaxBounds = mapMinBounds + new Vector3(myTerrain.terrainData.size.x, 0, myTerrain.terrainData.size.z);
        mapMinBounds.x += nonPassibleBorderWidth;
        mapMinBounds.z += nonPassibleBorderWidth;
        mapMaxBounds.x -= nonPassibleBorderWidth;
        mapMaxBounds.z -= nonPassibleBorderWidth;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // while holding the right mouse button, moving the mouse pans the camera (on the XZ-plane)
        if (Input.GetMouseButton(1)) {
            if (Input.GetAxis("Mouse X") != 0)
            {
                Vector3 move = new Vector3(0, 0, -Input.GetAxisRaw("Mouse X") * speed * Time.deltaTime);
                transform.Translate(move, Space.World);
            }
            if (Input.GetAxis("Mouse Y") != 0)
            {
                Vector3 move = new Vector3(Input.GetAxisRaw("Mouse Y") * speed * Time.deltaTime, 0, 0);
                transform.Translate(move, Space.World);
            }
        }
        // scrolling the mouse wheel zooms the camera in and out
        var fov = Camera.main.fieldOfView;
   		fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
   		fov = Mathf.Clamp(fov, minFov, maxFov);
   		Camera.main.fieldOfView = fov;
        Vector3 p = transform.position;
        // The camera cannot be moved outside the boundaries of the terrain
        if (p.x > mapMaxBounds.x)
        {
            p.x = mapMaxBounds.x;
        }
        if (p.z > mapMaxBounds.z)
        {
            p.z = mapMaxBounds.z;
        }
        if (p.x < mapMinBounds.x)
        {
            p.x = mapMinBounds.x;
        }
        if (p.z < mapMinBounds.z)
        {
            p.z = mapMinBounds.z;
        }
        transform.position = p;

        // Left clicking a tile outputs the type of tile as text on the console (the name of the GameObject is sufficient)
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Tile"))
                {
                    name = hit.collider.name;
                    Debug.Log("Hit the " + name);
                }
            }
        }


    }
}
