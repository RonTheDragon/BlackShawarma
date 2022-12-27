using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int tzadokHp =3 ;
    public int MaxFillers = 4;
    void Awake()
    {
         instance = this;
    }
}
