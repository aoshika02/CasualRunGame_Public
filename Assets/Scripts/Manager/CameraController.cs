using UnityEngine;

public class CameraController : SingletonMonoBehaviour<CameraController>
{
    private PlayerController _playerController;
    private bool _isFollowing = true;
    void Start()
    {
        _playerController = PlayerController.Instance;
    }
    private void FixedUpdate()
    {
        if (_isFollowing == false) return;
        Vector3 newPosition = transform.position;
        newPosition.x = _playerController.transform.position.x + 2;
        transform.position = newPosition;
    }
    public void SetFollow(bool isFollow)
    {
        _isFollowing = isFollow;
    }

}
