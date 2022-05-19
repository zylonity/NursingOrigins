using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform player;
    public CharacterController charPlayer;

    public float speed = 6.0f;
    public float mouseSens = 100f;
    float rotationX = 0;
    float mouseX = 0;

    Vector3 velocity;

    void Start()
    {
        //maxY = player.position.y;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        mouseX += Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        float movementX = Input.GetAxis("Horizontal");
        float movementZ = Input.GetAxis("Vertical");

        Vector3 toMove = transform.right * movementX + transform.forward * movementZ;

        charPlayer.Move(toMove * speed * Time.deltaTime);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);




        transform.localRotation = Quaternion.Euler(rotationX, mouseX, 0);

        velocity.y += -9.81f * Time.deltaTime;

        charPlayer.Move(velocity * Time.deltaTime);

    }

}
