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

    // Update is called once per frame
    void FixedUpdate()
    {
        // while holding the right mouse button, moving the mouse pans the camera (on the XZ-plane)
        if (Input.GetMouseButton(1)) {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (Input.GetAxis("Mouse X") != 0)
            {
                Vector3 move = new Vector3(Input.GetAxisRaw("Mouse X") * speed * Time.deltaTime, 0, 0);
                transform.Translate(move, Space.World);
            }
            if (Input.GetAxis("Mouse Y") != 0)
            {
                Vector3 move = new Vector3(0, 0, Input.GetAxisRaw("Mouse Y") * speed * Time.deltaTime);
                transform.Translate(move, Space.World);
            }
        }
        // scrolling the mouse wheel zooms the camera in and out
        var fov = Camera.main.fieldOfView;
   		fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity;
   		fov = Mathf.Clamp(fov, minFov, maxFov);
   		Camera.main.fieldOfView = fov;
        
    }
}
