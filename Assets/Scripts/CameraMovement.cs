using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour {
    public float speed;
    public Camera cam;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    public float dragSpeed = 2;
    private Vector3 dragOrigin;

    float t0;

    // Use this for initialization
    void Start () {
        speed = 10f;
        minX = 10f;
        maxX = 55f;
        minY = 5f;
        maxY = 28f;
        t0 = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        CamaraKeyboardMovement();
        CamaraSpeed();
        CamaraZoom();
        CamaraMouseMovement();
        if (Input.GetKey(KeyCode.R) || Input.GetKey(KeyCode.D))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void CamaraKeyboardMovement()
    {
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            if (transform.position.x < maxX)
            {
                transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            }
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            if (transform.position.x > minX)
            {
                transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
            }
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            if (transform.position.y > minY)
            {
                transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
            }
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            if (transform.position.y < maxY)
            {
                transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
            }
        }
    }
    void CamaraMouseMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            t0 = Time.time;
            return;
        }

        if (!Input.GetMouseButton(0)) return;
        
        if (Input.GetMouseButton(0) && (Time.time - t0) > 0.1f)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(pos.x * -dragSpeed, pos.y * -dragSpeed, 0);

            transform.Translate(move, Space.World);
        }
    }
    void CamaraSpeed()
    {
        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            speed = speed + 1f;
        }
        if (Input.GetKey(KeyCode.KeypadMinus))
        {
            if (speed > 10f)
            {
                speed = speed - 1f;
            }
            else
            {
                speed = 10f;
            }
        }
    }
    void CamaraZoom()
    {
        var d = Input.GetAxis("Mouse ScrollWheel");
        if (d > 0f)
        {
            cam.orthographicSize = cam.orthographicSize - 1f;
        }
        else if (d < 0f)
        {
            cam.orthographicSize = cam.orthographicSize + 1f;
        }
    }
}


