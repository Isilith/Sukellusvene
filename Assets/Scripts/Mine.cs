using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{

    public Collider mineCollider;
    public float id;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameController.HitMine(this);
            Destroy(transform.parent.gameObject);
        }
    }

    public Collider GetCollider()
    {
        if (mineCollider == null)
        {
            Debug.LogError("");
        }
        return mineCollider;
    }
}
