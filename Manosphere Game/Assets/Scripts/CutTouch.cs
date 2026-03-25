using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class CutTouch : MonoBehaviour
{
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] int touchIndex = 0;

    TouchControl touchAction;

    void Start()
    {
        touchAction = Touchscreen.current.touches[touchIndex];
    }

    void Update()
    {
        if (touchAction.isInProgress)
        {
            // Enable cut collider only if not over a "No Cut" element
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touchAction.position.ReadValue());
            touchPosition.z = 0;

            if (Physics2D.OverlapPoint(touchPosition, LayerMask.GetMask("No Cut")) == null)
            {
                transform.position = touchPosition;
                circleCollider.enabled = true;
                trailRenderer.enabled = true;
            }
            else
            {
                circleCollider.enabled = false;
                trailRenderer.Clear();
                trailRenderer.enabled = false;
            }
        }
        else
        {
            circleCollider.enabled = false;
            trailRenderer.Clear();
            trailRenderer.enabled = false;
        }
    }
}
