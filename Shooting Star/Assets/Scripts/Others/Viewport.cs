using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//这是一个工具类
public class Viewport : Singleton<Viewport>
{
    public static float minX, maxX, minY, maxY;

    private void Start()
    {
        Camera mainCamera = Camera.main;
        //正交模式下，只会利用Vector3的x和y分量，保证z是原来的z即可
        Vector3 leftBottom = mainCamera.ViewportToWorldPoint(new Vector3(0, 0));
        Vector3 rightTop = mainCamera.ViewportToWorldPoint(new Vector3(1, 1));
        minX = leftBottom.x;
        maxX = rightTop.x;
        minY = leftBottom.y;
        maxY = rightTop.y;
    }
    //限制玩家可移动的范围
    public Vector3 GetMoveablePosition(Vector3 playerPosition, float paddingX, float paddingY)
    {
        Vector3 position = Vector3.zero;
        position.x = Mathf.Clamp(playerPosition.x, minX + paddingX, maxX - paddingX);
        position.y = Mathf.Clamp(playerPosition.y, minY + paddingY, maxY - paddingY);
        position.z = 1.6f;//控制飞机的z是1.6
        return position;
    }
    //测帧率
    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 100;
        style.normal.textColor = Color.white;
        GUILayout.Label("" + (int)(1 / Time.deltaTime), style);
    }
}
