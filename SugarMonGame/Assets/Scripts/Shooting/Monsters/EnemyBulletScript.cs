//This file was create by Mark Botaish on July 8th, 2019

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletScript : MonoBehaviour
{
    public float _damage;

    private void OnTriggerEnter(Collider other)
    {
        //If the players bullets hits this GameObject, destroy this object and the other
        if (other.tag == "Bullet")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
