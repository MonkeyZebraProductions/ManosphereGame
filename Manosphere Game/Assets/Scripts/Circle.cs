using UnityEngine;
using UnityEngine.InputSystem;

public class Circle : MonoBehaviour
{
    [SerializeField] GameObject linePrefab;
    
    GameObject linesParent;
    GameObject currentLine;
    LineRenderer currentLineRenderer;
    bool isDragging;
    InputAction touchAction;

    void Start()
    {
        isDragging = false;
        linesParent = GameObject.Find("Connections");
        touchAction = InputSystem.actions.FindAction("Touch");
    }

    void Update()
    {
        if (touchAction.IsPressed())
        {
            if (!isDragging)
            {
                if (PositionIsOverCircle() && touchAction.WasPressedThisFrame())
                {
                    currentLine = Instantiate(linePrefab, linesParent.transform);
                    currentLineRenderer = currentLine.GetComponent<LineRenderer>();
                    currentLineRenderer.SetPosition(0, transform.position);
                    
                    Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                    mousePosition.z = 0;
                    currentLineRenderer.SetPosition(1, mousePosition);
                    
                    isDragging = true;
                }
            }
            else
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                mousePosition.z = 0;
                currentLineRenderer.SetPosition(1, mousePosition);
            }
        }
        else if (isDragging)
        {
            isDragging = false;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePosition.z = 0;
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

            if (hitCollider != null && hitCollider.gameObject != gameObject && hitCollider.CompareTag("Circle"))
            {
                currentLineRenderer.SetPosition(1, hitCollider.transform.position);
            }
            else
            {
                Destroy(currentLine);
            }
        }
    }

    bool PositionIsOverCircle()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePosition.z = 0;
        float distance = Vector3.Distance(mousePosition, transform.position);
        return distance <= GetComponent<CircleCollider2D>().radius;
    }
}
