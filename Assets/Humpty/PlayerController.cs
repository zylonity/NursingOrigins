using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Transform player;
    public CharacterController charPlayer;

    public float speed = 6.0f;

    [HideInInspector]
    public float mouseSens = 4f;

    bool firstFrame = false;

    float mouseX = 0;
    float mouseY = 0;

    Vector3 velocity;
    void Start()
    {
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        mouseSens = PlayerPrefs.GetFloat("MouseSens");


    }

    void Update()
    {

        StartCoroutine(onMouse());

        float movementX = Input.GetAxis("Horizontal");
        float movementZ = Input.GetAxis("Vertical");

        Vector3 toMove = transform.right * movementX + transform.forward * movementZ;

        charPlayer.Move(toMove * speed * Time.deltaTime);


        velocity.y += -9.81f * Time.deltaTime;

        charPlayer.Move(velocity * Time.deltaTime);

        if (firstFrame == false)
        {
            Cursor.lockState = CursorLockMode.Locked;
            firstFrame = true;
        }


    }

    IEnumerator onMouse()
    {
        mouseX += (Input.GetAxis("Mouse X")) * mouseSens;
        mouseY -= (Input.GetAxis("Mouse Y")) * mouseSens;

        mouseY = Mathf.Clamp(mouseY, -90f, 90f);

        transform.localRotation = Quaternion.Euler(mouseY, mouseX, 0);

        yield return null;
    }


}
