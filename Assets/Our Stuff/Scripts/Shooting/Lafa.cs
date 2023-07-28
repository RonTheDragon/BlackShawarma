using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lafa : MonoBehaviour, IpooledObject
{
    public bool Adible;

    public void OnObjectSpawn()
    {
        Adible = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!Adible)
            return;

        EnemyAI enemy = collision.transform.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.EatLafa();
            gameObject.SetActive(false);
        }
        else if (collision.transform.tag == "Floor")
        {
            Adible = false;
        }
    }
}
