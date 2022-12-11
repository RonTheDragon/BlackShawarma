using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodStation : MonoBehaviour , Interactable
{
    [SerializeField]
    private string _info;
    public string Info { get => _info; set => _info = value; }

    [SerializeField] GameObject Panel;

    public void Use(Gun g)
    {
        Panel.SetActive(!Panel.activeSelf);
        if (g != null)
        {
            g.UsingStation();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
