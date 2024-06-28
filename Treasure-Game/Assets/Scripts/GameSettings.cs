using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Set the target frame rate to 60 FPS
        Application.targetFrameRate = 60;

        // Lock the cursor to the center of the screen
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false; // Hide the cursor

        // Physics.gravity = new Vector3(0, -10f, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Check for mouse click to focus the game
        if (Input.GetMouseButtonDown(0))
        {
            // Lock the cursor to the center of the screen
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false; // Hide the cursor
        }
    }
}
