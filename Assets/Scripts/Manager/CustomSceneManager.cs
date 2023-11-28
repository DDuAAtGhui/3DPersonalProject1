using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    [SerializeField] GameObject migratePackage;

    public static CustomSceneManager instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);


        else
            instance = this;

        SceneManager.sceneLoaded += onSceneLoaded;
        SceneManager.sceneUnloaded += onSceneUnLoaded;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {

    }
    public void onSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.buildIndex != 0)
        {
            Debug.Log("¾À ·ÎµåµÊ");
            GameObject go;

            if (GameObject.Find(migratePackage.name) == null)
            {
                go = Instantiate(migratePackage);
                go.AddComponent<InitialDead>();
            }

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (var aa in enemies)
            {
                aa.SetActive(false);
            }

            GameManager.instance.LoadParkourPositionObejcts();
        }

        if (arg0.name == "Hospital")
        {
            AudioManager.instance.PlayBGM("Hospital");
        }
    }

    private void onSceneUnLoaded(Scene arg0)
    {

    }
}
