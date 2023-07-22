using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class Chili : MonoBehaviour , Interactable
{
    [SerializeField] private string _info;
    [SerializeField] private float _cooldown = 0;
    [SerializeField] private int _defaultCooldown = 10;
    public string Info { get => _info; set => _info = value; }
    public int chiliammount = 0;
    public bool NotActive { get => _notActive; set => _notActive = value; }
    public int ChiliBuffTime = 10;
    public Action Used { get => _used; set => _used = value; }
    public List<GameObject> Chilis = new List<GameObject>(3);



    private GameManager _gm;
    private bool _notActive;
    private Action _used;
    private Gun _gun;

    private void Start()
    {
        foreach (GameObject go in Chilis)
        {
            go.SetActive(false);
        }
        _gm = GameManager.Instance;
        _gun = _gm.Player.GetComponent<Gun>();
        _gm.OnStartLevel += () => _cooldown = -1;
        _notActive = true;
    }

    private void Update()
    {
        if (_cooldown > 0)
        {
            _cooldown -= Time.deltaTime;
        }
        else if (_cooldown< 0)
        {
            _cooldown = 0;
            if (chiliammount > 0)
            {
                _notActive = false;
            }
        }
    }

    public void Use(GameObject player)
    {
        if (_cooldown > 0)
        {
            return;
        }
        _used?.Invoke();
        _gm.UsedChili = true;
        switch (chiliammount)
        {
            case 1: Chilis[0].gameObject.SetActive(false);  break;
            case 2: Chilis[1].gameObject.SetActive(false);  break;
            case 3: Chilis[2].gameObject.SetActive(false);  break;

            default:
                break;
        }
         chiliammount--;
        Invoke("ChiliBuffHandler", ChiliBuffTime);
        _cooldown = _defaultCooldown;
        _notActive = true;
        _gun.ChiliParticles(true);
    }
    private void ChiliBuffHandler()
    {
        _gm.UsedChili = false;
        _gun.ChiliParticles(false);
    }

    public void Activate()
    {
        _notActive = false;
    }
}
