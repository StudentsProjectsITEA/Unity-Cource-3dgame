using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

    public GameObject explosion;
    public float explosionRadius;
    public float explosionForce;

    void Explode()
    {
        Instantiate(explosion, this.transform.position, Quaternion.identity);
        foreach (Collider item in Physics.OverlapSphere(transform.position, explosionRadius))
        {
            Rigidbody tempRB = item.GetComponent<Rigidbody>();
            if (!tempRB.Equals(null))
                tempRB.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            if (item.tag == "Enemy")
            {
                item.GetComponent<Animator>().SetBool("isDead", true);
            }
        }
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }
}
