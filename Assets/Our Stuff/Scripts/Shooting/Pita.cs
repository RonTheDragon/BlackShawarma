using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pita : MonoBehaviour
{
    [ReadOnly]public List<BuildOrder.Fillers> Ingridients = new List<BuildOrder.Fillers>();

    private void OnCollisionEnter(Collision collision)
    {
        EnemyAI enemy = collision.transform.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.EatPita(Ingridients);
            gameObject.SetActive(false);
        }
    }
}
