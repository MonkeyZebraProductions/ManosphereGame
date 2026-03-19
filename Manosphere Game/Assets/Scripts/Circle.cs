using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Circle : MonoBehaviour
{
    [SerializeField] GameObject linePrefab;
    [SerializeField] float maxConnectionDistance = 3f;
    
    GameObject linesParent;
    GameObject currentLine;
    LineRenderer currentLineRenderer;
    bool isDragging;
    InputAction touchAction;
    List<GameObject> connectedCircles = new List<GameObject>();

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
                // Start line drawing if the touch is over the circle and was just pressed
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
                // Continue drawing the line to follow the touch position
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
                mousePosition.z = 0;

                if (Vector3.Distance(transform.position, mousePosition) > maxConnectionDistance)
                {
                    Vector3 direction = (mousePosition - transform.position).normalized;
                    mousePosition = transform.position + direction * maxConnectionDistance;
                }

                currentLineRenderer.SetPosition(1, mousePosition);
            }
        }
        else if (isDragging)
        {
            isDragging = false;

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePosition.z = 0;

            if (Vector3.Distance(transform.position, mousePosition) > maxConnectionDistance)
            {
                Vector3 direction = (mousePosition - transform.position).normalized;
                mousePosition = transform.position + direction * maxConnectionDistance;
            }

            Collider2D hitCollider = Physics2D.OverlapPoint(mousePosition);

            // Check if the line ends over another circle that is not the same as the starting circle and is not already connected
            if (hitCollider != null && hitCollider.gameObject != gameObject && hitCollider.CompareTag("Circle") && !connectedCircles.Contains(hitCollider.gameObject))
            {
                currentLineRenderer.SetPosition(1, hitCollider.transform.position);
                
                // Add the connected circle to the list and also add this circle to the other circle's list
                connectedCircles.Add(hitCollider.gameObject);
                Circle otherCircleScript = hitCollider.GetComponent<Circle>();
                if (otherCircleScript != null)
                {
                    otherCircleScript.AddConnectedCircle(gameObject);
                }
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

    public void AddConnectedCircle(GameObject circle)
    {
        if (!connectedCircles.Contains(circle))
        {
            connectedCircles.Add(circle);
        }
    }
}
