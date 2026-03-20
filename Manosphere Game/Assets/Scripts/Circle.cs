using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Circle : MonoBehaviour
{
    [SerializeField] GameObject linePrefab;
    [SerializeField] float maxConnectionDistance = 3f;
    [SerializeField] bool isEnemy;
    [SerializeField] float infectionTime = 3f;

    GameObject linesParent;
    GameObject currentLine;
    LineRenderer currentLineRenderer;
    bool isDragging;
    InputAction touchAction;
    List<GameObject> connectedCircles = new List<GameObject>();

    //Prevents players from connecting lines to enemies once they have been discovered
    bool enemyDiscovered;
    bool isInfected;
    float alpha = 0f;

    void Start()
    {
        isDragging = false;
        linesParent = GameObject.Find("Connections");
        touchAction = InputSystem.actions.FindAction("Touch");
    }

    void Update()
    {
        if (touchAction.IsPressed() && !enemyDiscovered)
        {
            //Prevents players from connecting lines to enemies once they have been discovered
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

                // When Enemy connects to circle,
                if(isEnemy)
                {
                    enemyDiscovered = true;
                    if (otherCircleScript != null)
                    {
                        otherCircleScript.isInfected = true;
                    }
                }
            }
            else
            {
                Destroy(currentLine);
            }
        }

        //Once Infected or discovered, circles start automatically connectint to each other
        if(enemyDiscovered || (isInfected && IsConnectedToEnemy()))
        {
            if(alpha == 0f)
            {
                Collider2D[] collidedCircles = Physics2D.OverlapCircleAll(transform.position,maxConnectionDistance);
                List<GameObject> potentialConnections = new List<GameObject>();

                //Logic for choosing which circle to potentially infect. Flesh this out once rules are set
                foreach (Collider2D col in collidedCircles)
                {
                    if(!connectedCircles.Contains(col.gameObject))
                    {
                        potentialConnections.Add(col.gameObject);
                    }
                }
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

    bool IsConnectedToEnemy()
    {
        foreach (GameObject circle in connectedCircles)
        {
            Circle circleScript = circle.GetComponent<Circle>();
            if(circleScript != null && circleScript.isEnemy)
            {
                return true;
            }
        }
        return false;
    }
}
