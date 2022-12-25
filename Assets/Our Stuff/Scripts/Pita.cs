using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pita : MonoBehaviour
{
    public List<BuildOrder.Fillers> pitashoot = new List<BuildOrder.Fillers>();

    private void OnCollisionEnter(Collision collision)
    {
        EnemyAI enemy = collision.transform.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.EatPita(pitashoot);
            gameObject.SetActive(false);
        }
    }
}
