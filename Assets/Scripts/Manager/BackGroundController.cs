using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;

public class BackGroundController : SingletonMonoBehaviour<BackGroundController>
{
    [SerializeField] private float[] _parallaxFactors;
    [SerializeField] private GameObject[] _bgLayers;
    [SerializeField] private float[] _layerWidths;
    private float[] _initLayerPositionsX;

    private async void Start()
    {
        _initLayerPositionsX = new float[_bgLayers.Length];
        for (int i = 0; i < _bgLayers.Length; i++)
        {
            _initLayerPositionsX[i] = _bgLayers[i].transform.position.x;
        }
        await UniTask.WaitUntil(() => CameraController.Instance != null);
        CameraController.Instance.transform.ObserveEveryValueChanged(x=>x.position.x)
            .Subscribe(x =>
            {
                    UpdateParallaxLayers(x);
            }).AddTo(this);
    }
    private void UpdateParallaxLayers(float cameraPosX)
    {
        for (int i = 0; i < _bgLayers.Length; i++)
        {
            float distance = cameraPosX * _parallaxFactors[i];
            _bgLayers[i].transform.position = new Vector2(_initLayerPositionsX[i] + distance, _bgLayers[i].transform.position.y);
            float temp = (cameraPosX * (1 - _parallaxFactors[i]));
        }
    }
}
