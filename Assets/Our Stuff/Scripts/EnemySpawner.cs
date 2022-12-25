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
   [SerializeField] List<string> enemyTypes = new List<string>();
   [ReadOnly] [SerializeField] int CurrentEnemyAmmout = 0;

    [SerializeField] int _maxEnemyInGame ;
   List<EnemyAI> enemies = new List<EnemyAI>();



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
            if (CurrentEnemyAmmout <_maxEnemyInGame)
            {
            CurrentTimeLeft = Random.Range(RandomTime.x, RandomTime.y);
            SpawnEnemy();
        }
            }
    }

    EnemyAI SpawnEnemy()
    {
        GameObject Enemy = ObjectPooler.Instance.SpawnFromPool(enemyTypes[Random.Range(0,enemyTypes.Count)], DoorSpawnPoint.position, DoorSpawnPoint.rotation);
        EnemyAI enemyAI = Enemy.GetComponent<EnemyAI>();
        enemyAI.SetDestination(GetPreferableDestination(enemyAI));
        enemyAI.Spawn(this);
        CurrentEnemyAmmout++;         
        return enemyAI;
    }

    Vector3 GetPreferableDestination(EnemyAI enemyAI)
    {
        int SmallestAmountOfPeople = 10000;
        int SmallestLane = 0;
        for (int i = 0; i < CustomersInLane.Length; i++) //Check the Lowest line length
        {
            //Debug.Log($"{CustomersInLane[i]} < {SmallestAmountOfPeople} = {CustomersInLane[i] < SmallestAmountOfPeople}");
            if (CustomersInLane[i] < SmallestAmountOfPeople)
            {
                SmallestAmountOfPeople = CustomersInLane[i];
                SmallestLane = i;
            }
        }

        List<int> LaneNumbers = new List<int>();

        for (int i = 0; i < CustomersInLane.Length; i++) // Get All Shortest Lanes 
        {
            if (CustomersInLane[SmallestLane] == CustomersInLane[i])
            {
                LaneNumbers.Add(i);
            }
        }

        int ChosenRandom = Random.Range(0, LaneNumbers.Count); // Pick Random of the shortest lane

        int chosen = LaneNumbers[ChosenRandom];

        Vector3 Destination = LaneDestination(chosen, CustomersInLane[chosen]);

        enemyAI.WhichLane = chosen;
        enemyAI.PlaceInLane = CustomersInLane[chosen];
        enemies.Add(enemyAI);
        CustomersInLane[chosen]++; // Tell the enemy manager that place was filled
        //Debug.Log(Destination);

        return Destination; //Tells the enemy where to go 
    }

    private void OnDrawGizmos()
    {
        if (DrawGizmos)
        {
            for (int i = 0; i < CustomersInLane.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(LaneDestination(i, 2+ CustomersInLane[i]), Vector3.one);

                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(LaneDestination(i, 1+CustomersInLane[i]), Vector3.one);

                Gizmos.color = Color.blue;
                Gizmos.DrawCube(LaneDestination(i,CustomersInLane[i]), Vector3.one);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawCube(DoorSpawnPoint.transform.position + Vector3.up, Vector3.one + Vector3.up);
        }
    }
    
    public void RemoveOnLane(int Lane, int spotInLane)
    {
        CustomersInLane[Lane]--;

        foreach(EnemyAI e in enemies)
        {
            if (e.WhichLane == Lane && e.PlaceInLane > spotInLane)
            {
                e.PlaceInLane--;
                e.SetDestination(LaneDestination(Lane,e.PlaceInLane));
            }
        }
    }

    public void RemoveEnemy(EnemyAI enemyAI)
    {
        enemies.Remove(enemyAI);
        CurrentEnemyAmmout--;
      
    }

    Vector3 LaneDestination(int i,int place = 0)
    {
        return LanesBase.transform.position // Lanes Base
               + LanesBase.transform.right * i * DistanceBetweenLanes //Lanes Seperation
               + LanesBase.transform.forward *  place * DistanceInLane; //In Lane Seperation
    }
}


