using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    public Transform DoorSpawnPoint;

    [SerializeField] int[] CustomersInLane = new int[5];

    [SerializeField] float CurrentTimeLeft;
    [SerializeField] Vector2 RandomTime;

    [SerializeField] Transform LanesBase;

    [SerializeField] float DistanceBetweenLanes = 5;
    [SerializeField] float DistanceInLane = 5;

    [SerializeField] bool DrawGizmos = true;

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
        enemyAI.Spawn(this);
        return enemyAI;
    }

    Vector3 GetPreferableDestination()
    {
        int SmallestAmountOfPeople = 10000;
        int SmallestLane = 0;
        for (int i = 0; i < CustomersInLane.Length; i++)
        {
            Debug.Log($"{CustomersInLane[i]} < {SmallestAmountOfPeople} = {CustomersInLane[i] < SmallestAmountOfPeople}");
            if (CustomersInLane[i] < SmallestAmountOfPeople)
            {
                SmallestAmountOfPeople = CustomersInLane[i];
                SmallestLane = i;
            }
        }
        
        List<int> LaneNumbers = new List<int>();

        for (int i = 0; i < CustomersInLane.Length; i++)
        {
            if (CustomersInLane[SmallestLane] == CustomersInLane[i])
            {
                LaneNumbers.Add(i);
            }
        }

        int ChosenRandom = Random.Range(0,LaneNumbers.Count);

        int chosen = LaneNumbers[ChosenRandom];

        Vector3 Destination = LaneDestination(chosen);

        CustomersInLane[chosen]++;
        Debug.Log(Destination);
       
        return Destination;
    }

    private void OnDrawGizmos()
    {
        if (DrawGizmos)
        {
            for (int i = 0; i < CustomersInLane.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(LaneDestination(i, 2), Vector3.one);

                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(LaneDestination(i, 1),Vector3.one);

                Gizmos.color = Color.blue;
                Gizmos.DrawCube(LaneDestination(i), Vector3.one);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawCube(DoorSpawnPoint.transform.position + Vector3.up, Vector3.one + Vector3.up);
        }
    }

    Vector3 LaneDestination(int i,int place = 0)
    {
        return LanesBase.transform.position // Lanes Base
               + LanesBase.transform.right * i * DistanceBetweenLanes //Lanes Seperation
               + LanesBase.transform.forward * (CustomersInLane[i] + place) * DistanceInLane; //In Lane Seperation
    }
}


