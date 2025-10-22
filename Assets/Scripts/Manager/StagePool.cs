using System.Collections.Generic;
using UnityEngine;

public class StagePool : SingletonMonoBehaviour<StagePool>
{
    [SerializeField] private GameObject[] _stagePrefabs;
    private readonly Dictionary<int, GenericObjectPool<Stage>> _stageDic = new Dictionary<int, GenericObjectPool<Stage>>();
    protected override void Awake()
    {
        if (CheckInstance() == false) return;
        for (int i = 0; i < _stagePrefabs.Length; i++)
        {
            if (_stageDic.TryAdd(i, new GenericObjectPool<Stage>(_stagePrefabs[i], transform)) == false)
            {
                Debug.LogError($"StagePoolの辞書にKey:{i}の追加に失敗");
                continue;
            }
        }
    }
    public Stage Get()
    {
        if (_stageDic.Count == 0)
        {
            Debug.LogError("StagePool に登録されているステージがありません。");
            return null;
        }

        int id = Random.Range(0, _stageDic.Count);
        if (_stageDic.TryGetValue(id, out var stagePool) == false)
        {
            Debug.LogError($"StagePoolの辞書からKey:{id}のステージを取得できませんでした。");
            return null;
        }
        Stage stage = stagePool.Get();
        stage.SetId(id);
        return stage;
    }
    public void Release(Stage stage,int id)
    {
        if (_stageDic.TryGetValue(id, out var _stagePool) == false)
        {
            Debug.LogError($"StagePoolの辞書にKey:{id}が存在しません。");
            return;
        }
        _stagePool.Release(stage);
    }
}
