using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [SerializeField] float StartTime;
    [SerializeField] float TimeToPannel = 5;
    [SerializeField] float TimeToRestart = 10;
    [SerializeField] float PopupMultiplier = 0.5f;
    public bool _isSlowedTime;
    public float timeSinceNoInteraction;

    private TextMeshProUGUI TimerText;
    private bool timerStarted;
    private InputAction touchAction;

    public UnityEvent StartIdleEvent;
    public UnityEvent StopIdleEvent;
    public UnityEvent EndGame;
    private bool _idleInvoked;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TimerText = GetComponent<TextMeshProUGUI>();
        timerStarted = true;
        touchAction = InputSystem.actions.FindAction("Touch");
    }

    // Update is called once per frame
    void Update()
    {
        if(timerStarted)
        {
            StartTime-=Time.deltaTime * (_isSlowedTime? PopupMultiplier: 1);
            var ts = TimeSpan.FromSeconds(StartTime);
            TimerText.text = string.Format("{0:00}:{1:00}", (int)ts.TotalMinutes, (int)ts.Seconds);

            if (StartTime <= 0)
            {
                EndGame.Invoke();
                timerStarted = false;
            }
        }

        timeSinceNoInteraction += Time.deltaTime;

        if(timeSinceNoInteraction > TimeToPannel)
        {
            if(!_idleInvoked)
            {
                StartIdleEvent.Invoke();
                _idleInvoked = true;
            }
            if(timeSinceNoInteraction > TimeToRestart)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }

        if(touchAction.WasPressedThisFrame())
        {
            StopIdleEvent.Invoke();
            timeSinceNoInteraction = 0;
            _idleInvoked= false;
        }
    }

    public void ToggleTimer(bool isOn)
    {
        timerStarted = isOn;
    }

    public void ToggleSlowdown(bool isSlow)
    {
        _isSlowedTime = isSlow;
    }
}
