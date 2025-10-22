using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class CoinObj : MonoBehaviour,IPool
{
    public bool IsGenericUse { get; set; }
    private CoinCountManager _coinCountManager;
    private FieldObjPool _fieldObjPool;
    private int _coinValue = 1;
    private void Start()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        _coinCountManager = CoinCountManager.Instance;
        _fieldObjPool = FieldObjPool.Instance;
    }
    public void Init(int coinValue = 1) 
    {
        _coinValue = coinValue;
    }
    public void OnRelease()
    {
        gameObject.SetActive(false);
    }

    public void OnReuse()
    {
        gameObject.SetActive(true);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag(ParamConsts.PLAYER_TAG))
        {
            _coinCountManager.AddCoin(_coinValue);
            gameObject.SetActive(false);
        }
    }

}
