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
                        enemy.MakeCalmer(3, 0.5f);
                        
                    }
                }
            }
            if (foundGrandpa) 
            {
                MakeCalmer(3, 0.5f);
            }
        }
    }

    public override void Spawn(EnemySpawner spawner, int num)
    {
        base.Spawn(spawner, num);
        PassesInLines = true;
        PassInLine(spawner);
    }

    protected override void HappyCustomer()
    {
        _spawner.CalmEveryone(10,transform.position,5);
        RemoveCustomer();
    }
}
