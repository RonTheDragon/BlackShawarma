using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStation : MonoBehaviour , Interactable
{
    [SerializeField]
    private string _info;
    public string Info { get => _info; set => _info = value; }

    [SerializeField] GameObject Panel;

    public void Use(GameObject player)
    {
        Panel.SetActive(!Panel.activeSelf);
        Gun g = player.GetComponent<Gun>();
        if (g != null) 
        {
            g.CantShoot = !g.CantShoot;
            g.cinemachine.enabled = !g.cinemachine.enabled;
            ThirdPersonMovement tpm = g.GetComponent<ThirdPersonMovement>();
            if (tpm!=null)tpm.enabled = !tpm.enabled;
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = !Cursor.visible;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
