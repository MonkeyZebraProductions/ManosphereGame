using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class FadePopup : MonoBehaviour
{
    [SerializeField] float FadeSpeed = 3f;
    [SerializeField] bool isTutorial;

    public UnityEvent TutorialOnFadeOut;

    bool _isfadedIn, _isfadedOut, _startFade, _fadeIn;
    CanvasGroup canvasGroup;
    private InputAction touchAction;
    private Circle circle;
    private CircleTouch circleTouch;
    private SpriteManager spriteManager;
    private Animator animator;

    private SpriteRenderer BlackBackground;
    private AudioManager audioManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isfadedOut = true;
        canvasGroup = GetComponentInChildren<CanvasGroup>();
        touchAction = InputSystem.actions.FindAction("Double Tap");
        circle = GetComponent<Circle>();
        circleTouch = GetComponent<CircleTouch>();
        spriteManager = GetComponentInChildren<SpriteManager>();
        animator = GetComponentInChildren<Animator>();
        audioManager = FindFirstObjectByType<AudioManager>();
        //BlackBackground = GameObject.Find("Background").GetComponent<SpriteRenderer>();
        //Debug.Log(BlackBackground);
    }

    // Update is called once per frame
    void Update()
    {
        if (touchAction != null && touchAction.WasPressedThisFrame())
        {
            if ((circle != null && circle.PositionIsOverCircle()) || (circleTouch != null && circleTouch.PositionIsOverCircle()))
            {
                if (_isfadedOut)
                {
                    _fadeIn = true;
                    if (!_startFade)
                    {
                        _startFade = true;
                        animator.Play("Popup");
                        if(audioManager != null)
                        {
                            if(!audioManager.IsPlaying("TweetReduction"))
                            {
                                audioManager.Play("TweetPopup");
                            }
                        }
                        if (BlackBackground != null)
                        {
                            BlackBackground.enabled = true;
                        }

                    }
                }
                else
                {
                    _fadeIn = false;
                    if (!_startFade)
                    {
                        _startFade = true;
                        animator.Play("Popup Reversed");
                        if (audioManager != null)
                        {
                            if (!audioManager.IsPlaying("TweetPopup"))
                            {
                                audioManager.Play("TweetReduction");
                            }
                        }
                        if (BlackBackground != null)
                        {
                            BlackBackground.enabled = false;
                        }
                    }
                }
            }
            else
            {
                if (_isfadedIn)
                {
                    _fadeIn = false;
                    if (!_startFade)
                    {
                        _startFade = true;
                        animator.Play("Popup Reversed");
                        if (audioManager != null)
                        {
                            if (!audioManager.IsPlaying("TweetPopup"))
                            {
                                audioManager.Play("TweetReduction");
                            }
                        }
                        if (BlackBackground != null)
                        {
                            BlackBackground.enabled = false;
                        }
                    }
                }
            }
            spriteManager.MoveSpriteLayer(_fadeIn);
        }
        
 
    }

    private void FixedUpdate()
    {
        if (_startFade)
        {
            if (_fadeIn)
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
            gameObject.transform.localScale += new Vector3(0.025f, 0.025f, 0f);
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
            gameObject.transform.localScale -= new Vector3(0.025f, 0.025f, 0f);
        }
        else
        {
            _isfadedIn = false;
            _isfadedOut = true;
            _startFade = false;
            if(isTutorial)
            {
                TutorialOnFadeOut.Invoke();
            }
        }
    }
}
