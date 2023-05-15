using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Police : EnemyAI
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

            bool mobsterFound = false;
            foreach (Collider collider in colliders)
            {
                EnemyAI enemy = collider.GetComponent<EnemyAI>();
                if (enemy != null)
                {
                    if (enemy is Mobster)
                    {
                        enemy.SetTempRage(3.1f, 1.5f);
                        mobsterFound = true;
                    }
                }
            }
            if (mobsterFound)
            {
                SetTempRage(3, 2f);
            }
        }
    }

    protected override void MadCustomer()
    {
        if (_coffee) base.MadCustomer();
        else { _gm.TazdokTakeDamage(10); RemoveCustomer(); }
    }
}
