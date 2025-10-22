using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : SingletonMonoBehaviour<PlayerController>
{
    [SerializeField] private float _defaultMoveSpeed = 5f;
    [SerializeField] private float _maxMoveSpeed = 10f;
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private float _maxSpeedStack = 20f;
    [SerializeField] private float _minSpeedStack = 0f;
    [SerializeField] private float _speedUpDuration = 10f;
    [SerializeField] private float _gravity = 7.5f;

    public float MaxSpeedStack => _maxSpeedStack;
    public IReadOnlyReactiveProperty<float> SpeedStack => _speedStack;
    private ReactiveProperty<float> _speedStack = new ReactiveProperty<float>();
    private float _currentMoveSpeed;
    private Rigidbody2D _rigidbody2D;
    private bool _isSubStacking = false;
    private bool _isJumping = false;
    private bool _ignoreGroundCheck = false;
    private bool _isMove = false;
    private int _itemCount = 0;
    public int ItemCount => _itemCount;
    private PlayerAnimController _playerAnimController;
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerAnimController = PlayerAnimController.Instance;
        _speedStack.Value = _minSpeedStack;
        _currentMoveSpeed = _defaultMoveSpeed;
        _isMove = false;
        _speedStack.Subscribe(async x =>
        {
            if (x >= _maxSpeedStack)
            {
                await DOTween.To
                (
                    () => _currentMoveSpeed,
                    x => _currentMoveSpeed = x,
                    _maxMoveSpeed,
                    0.5f
                ).SetLink(gameObject).ToUniTask();
                SubStackAsync().Forget();
            }
        }).AddTo(this);
        InputManager.Instance.MouseInput
            .Subscribe(x =>
            {
                if (x != 1)
                {
                    Debug.Log("Not Jump");
                    return;
                }
                Jump();
            }).AddTo(this);
    }
    private void Update()
    {
        if (_isMove == false) return;

        _rigidbody2D.velocity = new Vector2(_currentMoveSpeed, _rigidbody2D.velocity.y);
    }
    private void FixedUpdate()
    {
        if (_isMove == false) return;
        bool grounded = !_ignoreGroundCheck && IsGrounded();

        if (_isJumping && grounded)
        {
            _playerAnimController.JumpEnd();
            _isJumping = false;
            _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, 0);
        }

        if (_isJumping && !grounded)
        {
            _rigidbody2D.AddForce(Vector2.down * _gravity, ForceMode2D.Force);
        }
    }
    public async void Jump()
    {
        if (_isMove == false)
        {
            return;
        }
        if (_isJumping) return;
        if (Mathf.Abs(_rigidbody2D.velocity.y) < 0.01f)
        {
            _ignoreGroundCheck = true;
            _playerAnimController.JumpStart();
            _rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _isJumping = true;
        }

        await UniTask.WaitForSeconds(0.02f);
        _ignoreGroundCheck = false;
    }

    #region SpeedStack
    public void AddSpeedStack(float stack)
    {
        if (_isSubStacking) return;
        _itemCount++;
        _speedStack.Value = Mathf.Clamp(_speedStack.Value + stack, _minSpeedStack, _maxSpeedStack);
    }

    public async UniTask SubStackAsync()
    {
        if (_isSubStacking) return;
        _isSubStacking = true;
        await DOTween.To
        (
            () => _speedStack.Value,
            x => _speedStack.Value = x,
            _minSpeedStack,
            _speedUpDuration
        ).SetLink(gameObject).ToUniTask();
        await DOTween.To
        (
            () => _currentMoveSpeed,
            x => _currentMoveSpeed = x,
            _defaultMoveSpeed,
             0.5f
        ).SetLink(gameObject).ToUniTask();
        _isSubStacking = false;
    }
    #endregion

    public void SetMove(bool isMove)
    {
        _isMove = isMove;
        if (isMove == false)
        {
            _rigidbody2D.velocity = Vector2.zero;
        }
    }
    private bool IsGrounded()
    {
        var col = GetComponent<Collider2D>();
        var bounds = col.bounds;
        float extraHeight = 0.1f;
        bool grounded = Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.down, extraHeight, LayerMask.GetMask(ParamConsts.GROUND));

        Debug.DrawRay(bounds.center + new Vector3(-bounds.extents.x, -bounds.extents.y, 0), Vector2.down * extraHeight, Color.green);
        Debug.DrawRay(bounds.center + new Vector3(bounds.extents.x, -bounds.extents.y, 0), Vector2.down * extraHeight, Color.green);

        return grounded;
    }
    private void OnDestroy()
    {
        _isMove = false;
        if (_rigidbody2D != null)
            _rigidbody2D.velocity = Vector2.zero;

        DOTween.Kill(gameObject);
    }
}
