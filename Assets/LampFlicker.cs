using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LampFlickerOption
{
    Random = 1
}
public class LampFlicker : MonoBehaviour
{
    [Tooltip("�����̱� ���ϴ� ����")][SerializeField] GameObject[] flickeringLights;
    [SerializeField] LampFlickerOption option;

    [Header("���� ����Ҷ� ���")]
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
