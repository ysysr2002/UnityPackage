using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

//实际的玩家输入管理类
[CreateAssetMenu(menuName ="PlayerInput")]
public class PlayerInput : ScriptableObject, PlayerInputActions.IGamePlayActions
{
    public event UnityAction<Vector2> onMove = delegate { };//UnityAction对应C#中的Action，是一个委托类型
    public event UnityAction onStopMove = delegate { };//可以用空的委托来初始化
    public event UnityAction onFire = delegate { };
    public event UnityAction onStopFire = delegate { };
    PlayerInputActions playerInputActions;

    private void OnEnable()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.GamePlay.SetCallbacks(this);//登记GamePlay动作表的回调函数
    }
    private void OnDisable()
    {
        playerInputActions.GamePlay.Disable();
    }
    //启用GamePlay动作表时进行的设置
    public void EnableGamePlayInput()
    {
        playerInputActions.GamePlay.Enable();//启用动作表
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    //在某些时候，如过场动画时，需要禁用所有的输入
    public void DisableAllInputs()
    {
        playerInputActions.GamePlay.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        #region InputActionPhase包含五种状态：
        //1.Disabled 当前动作表被禁用
        //2.Waiting 当前动作表被启用，但没有任何信号输入
        //3.Started 相当于Inupt.GetKeyDown
        //4.Performed 相当于Inupt.GetKey
        //5.Canceled 相当于Inupt.GetKeyUp
        #endregion
        //如果按下左键，再按右键，状态将为Canceled
        if (context.phase == InputActionPhase.Performed)
        {
            onMove.Invoke(context.ReadValue<Vector2>());
        } 
        if (context.phase == InputActionPhase.Canceled)
        {
            onStopMove.Invoke();
        }

    }
    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onFire.Invoke();
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            onStopFire.Invoke();
        }
    }
}
