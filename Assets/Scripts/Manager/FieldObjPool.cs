using UnityEngine;

public class FieldObjPool : SingletonMonoBehaviour<FieldObjPool>
{
    private GenericObjectPool<CoinObj> _coinPool;
    private GenericObjectPool<SpeedUpItem> _speedUpItemPool;

    [SerializeField] private GameObject _coinObj;
    [SerializeField] private GameObject _speedUpItem;

    [SerializeField] private Transform _coinPoolParent;
    [SerializeField] private Transform _speedUpItemPoolParent;
    protected override void Awake()
    {
        if (CheckInstance() == false)
        {
            return;
        }
        _coinPool = new GenericObjectPool<CoinObj>(_coinObj, _coinPoolParent);
        _speedUpItemPool = new GenericObjectPool<SpeedUpItem>(_speedUpItem, _speedUpItemPoolParent);
    }
    public T GetObj<T>(ItemType itemType) where T : MonoBehaviour, IPool
    {
        switch (itemType)
        {
            case ItemType.Coin:
                return _coinPool.Get() as T;
            case ItemType.SpeedUpItem:
                return _speedUpItemPool.Get() as T;
            default:
                Debug.LogError("存在しないアイテムタイプです");
                return null;
        }
    }
    public void ReleaseObj<T>(T obj, ItemType itemType) where T : MonoBehaviour, IPool
    {
        switch (itemType)
        {
            case ItemType.Coin:
                _coinPool.Release(obj as CoinObj);
                break;
            case ItemType.SpeedUpItem:
                _speedUpItemPool.Release(obj as SpeedUpItem);
                break;
            default:
                Debug.LogError("存在しないアイテムタイプです");
                break;
        }
    }
}

public enum ItemType
{
    Coin = 0,
    SpeedUpItem = 1,
}