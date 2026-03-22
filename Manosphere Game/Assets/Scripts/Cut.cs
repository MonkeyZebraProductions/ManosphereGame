using UnityEngine;
using UnityEngine.InputSystem;

public class Cut : MonoBehaviour
{
    [SerializeField] CircleCollider2D circleCollider;
    [SerializeField] TrailRenderer trailRenderer;

    InputAction touchAction;

    void Start()
    {
        touchAction = InputSystem.actions.FindAction("Touch");
    }

    void Update()
    {
        if (touchAction.IsPressed())
        {
            // Enable cut collider only if not over a "No Cut" element
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePosition.z = 0;

            if (Physics2D.OverlapPoint(mousePosition, LayerMask.GetMask("No Cut")) == null)
            {
                transform.position = mousePosition;
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
