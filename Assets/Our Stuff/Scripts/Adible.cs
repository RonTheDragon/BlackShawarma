using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Adible : MonoBehaviour
{
    public enum Food 
    {
      falfel,
      eggplant,
      chips
    }
    public Food foodtype;
    private void OnCollisionEnter(Collision collision)
    {
        EnemyAI enemy = collision.transform.GetComponent<EnemyAI>();
        if (enemy != null )
        {
            enemy.Eat(foodtype);
            gameObject.SetActive(false);
        }
    }
}
