using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level")]
public class SOLevel : ScriptableObject
{
    public List<SOspawnEnemy> Enemies = new List<SOspawnEnemy>();
    public Vector2 RandomSpawnRate = new Vector2(30, 60);
    public Vector2 WarmUpTime = new Vector2(0, 3);
    public int MaxEnemiesAtOnce = 10;

    [Header("Timer")]
    public int Seconds = 10;
}
