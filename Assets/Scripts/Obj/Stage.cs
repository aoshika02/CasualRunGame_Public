using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour,IPool
{
    [SerializeField] private Transform[] _coinSpawnPoints;
    [SerializeField] private Transform[] _speedUpItemSpawnPoints;

    private List<CoinObj> _spawnedCoins = new List<CoinObj>();
    private List<SpeedUpItem> _spawnedSpeedUpItems = new List<SpeedUpItem>();
    public bool IsGenericUse { get; set; }
    private bool _isSpawned = false;
    private int _id;
    public int Id => _id;

    FieldObjPool _fieldObjPool;
    private void Awake()
    {
        _fieldObjPool = FieldObjPool.Instance;
    }
    public void SetId(int id)
    {
        _id = id;
    }
    public void SpawnItem()
    {
        if (_isSpawned) return;
        if (_coinSpawnPoints != null && _coinSpawnPoints.Length > 0)
        {
            foreach (var point in _coinSpawnPoints)
            {
                if (point == null) continue;
                var coin = _fieldObjPool.GetObj<CoinObj>(ItemType.Coin);
                coin.transform.position = point.position;
                _spawnedCoins.Add(coin);
            }
        }
        if (_speedUpItemSpawnPoints != null && _speedUpItemSpawnPoints.Length > 0)
        {
            foreach (var point in _speedUpItemSpawnPoints)
            {
                if (point == null) continue;
                var speedUpItem = _fieldObjPool.GetObj<SpeedUpItem>(ItemType.SpeedUpItem);
                speedUpItem.transform.position = point.position;
                _spawnedSpeedUpItems.Add(speedUpItem);
            }
        }
        _isSpawned = true;
    }
    public void ReleaseItems() 
    {
        foreach (var coin in _spawnedCoins)
        {
            _fieldObjPool.ReleaseObj(coin, ItemType.Coin);
        }
        _spawnedCoins.Clear();
        foreach (var speedUpItem in _spawnedSpeedUpItems)
        {
            _fieldObjPool.ReleaseObj(speedUpItem, ItemType.SpeedUpItem);
        }
        _spawnedSpeedUpItems.Clear();
        _isSpawned = false;
    }
    public void OnRelease()
    {
        gameObject.SetActive(false);
        _isSpawned = false;
        ReleaseItems();
    }
    public void OnReuse()
    {
        gameObject.SetActive(true);
    }
}
