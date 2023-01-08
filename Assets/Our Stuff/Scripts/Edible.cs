using UnityEngine;

public class Edible : MonoBehaviour
{
    public enum Food 
    {
      Falafel,
      Eggplant,
      Fries
    }
    public Food FoodType;

    private void OnCollisionEnter(Collision collision)
    {
        EnemyAI enemy = collision.transform.GetComponent<EnemyAI>();
        if (enemy != null )
        {
            enemy.Eat(FoodType);
            gameObject.SetActive(false);
        }
    }
}
