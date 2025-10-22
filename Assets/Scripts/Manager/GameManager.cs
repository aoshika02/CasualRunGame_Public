using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private TextMeshProUGUI _countDownText;
    [SerializeField] private RectTransform _taegetRoot;
    [SerializeField] private TextMeshProUGUI _targetText;
    [SerializeField] private float _targetMeter = 1000f;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private CanvasGroup _titleCanvasGroup;
    [SerializeField] private CanvasGroup _titleTextGroup;
    private bool _isTap = false;
    ResultView _result;
    PlayerController _playerController;
    CameraController _cameraController;
    private Vector3 _playerSatrtPos = new Vector2(-2.75f, -1.47f);
    private Vector3 _cameraStartPos = new Vector3(-0.75f, 0, -10);
    protected override void Awake()
    {
        if (!CheckInstance()) return;
        _result = ResultView.Instance;
        _playerController = PlayerController.Instance;
        _cameraController = CameraController.Instance;
        _countDownText.text = "";
        _canvasGroup.alpha = 1;
        _titleCanvasGroup.alpha = 1;
        TitileFlow().Forget();
    }
    private async UniTask TitileFlow()
    {
        _playerController.transform.position = _playerSatrtPos;
        _cameraController.transform.position = _cameraStartPos;
        _result.Init();
        var tween = DOTween.Sequence()
            .Append(DOVirtual.Float(0f, 1f, 2f, v => _titleTextGroup.alpha = v))
            .Append(DOVirtual.Float(1f, 0f, 2f, v => _titleTextGroup.alpha = v))
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Restart);
        _isTap = false;
        await UniTask.WaitUntil(() => _isTap);
        tween.Kill();
        await _titleCanvasGroup.DOFade(0, 0.5f);
        GameFlow().Forget();
    }
    private async UniTask GameFlow()
    {
        await ShowTargetAsync();
        await CountDownAsync(3);
        _canvasGroup.alpha = 0;
        TimerManager.Instance.Timer().Forget();
        PlayerController.Instance.SetMove(true);
        await UniTask.WaitUntil(() => PlayerController.Instance.transform.position.x >= _targetMeter);
        TimerManager.Instance.StopTimer();
        CameraController.Instance.SetFollow(false);
        StageManager.Instance.SetScroll(false);
        await UniTask.WaitUntil(() => PlayerController.Instance.transform.position.x > _targetMeter + 12f);
        PlayerController.Instance.SetMove(false);
        ResultView.Instance.SetResultStatus(CoinCountManager.Instance.GetCoin(), PlayerController.Instance.ItemCount, TimerManager.Instance.Time.Value);
        ResultView.Instance.Show();
        _isTap = false;
        await UniTask.WaitUntil(() => _isTap);
        await SceneManager.LoadSceneAsync(0);
    }
    private async UniTask ShowTargetAsync()
    {
        _targetText.text = $"{_targetMeter} m";
        await _taegetRoot.DOLocalMoveY(0, 0.5f).SetEase(Ease.OutCirc).ToUniTask();
        await UniTask.WaitForSeconds(1.5f);
        await _taegetRoot.DOLocalMoveY(-1300f, 0.5f).SetEase(Ease.InCirc).ToUniTask();
    }
    private async UniTask CountDownAsync(int maxCount)
    {
        for (int i = maxCount; i > 0; i--)
        {
            _countDownText.text = i.ToString();
            await UniTask.WaitForSeconds(1);
        }
        _countDownText.text = "GO!";
        await UniTask.WaitForSeconds(1);
        _countDownText.text = "";
    }
    public void OnTap()
    {
        _isTap = true;
    }
}
