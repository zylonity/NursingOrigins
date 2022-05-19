using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    Rigidbody rb;
    bool fired = false;
    public GameObject goodCannon;
    public GameObject brokenCannon;
    public Transform explosionCentre;
    public GameObject[] brokenCannonPieces;

    public ParticleSystem explosion;

    public float radius = 5.0f;
    public float power = 1.01f;


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

        goodCannon.SetActive(false);
        brokenCannon.SetActive(true);

        Vector3 explosionPos = explosionCentre.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        explosion.Play();
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
                rb.AddExplosionForce(power, explosionPos, radius, 0.01f);
        }


    }

    


}
