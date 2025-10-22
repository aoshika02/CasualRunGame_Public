using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class OnTapEvent : MonoBehaviour
{
    void Start()
    {
        InputManager.Instance.OnTapped.Subscribe(x => 
        {
            Debug.Log(x);
            if (x != gameObject) return;
            GameManager.Instance.OnTap();
        }).AddTo(this);    
    }
}
