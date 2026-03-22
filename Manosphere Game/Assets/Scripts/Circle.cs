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
    bool isInfected;
    float timeToInfect;

    // Prevents players from connecting lines to enemies once they have been discovered
    bool enemyDiscovered;

    void Start()
    {
        isDragging = false;
        linesParent = GameObject.Find("Connections");
        touchAction = InputSystem.actions.FindAction("Touch");
        timeToInfect = 0f;
    }

    void Update()
    {
        // Prevents players from connecting lines from enemies once they have been discovered
        if (touchAction.IsPressed() && !enemyDiscovered)
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

            // Check if the line ends over another circle that is not the same as the starting circle, is not already connected and is not an enemy that has already been discovered
            if (hitCollider != null && hitCollider.gameObject != gameObject && hitCollider.CompareTag("Circle") && !connectedCircles.Contains(hitCollider.gameObject) && hitCollider.GetComponent<Circle>() != null && !hitCollider.GetComponent<Circle>().IsDiscovered())
            {
                currentLineRenderer.SetPosition(1, hitCollider.transform.position);
                
                // Add the connected circle to the list and also add this circle to the other circle's list
                connectedCircles.Add(hitCollider.gameObject);
                Circle otherCircleScript = hitCollider.GetComponent<Circle>();
                if (otherCircleScript != null)
                {
                    otherCircleScript.AddConnectedCircle(gameObject);
                }

                // If the circle is an enemy, mark it as discovered and infect the other circle
                if (isEnemy)
                {
                    enemyDiscovered = true;
                    GetComponent<SpriteRenderer>().color = Color.red;
                    if (otherCircleScript != null)
                    {
                        if (otherCircleScript.NumberOfConnections() < 3 && !otherCircleScript.IsEnemy())
                        {
                            otherCircleScript.Infect(true);
                        }
                    }
                }
            }
            else
            {
                Destroy(currentLine);
            }
        }

        // Once infected or discovered, circles start automatically connecting to each other
        if (enemyDiscovered || (isInfected && IsConnectedToEnemy()))
        {
            timeToInfect += Time.deltaTime;

            if (timeToInfect >= infectionTime)
            {
                timeToInfect = 0f;

                Collider2D[] collidedCircles = Physics2D.OverlapCircleAll(transform.position, maxConnectionDistance);
                List<GameObject> potentialConnections = new List<GameObject>();

                foreach (Collider2D col in collidedCircles)
                {
                    Circle colCircleScript = col.GetComponent<Circle>();

                    // Check if it is a circle that is not already connected, is not an enemy and has less than 2 connections or is infected
                    if (!connectedCircles.Contains(col.gameObject) && col.gameObject != gameObject && col.CompareTag("Circle") && colCircleScript != null && !colCircleScript.IsEnemy() && (colCircleScript.NumberOfConnections() < 2 || colCircleScript.IsInfected()))
                    {
                        potentialConnections.Add(col.gameObject);
                    }
                }

                if (potentialConnections.Count > 0)
                {
                    // Connect to a random potential circle
                    GameObject circleToConnect = potentialConnections[Random.Range(0, potentialConnections.Count)];
                    currentLine = Instantiate(linePrefab, linesParent.transform);
                    currentLineRenderer = currentLine.GetComponent<LineRenderer>();
                    currentLineRenderer.SetPosition(0, transform.position);
                    currentLineRenderer.SetPosition(1, circleToConnect.transform.position);
                    connectedCircles.Add(circleToConnect);
                    Circle otherCircleScript = circleToConnect.GetComponent<Circle>();
                    if (otherCircleScript != null)
                    {
                        otherCircleScript.AddConnectedCircle(gameObject);
                        otherCircleScript.Infect(true);
                    }
                }
            }
        }
        else
        {
            timeToInfect = 0f;
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
            
            // If this circle is an enemy, mark the connected circle as infected if it has less than 3 connections and is not an enemy
            Circle circleScript = circle.GetComponent<Circle>();
            if (circleScript != null && isEnemy)
            {
                enemyDiscovered = true;
                GetComponent<SpriteRenderer>().color = Color.red;
                if (circleScript.NumberOfConnections() < 3 && !circleScript.IsEnemy())
                {
                    circleScript.Infect(true);
                }
            }
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

    public void Infect(bool infected)
    {
        bool wasAlreadyInfected = isInfected;
        isInfected = infected;

        if (!isEnemy && infected)
        {
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.5f, 0.5f); // Light red for infected circles that are not enemies
        }        

        // If the circle becomes infected and was not already infected, it should also infect all connected circles that are not enemies and have less than 3 connections
        if (infected && !wasAlreadyInfected)
        {
            foreach (GameObject circle in connectedCircles)
            {
                Circle circleScript = circle.GetComponent<Circle>();
                if (circleScript != null && !circleScript.IsEnemy() && circleScript.NumberOfConnections() < 3)
                {
                    circleScript.Infect(true);
                }
            }
        }
    }

    public bool IsEnemy()
    {
        return isEnemy;
    }

    public bool IsDiscovered()
    {
        return enemyDiscovered;
    }

    public bool IsInfected()
    {
        return isInfected;
    }

    public int NumberOfConnections()
    {
        return connectedCircles.Count;
    }
}
