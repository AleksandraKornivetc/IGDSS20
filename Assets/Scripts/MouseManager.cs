using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    // Camera speeds
    float speed = 200;
    float zoomSpeed = 1000;

    // Camera bounds
    float lowerZoomBound;
    float upperZoomBound;
    float lowerBoundX;
    float upperBoundX;
    float lowerBoundZ;
    float upperBoundZ;

    Camera mainCamera;
    Transform cameraTransform;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
        DetermineVerticalBounds();

    }

    // Update is called once per frame
    void Update()
    {
        HandlePanning();
        HandleZooming();
        HandleClicking();

    }

    private void HandlePanning()
    {
        if (Input.GetMouseButton(1))
        {

            // For some reason, the vertical axis needs to be mirrored in order to make the interaction feel like "dragging" the map.
            Vector3 translationChange = new Vector3(Input.GetAxis("Mouse Y") * speed, 0, -Input.GetAxis("Mouse X") * speed);
            // Movement should be time dependent, not FPS dependent
            translationChange *= Time.deltaTime;
            // By using Space.World, the camera will be moved alongside the axes of the world, not the axes of the (rotated) camera

            cameraTransform.Translate(translationChange, Space.World);

            // Instead of doing math, we limit the movement of the camera by casting a ray from its center.
            // If it does not hit anything anymore, the current transform is undone.
            // Additionally, if the side of the front row of tiles is hit, the y of the hit point will be below 0.
            // This also results in an undoing of the transform.
            RaycastHit hit;
            if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit) || hit.point.y < 0)
            {
                cameraTransform.Translate(-translationChange, Space.World);
            }
        }

    }

    private void HandleZooming()
    {
        // Zoom is possible without right-clicking
        // Zoom bounds might be exceeded slightly at high scrolling speeds - but only by a small value.
        float zoomChange = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        if ((cameraTransform.position.y > lowerZoomBound && zoomChange > 0f)
            || (cameraTransform.position.y < upperZoomBound && zoomChange < 0f))
        {
            cameraTransform.Translate(0, 0, zoomChange, Space.Self);
        }
    }

    private void HandleClicking()
    {
        // This will output the GameObject name. Clicking the sides of the tile pillar will also count.
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit was on a tile, not a building
                Tile t = hit.transform.gameObject.GetComponent<Tile>();
                if(t != null)
                {
                    int h = hit.transform.gameObject.GetComponent<Tile>()._coordinateHeight;
                    int w = hit.transform.gameObject.GetComponent<Tile>()._coordinateWidth;
                    FindObjectOfType<GameManager>().TileClicked(h, w);
                }
            }
        }
    }

    public void DetermineHorizontalBounds()
    {
        // The horizontal bounds are determined by the tiles closest to the edge of the map
        GameObject[] allTiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in allTiles)
        {
            lowerBoundX = lowerBoundX < tile.transform.position.x ? lowerBoundX : tile.transform.position.x;
            lowerBoundZ = lowerBoundZ < tile.transform.position.z ? lowerBoundZ : tile.transform.position.z;
            upperBoundX = upperBoundX > tile.transform.position.x ? upperBoundX : tile.transform.position.x;
            upperBoundZ = upperBoundZ > tile.transform.position.z ? upperBoundZ : tile.transform.position.z;
        }
        CenterCamera();
    }

    private void DetermineVerticalBounds()
    {
        // Vertical bounds are dependant on the current value of terrainExtremity in the GameManager, otherwise one could zoom in too much.
        lowerZoomBound = FindObjectOfType<GameManager>().terrainExtremity * 2;
        upperZoomBound = lowerZoomBound * 3;
    }

    private void CenterCamera()
    {
        // Once the horizontal bounds are known, the camera can be placed in a way to display the entire map - otherwise, it would be at the top left corner.
        cameraTransform.Translate(new Vector3(upperBoundZ / 2, 0, upperBoundX / 2), Space.World);
    }
}
