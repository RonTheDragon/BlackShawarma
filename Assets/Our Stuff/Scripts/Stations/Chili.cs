using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Chili : MonoBehaviour
{
    [SerializeField] private string _info;
    [SerializeField] private float _cooldown = 0;
    [SerializeField] private int _defaultCooldown = 10;
    public string Info { get => _info; set => _info = value; }
    public int chiliammount = 0;
    public bool NotActive { get => _active; set => _active = value; }
    public int ChiliBuffTime = 10;
    public Action Used { get => _used; set => _used = value; }
    public List<GameObject> Chilis = new List<GameObject>(3);



    private GameManager _gm;
    private bool _active;
    private Action _used;

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
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _gm.usedChili = true;
         chiliammount--;
        switch (chiliammount)
        {
            case 1: Chilis[2].gameObject.SetActive(false);  break;
            case 2: Chilis[1].gameObject.SetActive(false);  break;
            case 3: Chilis[0].gameObject.SetActive(false);  break;

            default:
                break;
        }
        Invoke("ChiliBuffHandler", ChiliBuffTime);
        _cooldown = _defaultCooldown;
    }
    private void ChiliBuffHandler()
    {
        _gm.usedChili = false;
    }
}
