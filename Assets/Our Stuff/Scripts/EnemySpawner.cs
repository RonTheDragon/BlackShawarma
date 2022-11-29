using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int[] CustomersInLane = new int[5];

    [SerializeField] float CurrentTimeLeft;
    [SerializeField] Vector2 RandomTime;

    [SerializeField] Transform DoorSpawnPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SpawnerTimer();
    }

    void SpawnerTimer()
    {
       if (CurrentTimeLeft > 0) 
        { 
            CurrentTimeLeft -= Time.deltaTime;
        }
        else
        {
            CurrentTimeLeft = Random.Range(RandomTime.x,RandomTime.y);
            SpawnEnemy();
        }
    }

    EnemyAI SpawnEnemy()
    {
        GameObject Enemy = ObjectPooler.Instance.SpawnFromPool("Enemy", DoorSpawnPoint.position, DoorSpawnPoint.rotation);
        EnemyAI enemyAI = Enemy.GetComponent<EnemyAI>();
        enemyAI.SetDestination(GetPreferableDestination());
        return enemyAI;
    }

    Vector3 GetPreferableDestination()
    {
        int n = 10000;
        int SmallestLane = 0;
        for (int i = 0; i < CustomersInLane.Length; i++)
        {
            if (CustomersInLane[i] < n)
            {
                n= CustomersInLane[i];
                SmallestLane = i;
            }
        }

        List<int> LaneNumbers = new List<int>();

        for (int i = 0; i < CustomersInLane.Length; i++)
        {
            if (SmallestLane == CustomersInLane[i])
            {
                LaneNumbers.Add(i);
            }
        }

        int ChosenRandom = Random.Range(0,LaneNumbers.Count);

        int LaneNumber = CustomersInLane[ChosenRandom];

        Vector3 Destination = new Vector3(LaneNumber*5, 0, CustomersInLane[LaneNumber]*5);
        CustomersInLane[LaneNumber]++;
        Debug.Log(Destination);
       
        return Destination;
    }


}


