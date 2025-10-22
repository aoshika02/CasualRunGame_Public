using System.Collections.Generic;
using UnityEngine;

public class StageManager : SingletonMonoBehaviour<StageManager>
{
    private Queue<Stage> _stageQueue = new Queue<Stage>();
    private int _currentStageIndex = 0;
    private float _stageLength = 12f; // 各ステージの長さ
    private StagePool _stagePool;
    private int _initialStageCount = 5; // 初期に生成するステージ数
    private int _startStageIndex = 3; // ステージの開始インデックス
    private bool _isScrolling = false;
    void Start()
    {
        _stagePool = StagePool.Instance;
        for (int i = 3; i < _initialStageCount + 3; i++)
        {
            AddStage(i);
        }
        _currentStageIndex = _startStageIndex - _initialStageCount;
        _isScrolling = true;
    }
    private void Update()
    {
        UpdateStage();
    }
    private void UpdateStage()
    {
        if (_isScrolling == false) return;
        float playerX = PlayerController.Instance.transform.position.x;
        int newStageIndex = Mathf.FloorToInt(playerX / _stageLength);
        if (newStageIndex > _currentStageIndex + 1)
        {
            _currentStageIndex++;
            DisposalStage();
            AddStage(_currentStageIndex + _initialStageCount - 1);
        }
    }
    private void AddStage(int index) 
    {
        var stage = _stagePool.Get();
        stage.transform.position = new Vector3(index * _stageLength, 0, 0);
        stage.SpawnItem();
        _stageQueue.Enqueue(stage);
    }
    private void DisposalStage()
    {
        var stage = _stageQueue.Dequeue();
        _stagePool.Release(stage, stage.Id);
    }
    public void SetScroll(bool isScroll)
    {
        _isScrolling = isScroll;
    }

}
