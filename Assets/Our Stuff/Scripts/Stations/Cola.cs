using System.Collections.Generic;
using UnityEngine;
using System;

public class Cola : MonoBehaviour, Interactable
{
    [SerializeField] private string _info;
    public string Info { get => _info; set => _info = value; }
    [ReadOnly] public bool HasCola;
    public bool NotActive { get => _notActive; set => _notActive = value; }
    public Action Used { get => _used; set => _used = value; }
    public GameObject ColaBottle;
    [SerializeField] private Transform _colaParticles;
    private ParticleSystem[] _colaParticleSystems;

    [SerializeField] private float _calmPower;

    private GameManager _gm;
    private EnemySpawner _spawner;
    private bool _notActive;
    private Action _used;
    private Animator _anim =>GetComponent<Animator>();
    private Collider _collider => GetComponent<Collider>();

    public string NotActiveInfo { get => _notActiveInfo; set => _notActiveInfo = value; }
    private string _notActiveInfo;

    private void Start()
    {   
        ColaBottle.SetActive(false);   
        _gm = GameManager.Instance;
        _spawner = _gm.EnemySpawner;
        _notActive = true;
        _collider.enabled = false;
        _colaParticleSystems = new ParticleSystem[_colaParticles.childCount];
        for (int i = 0; i < _colaParticles.childCount; i++)
        {
            _colaParticleSystems[i] = _colaParticles.GetChild(i).GetComponent<ParticleSystem>();
        }
    }

    public void Use(GameObject player)
    {
        _used?.Invoke();
        HasCola = false;
        _notActive = true;
        _anim.SetTrigger("Throw");
    }

    public void Activate()
    {
        ColaBottle.SetActive(true);
        HasCola = true;
        _notActive = false;
        _collider.enabled = true;
    }

    public void ColaExplodes()
    {
        _colaParticles.position = ColaBottle.transform.position;
        foreach (ParticleSystem particle in _colaParticleSystems)
        {
            particle.Play();
        }
        ColaBottle.SetActive(false);
        _spawner.CalmEveryone(_calmPower);
    }
    public void UpdateInfo()
    {

    }
}
