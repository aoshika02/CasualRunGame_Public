using System;
using TMPro;
using UnityEngine;

public class ResultView : SingletonMonoBehaviour<ResultView>
{
    [SerializeField] private TextMeshProUGUI _coinText;
    [SerializeField] private TextMeshProUGUI _itemText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private CanvasGroup _canvasGroup;
    public void Init() 
    {
        _canvasGroup.alpha = 0;
        SetResultStatus(0, 0);
        InitTimer();
    }
    public void SetResultStatus(int coin, int item, float time)
    {
        SetResultStatus(coin,item);
        time = Math.Clamp(time, 0, 5999);
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        _timerText.text = $"{minutes:00}:{seconds:00}";
    }
    public void SetResultStatus(int coin, int item) 
    {
        _coinText.text = Math.Clamp(coin, 0, 99999).ToString();
        _itemText.text = Math.Clamp(item, 0, 99999).ToString();
    }
    public void InitTimer()
    {
        _timerText.text = "--:--";
    }
    public void Show()
    {
        _canvasGroup.alpha = 1;
    }
}
