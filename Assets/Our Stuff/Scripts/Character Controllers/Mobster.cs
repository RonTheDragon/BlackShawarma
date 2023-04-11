using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mobster : EnemyAI
{
    // easter egg
    private float _annoyingCooldown;


    protected override void Update()
    {
        base.Update();

        if (_annoyingCooldown > 0) { _annoyingCooldown -= Time.deltaTime; }
        else
        {
            _annoyingCooldown = 3;
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5);

            foreach (Collider collider in colliders)
            {
                EnemyAI enemy = collider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    if (enemy.TempRageMultiplier==1)
                    {
                        enemy.SetTempRage(2.9f, 1.5f);
                    }
                }
            }
        }
    }

    protected override void MadCustomer()
    {
        _spawner.UpsetEveryone(10, transform.position, 5);
        base.MadCustomer();
    }
}
