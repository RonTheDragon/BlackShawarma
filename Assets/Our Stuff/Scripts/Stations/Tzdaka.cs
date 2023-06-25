using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ztdaka : MonoBehaviour, Interactable
{
    [SerializeField] private string _info;
    private GameManager _gm;

    public string Info { get => _info; set => _info = value; }

    private bool _active;
    public bool NotActive { get => _active; set => _active = value; }
    private Action _used;
    public Action Used { get => _used; set => _used = value; }

    private void Update()
    {

    }

    public void Use(GameObject gameManager)
    {

        _used?.Invoke();
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _gm.tzdakaActivated = true;
    }

   
}