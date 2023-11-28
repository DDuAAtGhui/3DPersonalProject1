using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClearTrigger : MonoBehaviour
{
    [SerializeField] List<GameObject> hideObejcts = new List<GameObject>();
    [SerializeField] GameObject clearObject;
    private void Start()
    {
        hideObejcts = GameObject.FindGameObjectsWithTag("HideWhenClearUI").ToList();
        clearObject = GameObject.Find("UI").transform.Find("Clear").gameObject;


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.instance.isClear = true;

            foreach (GameObject go in hideObejcts)
            {
                go.SetActive(false);
            }

            clearObject.SetActive(true);
            AudioManager.instance.StopBGM();
        }
    }
}
