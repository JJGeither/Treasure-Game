using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public float verticalInput { get; private set; }
    public float horizontalInput { get; private set; }
    public bool jumpInput { get; private set; }
    public float mouseX { get; private set; }

    public bool button1Down {  get; private set; }
    public bool button2Down {  get; private set; }
    public bool button3Down {  get; private set; }

    // Update is called once per frame
    void Update()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        jumpInput = Input.GetKey(KeyCode.LeftShift);
        mouseX = Input.GetAxis("Mouse X");
        button1Down = Input.GetKeyDown(KeyCode.R);
        button1Down = Input.GetKeyDown(KeyCode.Q);
        button1Down = Input.GetKeyDown(KeyCode.F);
    }



}
