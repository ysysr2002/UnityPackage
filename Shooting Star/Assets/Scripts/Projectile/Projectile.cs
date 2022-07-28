using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//子弹基类
public class Projectile : MonoBehaviour
{
    [SerializeField] float moveSpeed = 10f;
    [SerializeField] Vector2 moveDirection;

    private void OnEnable()
    {
        StartCoroutine(nameof(MoveDirectly));
    }

    IEnumerator MoveDirectly()
    {
        while (gameObject.activeSelf)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
