using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform player;
    public CharacterController charPlayer;

    public float speed = 6.0f;
    public float mouseSens = 100f;

    bool firstFrame = false;

    float mouseX = 0;
    float mouseY = 0;

    Vector3 velocity;
    void Start()
    {
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    void Update()
    {

        if (Input.mousePosition.x > 950f && Input.mousePosition.x < 960f)
        {
            mouseX += (Input.GetAxis("Mouse X") * Time.deltaTime) * mouseSens;
            mouseY -= (Input.GetAxis("Mouse Y") * Time.deltaTime) * mouseSens;
        }
        

        float movementX = Input.GetAxis("Horizontal");
        float movementZ = Input.GetAxis("Vertical");

        Vector3 toMove = transform.right * movementX + transform.forward * movementZ;

        charPlayer.Move(toMove * speed * Time.deltaTime);

        mouseY = Mathf.Clamp(mouseY, -90f, 90f);

        transform.localRotation = Quaternion.Euler(mouseY, mouseX, 0);

        velocity.y += -9.81f * Time.deltaTime;

        charPlayer.Move(velocity * Time.deltaTime);

        if (firstFrame == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            firstFrame = true;
        }


    }

}
