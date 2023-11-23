using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LampFlickerOption
{
    Random = 1
}
public class LampFlicker : MonoBehaviour
{
    [Tooltip("깜빡이길 원하는 조명")][SerializeField] GameObject[] flickeringLights;
    [SerializeField] LampFlickerOption option;

    [Header("랜덤 사용할때 사용")]
    [SerializeField] float maxRandomValue = 1f;
    [SerializeField] float minRandomValue = 0.01f;


    void Start()
    {
        switch (option)
        {
            case LampFlickerOption.Random:
                StartCoroutine(RandomFlickerAtOnce());
                break;

        }
    }


    IEnumerator RandomFlickerAtOnce()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minRandomValue, maxRandomValue));

            foreach (var light in flickeringLights)
            {
                light.SetActive(false);
            }

            yield return new WaitForSeconds(Random.Range(minRandomValue, maxRandomValue));

            foreach (var light in flickeringLights)
            {
                light.SetActive(true);
            }

        }
    }
}
