using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform DoorSpawnPoint;

    [SerializeField] int[] CustomersInLane = new int[5];

    [SerializeField] float   _currentTimeLeft;
    [SerializeField] Vector2 _randomTime;


    [SerializeField] Transform _lanesBase;

    [SerializeField] float _distanceBetweenLanes = 5;
    [SerializeField] float _distanceInLane       = 5;

    [SerializeField]            bool         _drawGizmos          = true;
    [SerializeField]            List<string> _enemyTypes          = new List<string>();
    [ReadOnly] [SerializeField] int          _currentEnemyAmmout  = 0;

    [SerializeField] int           _maxEnemyInGame;
                     List<EnemyAI> _enemies = new List<EnemyAI>();

    [SerializeField] Transform  _sideOrdersContent;
    [SerializeField] GameObject _sideOrderPrefab;
    private GameManager _gm => GameManager.Instance;
    LevelTimer _leveltimer;

    public int CustomerCounter;

    private void Start()
    {
       _leveltimer              = _gm.GetComponent<LevelTimer>();
       _leveltimer.OnTimerDone += StoreIsClose;
    }

    void Update()
    {
        SpawnerTimer();
    }

    void SpawnerTimer()
    {
        if (_currentTimeLeft > 0)
        {
            _currentTimeLeft -= Time.deltaTime;
        }
        else
        {
            if (_currentEnemyAmmout <_maxEnemyInGame)
            {
                _currentTimeLeft = Random.Range(_randomTime.x, _randomTime.y);
                SpawnEnemy();
            }
        }
    }

    EnemyAI SpawnEnemy()
    {
        GameObject Enemy   = ObjectPooler.Instance.SpawnFromPool(_enemyTypes[Random.Range(0, _enemyTypes.Count)], DoorSpawnPoint.position, DoorSpawnPoint.rotation);
        EnemyAI    enemyAI = Enemy.GetComponent<EnemyAI>();
        enemyAI.SetDestination(GetPreferableDestination(enemyAI));
        CustomerCounter++;
        enemyAI.Spawn(this, CustomerCounter);
        _currentEnemyAmmout++;

        SideOrderUI order = Instantiate(_sideOrderPrefab, _sideOrdersContent.position, Quaternion.identity, _sideOrdersContent).GetComponent<SideOrderUI>();
        enemyAI.SideOrder = order;
        order.SetUp(enemyAI);

        return enemyAI;
    }

    Vector3 GetPreferableDestination(EnemyAI enemyAI)
    {
        int SmallestAmountOfPeople = 10000;
        int SmallestLane           = 0;

        for (int i = 0; i < CustomersInLane.Length; i++) //Check the Lowest line length
        {
            //Debug.Log($"{CustomersInLane[i]} < {SmallestAmountOfPeople} = {CustomersInLane[i] < SmallestAmountOfPeople}");
            if (CustomersInLane[i] < SmallestAmountOfPeople)
            {
                SmallestAmountOfPeople = CustomersInLane[i];
                SmallestLane           = i;
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

        enemyAI.WhichLane   = chosen;
        enemyAI.PlaceInLane = CustomersInLane[chosen];
        _enemies.Add(enemyAI);
        CustomersInLane[chosen]++; // Tell the enemy manager that place was filled
        //Debug.Log(Destination);

        return Destination; //Tells the enemy where to go 
    }

    private void OnDrawGizmos()
    {
        if (_drawGizmos)
        {
            for (int i = 0; i < CustomersInLane.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(LaneDestination(i, 2+ CustomersInLane[i]), Vector3.one);

                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(LaneDestination(i, 1+ CustomersInLane[i]), Vector3.one);

                Gizmos.color = Color.blue;
                Gizmos.DrawCube(LaneDestination(i,    CustomersInLane[i]), Vector3.one);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawCube(DoorSpawnPoint.transform.position + Vector3.up, Vector3.one + Vector3.up);
        }
    }
    
    public void RemoveOnLane(int Lane, int spotInLane)
    {
        CustomersInLane[Lane]--;

        foreach(EnemyAI e in _enemies)
        {
            if (e.WhichLane == Lane && e.PlaceInLane > spotInLane)
            {
                e.PlaceInLane--;
                e.SetDestination(LaneDestination(Lane,e.PlaceInLane));
            }
        }

        CollectFromLongestLane(Lane);
    }

    private void CollectFromLongestLane(int currentLane)
    {
        int n = 0;
        foreach(int i in CustomersInLane)
        {
            if (i > CustomersInLane[currentLane] + 1) //if lane is longer than current lane by at least 2
            {
                
                foreach (EnemyAI e in _enemies)
                {
                    
                    if (e.WhichLane == n && e.PlaceInLane == i-1)
                    {
                        
                        e.WhichLane = currentLane;
                        e.PlaceInLane = CustomersInLane[currentLane];
                        e.SetDestination(LaneDestination(currentLane, CustomersInLane[currentLane]));
                        
                        CustomersInLane[currentLane] += 1;
                        CustomersInLane[n] -= 1;

                        return;
                    }
                }
            }
            n++;
        }
    }

    public void AddInFrontOfLane(int Lane, int spotInLane) //for soldier (future use)
    {
        foreach (EnemyAI e in _enemies)
        {
            if (e.WhichLane == Lane && e.PlaceInLane > spotInLane)
            {
                e.PlaceInLane++;
                e.SetDestination(LaneDestination(Lane, e.PlaceInLane));
            }
        }
    }

    public void RemoveEnemy(EnemyAI enemyAI)
    {
        _enemies.Remove(enemyAI);
        _currentEnemyAmmout--;
    }

    Vector3 LaneDestination(int i,int place = 0)
    {
        return _lanesBase.transform.position                                  // Lanes Base
               + _lanesBase.transform.right   *   i   * _distanceBetweenLanes //Lanes Seperation
               + _lanesBase.transform.forward * place * _distanceInLane;      //In Lane Seperation
    }
    void StoreIsClose()
    {
        _maxEnemyInGame = 0;
       
    }

   public int HowManyEnemiesInTheStore()
    {
       return _enemies.Count;
        
    }

    public void LevelSetUp(List<string> enemies, Vector2 RandomTime, Vector2 WarmUpTime, int maxEnemies)
    {
        _enemyTypes      = enemies;
        _randomTime      = RandomTime;
        _currentTimeLeft = Random.Range(WarmUpTime.x,WarmUpTime.y);
        _maxEnemyInGame  = maxEnemies;
        _gm.SetTazdokHp(_gm.MaxTzadokHp);
        CustomerCounter = 0;
    }

    public void ClearingLevel()
    {
        _leveltimer.SetTimerTo0();
        for (int i = 0; i < _enemies.Count; i++)
        {
            _enemies[i].InstantlyRemoveCustomer();
        }
    }

    public void ChangeMaxEnemiesInGame(int maxEnemies)
    {
        _maxEnemyInGame = maxEnemies;
    }

    public EnemyAI GetFirstEnemy()
    {
        return _enemies[0];
    }

}


