using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject lookAt;
    Camera mainCamera;
    List<GameObject> searchedObjects = new List<GameObject>();
    [SerializeField] LayerMask whatIsFadable;

    RaycastHit[] rayHits_Player;
    Vector3 direction;
    float distance;
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    void Start()
    {

    }
    private void Update()
    {
        CheckMedialObjects();
    }


    private void LateUpdate()
    {
    }


    bool check;
    private void CheckMedialObjects()
    {
        direction = (GameManager.instance.player.transform.position - mainCamera.transform.position).normalized;
        distance = Vector3.Distance(mainCamera.transform.position, GameManager.instance.player.transform.position);
        rayHits_Player = Physics.RaycastAll(mainCamera.transform.position, direction,
           distance, whatIsFadable);

        check = Physics.Raycast(mainCamera.transform.position, direction, out RaycastHit hitInfo, distance, whatIsFadable);

        foreach (RaycastHit hit in rayHits_Player)
        {
            if (!searchedObjects.Contains(hit.collider.gameObject))
                searchedObjects.Add(hit.collider.gameObject);

        }


        if (!check)
        {
            ChangeObjectRenderingMode(1f);
            searchedObjects.Clear();
        }

        else
        {
            ChangeObjectRenderingMode(0.4f);
        }

        ////오브젝트 탐지되지 않으면 리스트 초기화
        //searchedObjects.RemoveAll(x => !rayHits_Player.Any(hit => hit.collider.gameObject == x));
    }

    #region 오브젝트 투명화

    // 0 = Opaque ~~
    private enum RenderingMode { Opaque, Cutout, Transparent, Fade }

    // 런타임 렌더링 모드 변화. ShaderLab 키워드들
    private void SetRenderingMode(Material material, RenderingMode renderingMode, float alpha = 1f)
    {
        material.DisableKeyword("_ALPHATEST_ON");
        material.DisableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        switch (renderingMode)
        {
            case RenderingMode.Opaque:
                material.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetFloat("_ZWrite", 1);
                material.renderQueue = -1;
                break;

            case RenderingMode.Cutout:
                material.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                material.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                material.SetFloat("_ZWrite", 1);
                material.renderQueue = 2450;
                break;

            case RenderingMode.Fade:
                material.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetFloat("_ZWrite", 0);
                material.renderQueue = 3000;
                break;

            case RenderingMode.Transparent:
                material.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetFloat("_ZWrite", 0);
                material.renderQueue = 3000;
                break;
        }

        Color color = material.color;
        color.a = alpha;
        material.color = color;
    }

    //렌더러에 달린 매터리얼 정보 가져와서 렌더링 모드 변환시키기
    private void ChangeObjectRenderingMode(RenderingMode mode, float alpha = 1f)
    {
        if (searchedObjects.Count > 0)
        {
            foreach (var obj in searchedObjects)
            {
                Renderer renderer = obj.GetComponent<Renderer>();

                if (renderer != null)
                {
                    Material material = renderer.material;

                    SetRenderingMode(material, mode, alpha);
                }
            }
        }
    }
    private void ChangeObjectRenderingMode(float alpha = 1f)
    {
        float tempAlpha = alpha;

        if (searchedObjects.Count > 0)
        {
            foreach (var obj in searchedObjects)
            {
                Renderer renderer = obj.GetComponent<Renderer>();

                if (renderer != null)
                {
                    Material material = renderer.material;

                    SetRenderingMode(material, RenderingMode.Transparent, alpha);

                    Color color = material.color;

                    color.a = alpha;

                    material.color = color;
                }
            }
        }

        if (!check)
        {
            foreach (var obj in searchedObjects)
            {
                Renderer renderer = obj.GetComponent<Renderer>();

                if (renderer != null)
                {
                    Material material = renderer.material;

                    Color color = material.color;

                    color.a = tempAlpha;

                    material.color = color;
                }
            }

        }
    }
    #endregion
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (mainCamera != null)
            Gizmos.DrawRay(mainCamera.transform.position, direction);
    }
}
