using System.Collections.Generic;
using UnityEngine;
using System;

public class Chili : MonoBehaviour , Interactable
{
    [SerializeField] private string _info;
    [SerializeField] private float _cooldown = 0;
    [SerializeField] private int _defaultCooldown = 10;
    public string Info { get => _info; set => _info = value; }
    public int chiliAmount = 0;
    public bool NotActive { get => _notActive; set => _notActive = value; }
    public int ChiliBuffTime = 10;
    public Action Used { get => _used; set => _used = value; }
    public List<GameObject> Chilis = new List<GameObject>(3);



    private GameManager _gm;
    private bool _notActive;
    private Action _used;
    private Gun _gun;
    private Collider _collider => GetComponent<Collider>();

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
        _collider.enabled = false;
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
            if (chiliAmount > 0)
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
        switch (chiliAmount)
        {
            case 1: Chilis[0].gameObject.SetActive(false); _collider.enabled = false; break;
            case 2: Chilis[1].gameObject.SetActive(false);  break;
            case 3: Chilis[2].gameObject.SetActive(false);  break;

            default:
                break;
        }
         chiliAmount--;
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
        _collider.enabled = true;
    }
}
