using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//自动禁用脚本
public class AutoDeactivate : MonoBehaviour
{
    [SerializeField] float lifetime = 2f;//生命周期
    [SerializeField] bool destroyGameobject;//是否希望删除该物体
    WaitForSeconds waitLifetime;

    private void Awake()
    {
        waitLifetime = new WaitForSeconds(lifetime);
    }
    private void OnEnable()
    {
        StartCoroutine(nameof(DeactivateCoroutine));
    }

    IEnumerator DeactivateCoroutine()
    {
        yield return waitLifetime;
        if (destroyGameobject)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
