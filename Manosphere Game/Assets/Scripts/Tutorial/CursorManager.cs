using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    private Image image;
    [SerializeField] float TimeToPannel = 5;
    public float timeSinceNoInteraction;
    private InputAction touchAction;
    private Animator animator;
    private int TimeIncrease = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        image = GetComponent<Image>();
        animator = GetComponent<Animator>();
        image.enabled = false;
        touchAction = InputSystem.actions.FindAction("Touch");
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceNoInteraction += Time.deltaTime * TimeIncrease;

        image.enabled = (timeSinceNoInteraction > TimeToPannel);

        if (touchAction.WasPressedThisFrame())
        {
            timeSinceNoInteraction = 0;
        }
    }

    public void IncrementCursor(int Step)
    {
        if(Step == 4)
        {
            TimeIncrease = 0;
        }
        else
        {
            animator.SetInteger("Step", Step);

        }
    }

    public void LoadNextScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
