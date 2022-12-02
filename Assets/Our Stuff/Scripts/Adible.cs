using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adible : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        EnemyAI enemy = collision.transform.GetComponent<EnemyAI>();
        if (enemy != null )
        {
            enemy.Eat();
            gameObject.SetActive(false);
        }
    }
}
