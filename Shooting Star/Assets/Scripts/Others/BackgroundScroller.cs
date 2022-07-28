using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//控制背景做卷轴移动
public class BackgroundScroller : MonoBehaviour
{
    Material material;
    [SerializeField] Vector2 velocity;
    private void Awake()
    {
        material = GetComponent<MeshRenderer>().material;
    }
    private void Update()
    {
        material.mainTextureOffset += Time.deltaTime * velocity;
    }
}
