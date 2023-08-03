using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : EnemyAI
{
    // easter egg
    private float _grandpaCheckCooldown;


    protected override void Update()
    {
        base.Update();
        
        if (_grandpaCheckCooldown>0 ) { _grandpaCheckCooldown -= Time.deltaTime; } else
        {
            _grandpaCheckCooldown = 3;
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5);

            bool foundGrandpa = false;
            foreach (Collider collider in colliders)
            {
                EnemyAI enemy = collider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    if (enemy.Old)
                    {
                        foundGrandpa = true;
                        enemy.SetTempRage(3.1f, 0.5f);
                        
                    }
                }
            }
            if (foundGrandpa) 
            {
                SetTempRage(3, 0.5f);
            }
        }
    }

    protected override void HappyCustomer()
    {
        _enemySound.PlayExitSound();
        _spawner.CalmEveryone(10,transform.position,5);
        _gm.CM.AddCombo();
        RemoveCustomer();
        
    }
}
