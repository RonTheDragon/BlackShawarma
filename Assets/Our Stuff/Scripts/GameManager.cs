using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int tazdokHp =3 ;
    public int MaxFillers = 4;
    void Awake()
    {
         instance = this;
    }
}
