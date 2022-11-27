using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemporaryEnemyMovement : MonoBehaviour
{
    [SerializeField] float Speed = 1;
    [SerializeField] float ReturnWhen;
    [SerializeField] float ReturnTo = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-transform.forward * Speed* Time.deltaTime);

        if (transform.position.z < ReturnWhen)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, ReturnTo);
        }
    }
}
