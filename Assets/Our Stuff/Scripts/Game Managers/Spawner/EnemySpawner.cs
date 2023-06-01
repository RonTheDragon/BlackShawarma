using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform DoorSpawnPoint;
    
    public QueueSystem EnemyQueues = new QueueSystem();
    private List<EnemyAI> _leaving = new List<EnemyAI>();

    [SerializeField] float   _currentTimeLeft;
    [SerializeField] Vector2 _randomTime;


    [SerializeField] Transform _lanesBase;

    [SerializeField] float _distanceBetweenLanes = 5;
    [SerializeField] float _distanceInLane       = 5;

    [SerializeField]            bool         _drawGizmos          = true;
    [SerializeField]            List<SOspawnEnemy> _enemyTypes          = new List<SOspawnEnemy>();
    [ReadOnly] [SerializeField] private SOLevel.SpawnLimit _spawnLimit;
    [ReadOnly] [SerializeField] int          _currentEnemyAmount  = 0;

    [SerializeField] int           _maxEnemyInGame;

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
            if (_currentEnemyAmount <_maxEnemyInGame)
            {
                _currentTimeLeft = Random.Range(_randomTime.x, _randomTime.y);
                SpawnEnemy();
            }
        }
    }

    EnemyAI SpawnEnemy()
    {
        GameObject Enemy   = ObjectPooler.Instance.SpawnFromPool(ChooseEnemy(), DoorSpawnPoint.position, DoorSpawnPoint.rotation);
        EnemyAI    enemyAI = Enemy.GetComponent<EnemyAI>();
        enemyAI.SetDestination(GetPreferableDestination(enemyAI));
        CustomerCounter++;
        enemyAI.Spawn(this, CustomerCounter);
        _currentEnemyAmount++;

        SideOrderUI order = Instantiate(_sideOrderPrefab, _sideOrdersContent.position, Quaternion.identity, _sideOrdersContent).GetComponent<SideOrderUI>();
        enemyAI.SideOrder = order;
        order.SetUp(enemyAI);

        return enemyAI;
    }

    private string ChooseEnemy()
    {
        SOspawnEnemy currentSpawnable = _enemyTypes[0];

        List<EnemyChances> chances = new List<EnemyChances>();

        if (currentSpawnable.Hipster > 0 && _spawnLimit.Hipster>0) chances.Add(new EnemyChances("FriesGuy", currentSpawnable.Hipster, () => _spawnLimit.Hipster--));
        if (currentSpawnable.OldMan > 0 && _spawnLimit.OldMan>0) chances.Add(new EnemyChances("EggplantGuy", currentSpawnable.OldMan, () => _spawnLimit.OldMan--));
        if (currentSpawnable.Ars > 0 && _spawnLimit.Ars>0) chances.Add(new EnemyChances("FalafelGuy", currentSpawnable.Ars, () => _spawnLimit.Ars--));
        if (currentSpawnable.Arsit > 0 && _spawnLimit.Arsit > 0) chances.Add(new EnemyChances("Arsit", currentSpawnable.Arsit, () => _spawnLimit.Arsit--));
        if (currentSpawnable.Soldier > 0 && _spawnLimit.Soldier>0) chances.Add(new EnemyChances("Soldier", currentSpawnable.Soldier, () => _spawnLimit.Soldier--));
        if (currentSpawnable.Cop > 0 && _spawnLimit.Cop > 0) chances.Add(new EnemyChances("Cop", currentSpawnable.Cop,  () => _spawnLimit.Cop--));
        if (currentSpawnable.Mobster > 0 && _spawnLimit.Mobster > 0) chances.Add(new EnemyChances("Mobster", currentSpawnable.Mobster, () => _spawnLimit.Mobster--));

        if (_enemyTypes.Count > 1)
        {
            _enemyTypes.RemoveAt(0);
        }

        if (chances.Count > 1)
        {
            float maxRandom = 0;
            foreach (EnemyChances chance in chances) 
            {
                maxRandom += chance.ChanceToSpawn;
            }

            float randomChoose = Random.Range(0, maxRandom);

            float countUp = 0;
            foreach (EnemyChances chance in chances)
            {
                countUp+= chance.ChanceToSpawn;
                if (countUp >= randomChoose) 
                {
                    return chance.GetEnemy();
                }
            }
        }

        if (chances.Count == 0) { Debug.LogWarning("Empty Spawnable Enemy"); return "FriesGuy";  }

        return chances[0].GetEnemy();
    }
    
    class EnemyChances
    {
        public string EnemyName;
        public float ChanceToSpawn;
        public System.Action OneLess;
        public EnemyChances(string EnemyName, float ChanceToSpawn, System.Action OneLess)
        {
            this.EnemyName = EnemyName;
            this.ChanceToSpawn = ChanceToSpawn;
            this.OneLess = OneLess;
        }
        
        public string GetEnemy()
        {
            OneLess?.Invoke();
            return EnemyName;
        }

    }

    Vector3 GetPreferableDestination(EnemyAI enemyAI)
    {
        if (enemyAI == null)
            return Vector3.zero;
        int shortestLane = EnemyQueues.PickRandomLane(EnemyQueues.GetShortestLanes());

        enemyAI.WhichLane   = shortestLane;
        enemyAI.PlaceInLane = EnemyQueues.Queues[shortestLane].Enemies.Count;
        Vector3 Destination = LaneDestination(enemyAI.WhichLane, enemyAI.PlaceInLane);
        EnemyQueues.Queues[shortestLane].Enemies.Add(enemyAI);

        return Destination; //Tells the enemy where to go 
    }

    private void OnDrawGizmos()
    {
        if (_drawGizmos)
        {
            for (int i = 0; i < EnemyQueues.Queues.Count; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(LaneDestination(i, 2+ EnemyQueues.Queues[i].Enemies.Count), Vector3.one);

                Gizmos.color = Color.cyan;
                Gizmos.DrawCube(LaneDestination(i, 1+ EnemyQueues.Queues[i].Enemies.Count), Vector3.one);

                Gizmos.color = Color.blue;
                Gizmos.DrawCube(LaneDestination(i, EnemyQueues.Queues[i].Enemies.Count), Vector3.one);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawCube(DoorSpawnPoint.transform.position + Vector3.up, Vector3.one + Vector3.up);
        }
    }

    public void RemoveEnemy(EnemyAI enemyAI)
    {
        EnemyQueues.RemoveEnemy(enemyAI);
        _currentEnemyAmount--;
        _leaving.Add(enemyAI);
        SortLanes();
    }

    public Vector3 LaneDestination(int i,int place = 0)
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
        return EnemyQueues.GetAmountOfEnemies(); 
    }

    public void LevelSetUp(List<SOspawnEnemy> enemies, SOLevel.SpawnLimit spawnLimit , Vector2 RandomTime, Vector2 WarmUpTime, int maxEnemies)
    {
        _enemyTypes.Clear();
        for (int i = 0; i < enemies.Count; i++)
        {
            _enemyTypes.Add(enemies[i]);
        }
        _spawnLimit = new SOLevel.SpawnLimit() // Deep Copy
        {
            Hipster = spawnLimit.Hipster == 0 ? 100 : spawnLimit.Hipster,
            Ars = spawnLimit.Ars == 0 ? 100 : spawnLimit.Ars,
            OldMan = spawnLimit.OldMan == 0 ? 100 : spawnLimit.OldMan,
            Arsit = spawnLimit.Arsit == 0 ? 100 : spawnLimit.Arsit,
            Cop = spawnLimit.Cop == 0 ? 100 : spawnLimit.Cop,
            Mobster = spawnLimit.Mobster == 0 ? 100 : spawnLimit.Mobster,
            Soldier = spawnLimit.Soldier == 0 ? 100 : spawnLimit.Soldier,
        };
        _randomTime      = RandomTime;
        _currentTimeLeft = Random.Range(WarmUpTime.x,WarmUpTime.y);
        _maxEnemyInGame  = maxEnemies;
        _gm.SetTazdokHp(_gm.MaxTzadokHp);
        _gm.HappyCustomers = 0;
        _gm.Player.GetComponent<ThirdPersonMovement>().FullStamina();
        CustomerCounter = 0;
        _gm.OnStartLevel?.Invoke();
    }

    public void ClearingLevel()
    {
        _leveltimer.SetTimerTo0();
        foreach (EnemyAI enemyAI in EnemyQueues.GetAllEnemies())
        {
            enemyAI.InstantlyRemoveCustomer();
        }
        foreach (EnemyAI enemyAI in _leaving)
        {
            enemyAI.InstantlyRemoveCustomer();
        }
    }

    public void ChangeMaxEnemiesInGame(int maxEnemies)
    {
        _maxEnemyInGame = maxEnemies;
    }

    public EnemyAI GetFirstEnemy()
    {
        return EnemyQueues.GetAllEnemies()[0];
    }


    public void CalmEveryone(float amount)
    {
       
            foreach (EnemyAI enemyAI in EnemyQueues.GetAllEnemies())
            {
                enemyAI.MakeHappier(amount);
            }
       
    }

    public void CalmEveryone(float amount,Vector3 position, float range)
    {       
            foreach (EnemyAI enemyAI in EnemyQueues.GetAllEnemies())
            {
                if (Vector3.Distance(position, enemyAI.transform.position) <= range)
                    enemyAI.MakeHappier(amount);
            }
    }

    public void UpsetEveryone(float amount, Vector3 position, float range)
    {
       
            foreach (EnemyAI enemyAI in EnemyQueues.GetAllEnemies())
            {
                float dist = Vector3.Distance(position, enemyAI.transform.position);
                if (dist <= range && dist > 0.01f)
                    enemyAI.MakeAngrier(amount);
            }
        
    }

    public void SortLanes()
    {
        //Debug.Log("heavy");

        foreach (EnemyAI enemy in EnemyQueues.GetAllPassingEnemies()) // all passers pass
        {
            if (enemy == null) continue;
            if (enemy.PlaceInLane == 0) { continue; }
            int Shortestlane = EnemyQueues.PickRandomLane(EnemyQueues.GetShortestLanesForPassers(enemy));
            int amount = EnemyQueues.UnPassablesInLane(Shortestlane,enemy);
            if (amount < enemy.PlaceInLane)
            {
                RemoveEnemy(enemy);
                EnemyQueues.Queues[Shortestlane].Enemies.Insert(amount, enemy);
                enemy.PlaceInLane = amount;
                enemy.WhichLane = Shortestlane;
                if (enemy is Mobster)
                {
                    foreach(EnemyAI e in EnemyQueues.Queues[Shortestlane].Enemies)
                    {
                        if (e.PlaceInLane > amount)
                        {
                            e.MakeAngrier(10);
                        }
                    }
                }
            }
        }

        foreach (EnemyAI enemy in EnemyQueues.GetAllNonePassingEnemies()) // all normal move to better lanes if possible
        {
            if (enemy == null) continue;
            if (enemy.PlaceInLane == 0) { continue; }
            int Shortestlane = EnemyQueues.PickRandomLane(EnemyQueues.GetShortestLanes());
            int amount = EnemyQueues.Queues[Shortestlane].Enemies.Count;
            if (amount < enemy.PlaceInLane)
            {
                RemoveEnemy(enemy);
                EnemyQueues.Queues[Shortestlane].Enemies.Insert(amount, enemy);
            }
        }

        for (int i = 0; i < EnemyQueues.Queues.Count; i++) // make them actually move
        {
            for (int j = 0; j < EnemyQueues.Queues[i].Enemies.Count; j++)
            {
                EnemyAI enemy = EnemyQueues.Queues[i].Enemies[j];
                enemy.WhichLane = i;
                enemy.PlaceInLane = j;
                enemy.SetDestination(LaneDestination(i, j));
                enemy.InfrontOfLine = j == 0;
            }
        }
    }

}


