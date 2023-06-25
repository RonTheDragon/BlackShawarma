using UnityEngine;
using System;

public class Supplies : MonoBehaviour, HoldInteractable
{
    [SerializeField] private string _info;
    public string Info { get => _info; set => _info = value; }

    private bool _notActive;
    public bool NotActive { get => _notActive; set => _notActive = value; }
    private Action _used;
    public Action Used { get => _used; set => _used = value; }

    [SerializeField] private bool _holdToUse;
    public bool HoldToUse { get => _holdToUse; set => _holdToUse = value; }

    [SerializeField] private float _useDuration;
    public float UseDuration { get => _useDuration; set => _useDuration = value; }

    private GameManager _gm;

    private void Start()
    {
        _gm = GameManager.Instance;
        _gm.OnPlaceDownSack += () => _notActive = false;
    }

    public void Use(GameObject player)
    {
        _used?.Invoke();

        BuildOrder b = player.GetComponent<BuildOrder>();

        if (b != null)
        {
            b.HasSupplies = true;
            b.Sack.SetActive(true);
            _notActive = true;
            _gm.OnPickUpSack?.Invoke();
        }
    }
}
