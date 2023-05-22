using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ztdaka : MonoBehaviour, Interactable
{
    [SerializeField] private string _info;
    private GameManager _gm;
    public int ztdakaBuffTime = 10;
    [SerializeField] private int _defaultCooldown = 10;
    [SerializeField] private float _cooldown = 10;
    [SerializeField] private float newMultiplier;

    public string Info { get => _info; set => _info = value; }

    private bool _active;
    public bool NotActive { get => _active; set => _active = value; }
    private Action _used;
    public Action Used { get => _used; set => _used = value; }

    private void Update()
    {
        if (_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;
        }
    }

    public void Use(GameObject gameManager)
    {
        if (_cooldown > 0)
        {
            return;
        }
        _used?.Invoke();
        _gm = GameObject.Find("Game Manager").GetComponent<GameManager>();
        _gm.ztdakaMultiplier = newMultiplier;
        Invoke("ztdakaBuffHandler", ztdakaBuffTime);
        _cooldown = _defaultCooldown;
    }

    private void ztdakaBuffHandler()
    {
        _gm.ztdakaMultiplier = 1;
    }
}