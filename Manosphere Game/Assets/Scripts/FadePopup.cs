using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FadePopup : MonoBehaviour
{

    [SerializeField] float FadeSpeed = 1f;

    bool _isfadedIn,_isfadedOut,_startFade,_fadeIn;
    CanvasGroup canvasGroup;
    private InputAction touchAction;
    private Circle circle;
    private CircleTouch circleTouch;
    private SpriteManager spriteManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isfadedOut = true;
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        touchAction = InputSystem.actions.FindAction("Touch");
        circle = GetComponent<Circle>();
        circleTouch = GetComponent<CircleTouch>();
        spriteManager = GetComponentInChildren<SpriteManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(touchAction != null && touchAction.WasPressedThisFrame())
        {
            if ((circle!=null && circle.PositionIsOverCircle()) || (circleTouch != null && circleTouch.PositionIsOverCircle()))
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
            spriteManager.MoveSpriteLayer(_fadeIn);
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

   
}
