using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

[System.Serializable]
public class QueueSystem
{
    public List<MyQueue> Queues = new List<MyQueue>();

    public EnemyAI[] GetAllEnemies()
    {
        EnemyAI[] enemyAIs = new EnemyAI[GetAmountOfEnemies()];
        int count = 0;
        for (int i = 0; i < Queues.Count; i++)
        {
            for (int j = 0; j < Queues[i].Enemies.Count; j++)
            {
                enemyAIs[count] = Queues[i].Enemies[j];
                count++;
            }
        }
        return enemyAIs;
    }

    public EnemyAI[] GetAllPassingEnemies()
    {
        List<EnemyAI> enemyAIs = new List<EnemyAI>();
        for (int i = 0; i < Queues.Count; i++)
        {
            for (int j = 0; j < Queues[i].Enemies.Count; j++)
            {
                if (Queues[i].Enemies[j].PassesInLines)
                {
                    enemyAIs.Add(Queues[i].Enemies[j]);
                }
            }
        }
        return enemyAIs.ToArray();
    }

    public EnemyAI[] GetAllNonePassingEnemies()
    {
        List<EnemyAI> enemyAIs = new List<EnemyAI>();
        for (int i = 0; i < Queues.Count; i++)
        {
            for (int j = 0; j < Queues[i].Enemies.Count; j++)
            {
                if (!Queues[i].Enemies[j].PassesInLines)
                {
                    enemyAIs.Add(Queues[i].Enemies[j]);
                }
            }
        }
        return enemyAIs.ToArray();
    }


    public int GetAmountOfEnemies()
    {
        int amount = 0;
        foreach(MyQueue queue in Queues)
        {
            amount += queue.Enemies.Count;
        }
        return amount;
    }

    public void RemoveEnemy(EnemyAI remove)
    {
        foreach(MyQueue queue in Queues) 
        {
            for (int i = 0; i < queue.Enemies.Count; i++)
            {
                if (queue.Enemies[i] == remove)
                {
                    queue.Enemies.RemoveAt(i);
                }
            }
        }
    }

    public int[] GetShortestLanes()
    {
        int shortestLength = int.MaxValue;
        List<int> shortestLanes = new List<int>();

        for (int i = 0; i < Queues.Count; i++)
        {
            int laneLength = Queues[i].Enemies.Count;
            if (laneLength < shortestLength)
            {
                shortestLength = laneLength;
                shortestLanes.Clear();
                shortestLanes.Add(i);
            }
            else if (laneLength == shortestLength)
            {
                shortestLanes.Add(i);
            }
        }

        return shortestLanes.ToArray();
    }

    public int[] GetShortestLanesForPassers(EnemyAI dontInclude = null)
    {
        int shortestLength = int.MaxValue;
        List<int> shortestLanes = new List<int>();

        for (int i = 0; i < Queues.Count; i++)
        {
            int laneLength = UnPassablesInLane(i, dontInclude);
            if (laneLength < shortestLength)
            {
                shortestLength = laneLength;
                shortestLanes.Clear();
                shortestLanes.Add(i);
            }
            else if (laneLength == shortestLength)
            {
                shortestLanes.Add(i);
            }
        }

        return shortestLanes.ToArray();
    }

    public int[] GetLongestLanes()
    {
        int longestLength = 0;
        List<int> longestLanes = new List<int>();

        for (int i = 0; i < Queues.Count; i++)
        {
            int laneLength = Queues[i].Enemies.Count;
            if (laneLength > longestLength)
            {
                longestLength = laneLength;
                longestLanes.Clear();
                longestLanes.Add(i);
            }
            else if (laneLength == longestLength)
            {
                longestLanes.Add(i);
            }
        }

        return longestLanes.ToArray();
    }

    public int PickRandomLane(int[] lanes)
    {
        int randomIndex = Random.Range(0, lanes.Length);
        return lanes[randomIndex];
    }

    public int UnPassablesInLane(int queueNum, EnemyAI dontInclude=null)
    {
        for (int i = Queues[queueNum].Enemies.Count - 1; i >= 0; i--)
        {
            if (Queues[queueNum].Enemies[i].CantBePassed && Queues[queueNum].Enemies[i] != dontInclude)
            {
                return i+1;
            }
        }
        return 0;
    }
}
