using System.Collections.Generic;
using UnityEngine;
using System;

public class LafaPile : MonoBehaviour, Interactable
{
    [SerializeField] private string _info;
    public string Info { get => _info; set => _info = value; }
    public int LafaAmount = 0;
    public bool NotActive { get => _notActive; set => _notActive = value; }
    public Action Used { get => _used; set => _used = value; }
    public List<GameObject> Lafas = new List<GameObject>(3);



    private GameManager _gm;
    private bool _notActive;
    private Action _used;
    private Gun _gun;
    private Collider _collider => GetComponent<Collider>();

    private void Start()
    {
        foreach (GameObject go in Lafas)
        {
            go.SetActive(false);
        }
        _gm = GameManager.Instance;
        _gun = _gm.Player.GetComponent<Gun>();
        _notActive = true;
        _collider.enabled = false;
        _gun.OnLafaShoot += () => _notActive = false;
    }

    public void Use(GameObject player)
    {
        _used?.Invoke();
        _gun.SetLafa(true);
        switch (LafaAmount)
        {
            case 1: Lafas[0].gameObject.SetActive(false); _collider.enabled = false; break;
            case 2: Lafas[1].gameObject.SetActive(false); break;
            case 3: Lafas[2].gameObject.SetActive(false); break;
            case 4: Lafas[3].gameObject.SetActive(false); break;
            case 5: Lafas[4].gameObject.SetActive(false); break;
            case 6: Lafas[5].gameObject.SetActive(false); break;

            default:
                break;
        }
        LafaAmount--;
        _notActive = true;
       // _gun.ChiliParticles(true);
    }

    public void Activate()
    {
        _notActive = false;
        _collider.enabled = true;
    }
}
