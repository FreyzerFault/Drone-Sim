using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public static Camera Camera { 
        get => Camera.main;
        set
        {
            Camera.main.gameObject.SetActive(false);
            value.gameObject.SetActive(true);
        }
    }
}
