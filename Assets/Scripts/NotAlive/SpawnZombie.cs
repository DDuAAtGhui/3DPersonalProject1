using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnZombie : MonoBehaviour
{
    [SerializeField] GameObject[] zombiePrefabs;
    [SerializeField] Transform spawnTF;
    [SerializeField] GameObject spawnCircle;
    [SerializeField] float spawnScale = 1f;
    [SerializeField] float spawnInterval = 0.5f;
    float timer = 0f;
    void Start()
    {

    }

    void Update()
    {
        timer += Time.deltaTime;
        spawnCircle.transform.localScale = new Vector3(spawnScale, spawnScale, spawnScale);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            if (timer >= spawnInterval)
            {
                timer = 0f;

                Instantiate(zombiePrefabs[Random.Range(0, zombiePrefabs.Length)], spawnCircle.transform.position
                    + new Vector3(Random.Range(-spawnCircle.transform.localScale.x / 2, spawnCircle.transform.localScale.x / 2), 0,
                    Random.Range(-spawnCircle.transform.localScale.z / 2, spawnCircle.transform.localScale.z / 2)),
                Quaternion.LookRotation((spawnTF.position - GameManager.instance.player.transform.position).normalized));
            }
        }
    }
}
