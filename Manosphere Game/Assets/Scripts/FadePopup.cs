using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FadePopup : MonoBehaviour
{

    [SerializeField] float FadeSpeed = 1f;

    bool _isfadedIn,_isfadedOut,_startFade,_fadeIn;
    CanvasGroup canvasGroup;
    private InputAction touchAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isfadedOut = true;
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        touchAction = InputSystem.actions.FindAction("Touch");
    }

    // Update is called once per frame
    void Update()
    {
        if(touchAction != null && touchAction.WasPressedThisFrame())
        {
            if (PositionIsOverCircle())
            {
                if (_isfadedOut)
                {
                    _fadeIn = true;
                    if (!_startFade)
                    {
                        _startFade = true;
                    }
                }
                else
                {
                    _fadeIn = false;
                    if (!_startFade)
                    {
                        _startFade = true;
                    }
                }
            }
            else
            {
                if (_isfadedIn)
                {
                    _fadeIn = false;
                    if(!_startFade)
                    {
                        _startFade = true;
                    }
                }
            }
        }
        if(_startFade)
        {
            if(_fadeIn)
            {
                FadeInCircle();
            }
            else
            {
                FadeOutCircle();
            }
        }
    }

    void FadeInCircle()
    {
        if (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.deltaTime * FadeSpeed;
        }
        else
        {
            _isfadedIn = true;
            _isfadedOut = false;
            _startFade = false;
        }
    }

    void FadeOutCircle()
    {
        if (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime * FadeSpeed;
        }
        else
        {
            _isfadedIn = false;
            _isfadedOut = true;
            _startFade = false;
        }
    }

    bool PositionIsOverCircle()
    {
        //change content once Henrique has figured out touching
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;
        float distance = Vector3.Distance(mousePosition, transform.position);
        return distance <= GetComponent<CircleCollider2D>().radius;
    }
}
