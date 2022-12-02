using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] int[] CustomersInLane = new int[5];

    [SerializeField] float CurrentTimeLeft;
    [SerializeField] Vector2 RandomTime;

    public Transform DoorSpawnPoint;

    [SerializeField] Transform LanesBase;

    [SerializeField] float DistanceBetweenLanes = 5;
    [SerializeField] float DistanceInLane = 5;

    [SerializeField] bool DrawGizmos = true;

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



        Vector3 Destination = LanesBase.transform.position + LanesBase.transform.right * chosen * DistanceBetweenLanes + LanesBase.transform.forward * CustomersInLane[chosen] * DistanceInLane;

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
                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(LanesBase.transform.position + LanesBase.transform.right * i * DistanceBetweenLanes + LanesBase.transform.forward * (CustomersInLane[i]+1) * DistanceInLane, Vector3.one);

                Gizmos.color = Color.blue;
                Gizmos.DrawCube(LanesBase.transform.position + LanesBase.transform.right * i * DistanceBetweenLanes + LanesBase.transform.forward * CustomersInLane[i] * DistanceInLane, Vector3.one);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawCube(DoorSpawnPoint.transform.position + Vector3.up, Vector3.one + Vector3.up);
        }
    }
}


