using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimController : SingletonMonoBehaviour<PlayerAnimController>
{
    private Animator _animator;
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void JumpStart()
    {
        _animator.SetTrigger(ParamConsts.JUMP_START);
    }
    public void JumpEnd()
    {
        _animator.SetTrigger(ParamConsts.JUMP_END);
    }
}
