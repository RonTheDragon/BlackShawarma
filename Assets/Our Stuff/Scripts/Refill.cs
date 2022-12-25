using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refill : MonoBehaviour, Interactable
{
    [SerializeField]
    private string _info;
    public string Info { get => _info; set => _info = value; }

    public void Use(GameObject player)
    {
        throw new System.NotImplementedException();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
