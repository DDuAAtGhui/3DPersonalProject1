using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnStageLoad_EnemyActive : MonoBehaviour
{
    [SerializeField] bool isSpawnActive = true;
    [SerializeField] float delay = 0.1f;
    List<GameObject> gameObjects = new List<GameObject>();

    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            gameObjects.Add(transform.GetChild(i).gameObject);
            gameObjects[i].SetActive(false);
        }
        if (isSpawnActive)
            StartCoroutine(DelayedSetActive(true));
    }

    IEnumerator DelayedSetActive(bool setActive)
    {
        yield return new WaitForSeconds(delay);
        gameObjects.ForEach(gameObject => { gameObject.SetActive(setActive); });
    }
}
