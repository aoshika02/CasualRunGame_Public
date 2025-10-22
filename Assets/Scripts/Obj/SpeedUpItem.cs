using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SpeedUpItem : MonoBehaviour,IPool
{
    public bool IsGenericUse { get; set; }

    private PlayerController _playerController;
    private float _speedStackValue = 5f;

    private void Start()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        _playerController = PlayerController.Instance;
    }
    public void Init(float speedStackValue = 5f)
    {
        _speedStackValue = speedStackValue;
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
        if (collision.gameObject.CompareTag(ParamConsts.PLAYER_TAG))
        {
            _playerController.AddSpeedStack(_speedStackValue);
            gameObject.SetActive(false);
        }
    }
}
