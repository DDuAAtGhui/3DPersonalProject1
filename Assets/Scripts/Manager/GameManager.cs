using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    #region °ü¸® ¸â¹ö
    public Player player;
    #endregion
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);

        else
            instance = this;
    }

    private void Update()
    {

    }
}
