using UnityEngine;

public class Edible : MonoBehaviour , IpooledObject
{
    public enum Food 
    {
      Falafel,
      Eggplant,
      Fries
    }
    public Food FoodType;
    public bool Adible;

    private void OnCollisionEnter(Collision collision)
    {
        if (!Adible) 
            return;

        EnemyAI enemy = collision.transform.GetComponent<EnemyAI>();
        if (enemy != null )
        {
            enemy.Eat(FoodType);
            gameObject.SetActive(false);
        }
        else if (collision.transform.tag == "Floor")
        {
            Adible = false;
        }
    }

    public void OnObjectSpawn()
    {
        Adible = true;
    }
}
