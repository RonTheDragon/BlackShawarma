using System.Collections.Generic;
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
    [SerializeField]            List<SOspawnEnemy> _enemyTypes          = new List<SOspawnEnemy>();
    [ReadOnly] [SerializeField] private SOLevel.SpawnLimit _spawnLimit;
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
        GameObject Enemy   = ObjectPooler.Instance.SpawnFromPool(ChooseEnemy(), DoorSpawnPoint.position, DoorSpawnPoint.rotation);
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

    private string ChooseEnemy()
    {
        SOspawnEnemy currentSpawnable = _enemyTypes[0];

        List<EnemyChances> chances = new List<EnemyChances>();

        if (currentSpawnable.Hipster > 0 && _spawnLimit.Hipster>0) chances.Add(new EnemyChances("FriesGuy", currentSpawnable.Hipster, () => _spawnLimit.Hipster--));
        if (currentSpawnable.OldMan > 0 && _spawnLimit.OldMan>0) chances.Add(new EnemyChances("EggplantGuy", currentSpawnable.OldMan, () => _spawnLimit.OldMan--));
        if (currentSpawnable.Ars > 0 && _spawnLimit.Ars>0) chances.Add(new EnemyChances("FalafelGuy", currentSpawnable.Ars, () => _spawnLimit.Ars--));
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

            //for (int i = 0; i < 50; i++)
            //{
            //    int chosen = Random.Range(0, chances.Count);
            //    //Debug.Log($"tries to spawn {chances[chosen].EnemyName}");
            //    if (chances[chosen].ChanceToSpawn >= Random.Range(0f, 1f))
            //    {
            //        return chances[chosen].EnemyName;
            //    }
            //}
            //Debug.LogWarning("Random broken in Spawnable Enemy");
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

    private int[] GetPassableCustomers()
    {
        int[] passList = new int[CustomersInLane.Length];

        return passList;
    }

    Vector3 GetPreferableDestination(EnemyAI enemyAI)
    {
        if (enemyAI == null)
            return Vector3.zero;
        int SmallestAmountOfPeople = 10000;
        int SmallestLane           = 0;

        for (int i = 0; i < CustomersInLane.Length; i++) //Check the Lowest line length
        {
            int[] a = CustomersInLane;
            if (enemyAI.PassesInLines)
            {
                a = GetPassableCustomers();
            }


            //Debug.Log($"{CustomersInLane[i]} < {SmallestAmountOfPeople} = {CustomersInLane[i] < SmallestAmountOfPeople}");
            if (a[i] < SmallestAmountOfPeople)
            {
                SmallestAmountOfPeople = a[i];
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

                        if (e.PassesInLines) e.PassInLine(e is Mobster);

                        return;
                    }
                }
            }
            n++;
        }
    }

    public int AddInFrontOfLane(int Lane, int spotInLane, bool annoying) //for soldier (future use)
    {
        EnemyAI[] stack = new EnemyAI[spotInLane];

        foreach (EnemyAI e in _enemies)
        {
            if (e.WhichLane == Lane && e.PlaceInLane < spotInLane)
            {
                stack[e.PlaceInLane] = e;
            }
        }

        for (int i = stack.Length-1; i > -1; i--)
        {
            EnemyAI e = stack[i];

            if (e.CanBePassed)
            {
                e.PlaceInLane++;
                e.SetDestination(LaneDestination(Lane, e.PlaceInLane));
                if (annoying) e.MakeAngrier(10);
            }
            else
            {
                return e.PlaceInLane + 1;
            }
        }

        return 0;
    }

    public void RemoveEnemy(EnemyAI enemyAI)
    {
        _enemies.Remove(enemyAI);
        _currentEnemyAmmout--;
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
       return _enemies.Count;
        
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
            Cop = spawnLimit.Cop == 0 ? 100 : spawnLimit.Cop,
            Mobster = spawnLimit.Mobster == 0 ? 100 : spawnLimit.Mobster,
            Soldier = spawnLimit.Soldier == 0 ? 100 : spawnLimit.Soldier,
        };
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


    public void CalmEveryone(float amount)
    {
        foreach(EnemyAI enemyAI in _enemies)
        {
            enemyAI.MakeHappier(amount);
        }
    }

    public void CalmEveryone(float amount,Vector3 position, float range)
    {
        foreach (EnemyAI enemyAI in _enemies)
        {
            if (Vector3.Distance(position,enemyAI.transform.position) <= range)
            enemyAI.MakeHappier(amount);
        }
    }

    public void UpsetEveryone(float amount, Vector3 position, float range)
    {
        foreach (EnemyAI enemyAI in _enemies)
        {
            float dist = Vector3.Distance(position, enemyAI.transform.position);
            if (dist <= range && dist > 0.01f)
                enemyAI.MakeAngrier(amount);
        }
    }

    public void FixShortLines()
    {
        for (int i = 0; i < CustomersInLane.Length; i++)
        {
            CollectFromLongestLane(i);
        }
    }
}


