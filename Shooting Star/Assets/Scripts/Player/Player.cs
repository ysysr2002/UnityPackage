using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//玩家
[RequireComponent(typeof(Rigidbody2D))]//自动为游戏对象加上某个组件（该组件将无法被删除）
public class Player : MonoBehaviour
{
    [SerializeField] PlayerInput input;//要将可编程脚本拖入面板中   
    [SerializeField] float moveSpeed = 1f;//飞机移动速度
    [SerializeField] float accelarationTime = 5;//加速时间
    [SerializeField] float decelarationTime = 10;//减速时间
    [SerializeField] float paddingX = 0.8f;//水平偏移量
    [SerializeField] float paddingY = 0.28f;//水平偏移量
    [SerializeField] float rotationAngle = 20f;//旋转角度
    [SerializeField] GameObject projectile;//子弹预制体
    [SerializeField] Transform muzzle;//枪口
    [SerializeField] float fireInterval = 0.2f;//发射间隔

    new Rigidbody2D rigidbody;//rigidbody与父类中的某个成员重名了，可以使用new来隐藏
    WaitForSeconds waitForNextFire;//等待下次发射

    private void OnEnable()//订阅
    {
        input.onMove += Move;
        input.onStopMove += StopMove;
        input.onFire += Fire;
        input.onStopFire += StopFire;
    }
    private void OnDisable()//退订
    {
        input.onMove -= Move;
        input.onStopMove -= StopMove;
        input.onFire -= Fire;
        input.onStopFire -= StopFire;
    }
    private void Awake()
    {
        Application.targetFrameRate = 60;//强制锁帧
        rigidbody = GetComponent<Rigidbody2D>();
        waitForNextFire = new WaitForSeconds(fireInterval);
    }
    private void Start()
    {
        rigidbody.gravityScale = 0;//将重力设为0
        input.EnableGamePlayInput();//激活GamPlay动作表
    }

    #region MOVE
    //使用协程代替Update，大幅提升性能
    IEnumerator PlayerPositionLimitCoroutine()
    {
        while (true)
        {
            transform.position = Viewport.Instance.GetMoveablePosition(transform.position, paddingX, paddingY);//进行位置限制
            yield return null;//挂起到下一帧
        }
        #region yield
        //yield break -> 立即终止
        //yield return null 或 yield return constant(常数) -> 挂起到下一帧
        //yield return new WaitForSeconds(float seconds) -> 挂起一定时间（受到timeScale影响）
        //yield return new WaitForSecondsRealtime(float seconds) -> 挂起一定时间（不受timeScale影响）
        //yield return new WaitUntil( ()=> bool judge ) -> 挂起至条件为真时
        //yield return StartCoroutine(IEnumerator otherCoroutine()) -> 挂起至参数协程的结束
        #endregion
    }

    private Coroutine moveCoroutine = null;//移动协程
    IEnumerator StartMoveCoroutine(Vector2 dstVelocity, Quaternion dstRotation)
    {
        float timer = 0;
        while (timer < accelarationTime)
        {
            timer += Time.fixedDeltaTime;
            rigidbody.velocity = Vector2.Lerp(rigidbody.velocity, dstVelocity, timer / accelarationTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, dstRotation, timer / accelarationTime);
            yield return null;//在while循环中挂起一帧
        }
    }
    IEnumerator StopMoveCoroutine(Vector2 dstVelocity, Quaternion dstRotation)
    {
        float timer = 0;
        while (timer < decelarationTime)
        {
            timer += Time.fixedDeltaTime;
            if (IfPositionOnTheEdge(transform.position))
            {
                rigidbody.velocity = Vector2.zero;//移动到边缘时理论上速度是最大值，如果此时停止飞机将会飞出去，故velocity应该要突变为0
            }
            else
            {
                rigidbody.velocity = Vector2.Lerp(rigidbody.velocity, dstVelocity, timer / decelarationTime);//velocity缓变为0
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, dstRotation, timer / decelarationTime);
            yield return null;//在while循环中挂起一帧
        }
    }

    //传入一个位置，判断是不是在边缘
    private bool IfPositionOnTheEdge(Vector3 position)
    {
        float minDistance = 0.1f;
        if (Mathf.Abs(position.x - (Viewport.maxX - paddingX)) <= minDistance)
            return true;
        else if (Mathf.Abs(position.x - (Viewport.minX + paddingX)) <= minDistance)
            return true;
        else if (Mathf.Abs(position.y - (Viewport.maxY - paddingY)) <= minDistance)
            return true;
        else if (Mathf.Abs(position.y - (Viewport.minY + paddingY)) <= minDistance)
            return true;
        return false;
    }

    private bool isFirstTime = true;//防止由于同时按键的问题，启用了多个协程
    void Move(Vector2 moveInput)//moveInput就是context.ReadValue<Vector2>()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        Quaternion rotation = Quaternion.AngleAxis(moveInput.y * rotationAngle, Vector3.right);
        moveCoroutine = StartCoroutine(StartMoveCoroutine(moveInput.normalized * moveSpeed, rotation));
        if (isFirstTime)
        {
            StartCoroutine(nameof(PlayerPositionLimitCoroutine));
            isFirstTime = false;
        }  
    }
    void StopMove()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(StopMoveCoroutine(Vector2.zero, Quaternion.identity));
        if (!isFirstTime)
        {
            StopCoroutine(nameof(PlayerPositionLimitCoroutine));
            isFirstTime = true;
        }    
    }
    #endregion

    #region FIRE
    void Fire()
    {
        StartCoroutine(nameof(FireCoroutine));
    }
    void StopFire()
    {
        StopCoroutine(nameof(FireCoroutine));
    }
    IEnumerator FireCoroutine()
    {
        while (true)
        {
            Instantiate(projectile, muzzle.position, Quaternion.identity);
            yield return waitForNextFire;
        }
    }
    #endregion
}
