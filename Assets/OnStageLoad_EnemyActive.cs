using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnStageLoad_EnemyActive : MonoBehaviour
{
    [SerializeField] float delay = 0.1f;
    [SerializeField] List<GameObject> gameObjects = new List<GameObject>();
    void Start()
    {
        StartCoroutine(DelayedSetActive(true));
    }

    IEnumerator DelayedSetActive(bool setActive)
    {
        yield return new WaitForSeconds(delay);
        gameObjects.ForEach(gameObject => { gameObject.SetActive(setActive); });
    }
}
