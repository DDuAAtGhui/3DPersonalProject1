using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSceneManager : MonoBehaviour
{
    [SerializeField] GameObject migratePackage;
    private void Awake()
    {
        SceneManager.sceneLoaded += onSceneLoaded;
        SceneManager.sceneUnloaded += onSceneUnLoaded;
        DontDestroyOnLoad(gameObject);
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
        }
    }

    private void onSceneUnLoaded(Scene arg0)
    {

    }
}
