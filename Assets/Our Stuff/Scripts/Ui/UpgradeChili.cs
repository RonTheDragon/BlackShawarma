using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeChili : MonoBehaviour
{
    GameManager gameManager;
    public int chiliprice = 10;
    private void UpgradeChiliy(int level)
    {
        Debug.Log($"Upgraded Coffee Level {level}");

        switch (level)
        {
            case 0: chiliammount[0] = 1; break;
            case 1: chiliammount[1] = 2; break;
            case 2: chiliammount[2] = 3; break;
        }
    }
    List<int> chiliammount = new List<int>(3);

    
}
