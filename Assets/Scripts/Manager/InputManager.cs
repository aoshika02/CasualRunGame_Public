using System.Collections.Generic;
using UniRx;
using UnityEngine.EventSystems;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class InputManager : SingletonMonoBehaviour<InputManager>
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private List<Canvas> _hitCanvases;
    public IObservable<GameObject> OnTapped => _onTapped;
    private readonly Subject<GameObject> _onTapped = new Subject<GameObject>();
    public ReactiveProperty<float> MouseInput => _mouseInput;
    private ReactiveProperty<float> _mouseInput = new ReactiveProperty<float>();
    private PlayerAction _playerAction;
    private List<GraphicRaycaster> _raycasters = new List<GraphicRaycaster>();
    private PointerEventData _pointerEventData;
    private EventSystem _eventSystem;

    protected override void Awake()
    {
        if (CheckInstance() == false) return;

        _playerAction = new PlayerAction();
        _playerAction.Player.Enable();

        if (_playerAction == null) return;

        _playerAction.Player.Jump.started += OnJumpStarted;
        _playerAction.Player.Jump.performed += OnJumpPerformed;
        _playerAction.Player.Jump.canceled += OnJumpCanceled;

        foreach (var canvas in _hitCanvases)
        {
            _raycasters.Add(canvas.GetComponent<GraphicRaycaster>());
        }
        _eventSystem = EventSystem.current;
    }
    private void OnTapStarted(InputAction.CallbackContext context)
    {
        TapEvent(_onTapped, context);
    }
    private void TapEvent(Subject<GameObject> subject, InputAction.CallbackContext context)
    {
        //if (_mainCamera == null)
        //    return;
        var tmpScreenPos = GetDeviceValue();

        if (tmpScreenPos == null) return;
        Vector3 screenPos = tmpScreenPos.Value;
        screenPos.z = _mainCamera.nearClipPlane;

        List<GameObject> uiHits = RaycastUI(screenPos);
        if (uiHits != null && uiHits.Count > 0)
        {
            subject.OnNext(uiHits[0]);
        }
    }
    private Vector3? GetDeviceValue()
    {
        //マウスが接続ならマウス座標を返す
        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }
        //指の座標を返す
        if (Touchscreen.current != null)
        {
            if (Touchscreen.current.touches.Count == 1)
            {
                return Touchscreen.current.primaryTouch.position.ReadValue();
            }
        }
        //例外
        return null;
    }
    private List<GameObject> RaycastUI(Vector2 screenPosition)
    {
        _pointerEventData = new PointerEventData(_eventSystem)
        {
            position = screenPosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        List<List<RaycastResult>> resultDatas = new List<List<RaycastResult>>();
        foreach (var cast in _raycasters)
        {
            cast.Raycast(_pointerEventData, results);
            resultDatas.Add(new List<RaycastResult>(results));
            results.Clear();
        }

        return resultDatas.SelectMany(list => list).Select(r => r.gameObject).ToList();
    }
    private void OnJumpStarted(InputAction.CallbackContext ctx)
    {
        _mouseInput.Value = ctx.ReadValue<float>();
        OnTapStarted(ctx);
    }

    private void OnJumpPerformed(InputAction.CallbackContext ctx)
    {
        _mouseInput.Value = ctx.ReadValue<float>();
    }

    private void OnJumpCanceled(InputAction.CallbackContext ctx)
    {
        _mouseInput.Value = ctx.ReadValue<float>();
    }
    private void OnDestroy()
    {
        if (_playerAction == null) return;

        _playerAction.Player.Jump.started -= OnJumpStarted;
        _playerAction.Player.Jump.performed -= OnJumpPerformed;
        _playerAction.Player.Jump.canceled -= OnJumpCanceled;

        _playerAction.Disable();

    }
}
