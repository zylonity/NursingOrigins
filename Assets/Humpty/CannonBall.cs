using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    Rigidbody rb;
    bool fired = false;
    public GameObject broken;

    public void Fire()
    {
        if (fired == false)
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(transform.up * 5, ForceMode.Impulse);
            fired = true;
        }

    }

    public void FireAndBreak()
    {

        if (fired == false)
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.AddForce(transform.up * 5, ForceMode.Impulse);
            fired = true;
        }

        transform.parent.gameObject.SetActive(false);
        broken.SetActive(true);
    }


}
