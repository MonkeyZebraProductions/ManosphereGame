using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.InputSystem.Controls;

public class CircleTouch : MonoBehaviour
{
    [SerializeField] GameObject linePrefab;
    [SerializeField] float maxConnectionDistance = 3f;
    [SerializeField] bool isEnemy;
    [SerializeField] float infectionTime = 5f;
    [SerializeField] GameObject glow;

    GameObject linesParent;
    GameObject currentLine;
    LineRenderer currentLineRenderer;
    bool isDragging;
    InputAction touchAction;
    TouchControl currentTouch;
    List<GameObject> connectedCircles = new List<GameObject>();
    bool isInfected;
    float timeToInfect;
    float nextInfectionTime;
    bool hasTouch;

    private SpriteManager spriteManager;
    private CircleTypes circleType;

    // Prevents players from connecting lines to enemies once they have been discovered
    bool enemyDiscovered;

    void Start()
    {
        isDragging = false;
        isInfected = false;
        hasTouch = false;
        linesParent = GameObject.Find("Connections");
        touchAction = InputSystem.actions.FindAction("Touch");
        timeToInfect = 0f;
        nextInfectionTime = Random.Range(infectionTime, infectionTime*1.5f);
        circleType = GetComponent<CircleTypes>();
        spriteManager = GetComponentInChildren<SpriteManager>();
        //isEnemy = (circleType.StartingCircleEnum == CircleEnum.Closeted);
    }

    void Update()
    {
        // Check which touch is currently interacting with the circle
        if (touchAction.IsPressed() && !hasTouch)
        {
            for (int i = 0; i < Touchscreen.current.touches.Count; i++)
            {
                TouchControl touch = Touchscreen.current.touches[i];
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position.ReadValue());
                touchPosition.z = 0;

                if (Vector3.Distance(touchPosition, transform.position) <= GetComponent<CircleCollider2D>().radius + 0.3f)
                {
                    currentTouch = touch;
                    hasTouch = true;
                    break;
                }
            }
        }
        else if (hasTouch && !touchAction.IsPressed())
        {
            hasTouch = false;
        }

        // Prevents players from connecting lines from enemies once they have been discovered or if the circle is already infected
        if (hasTouch && !enemyDiscovered && !isInfected && currentTouch.isInProgress)
        {
            if (!isDragging)
            {
                // Start line drawing if the touch just began and the touch is close enough to the circle
                if (currentTouch.phase.ReadValue() == UnityEngine.InputSystem.TouchPhase.Began && PositionIsOverCircle())
                {
                    currentLine = Instantiate(linePrefab, linesParent.transform);
                    currentLineRenderer = currentLine.GetComponent<LineRenderer>();
                    currentLineRenderer.SetPosition(0, transform.position);
                    glow.SetActive(true);
                    
                    Vector3 touchPosition = Camera.main.ScreenToWorldPoint(currentTouch.position.ReadValue());
                    touchPosition.z = 0;
                    currentLineRenderer.SetPosition(1, touchPosition);
                    
                    isDragging = true;
                }
            }
            else
            {
                // Continue drawing the line to follow the touch position
                Vector3 touchPosition = Camera.main.ScreenToWorldPoint(currentTouch.position.ReadValue());
                touchPosition.z = 0;

                if (Vector3.Distance(transform.position, touchPosition) > maxConnectionDistance)
                {
                    Vector3 direction = (touchPosition - transform.position).normalized;
                    touchPosition = transform.position + direction * maxConnectionDistance;
                }

                currentLineRenderer.SetPosition(1, touchPosition);
            }
        }
        else if (isDragging)
        {
            isDragging = false;
            hasTouch = false;

            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(currentTouch.position.ReadValue());
            touchPosition.z = 0;

            if (Vector3.Distance(transform.position, touchPosition) > maxConnectionDistance)
            {
                Vector3 direction = (touchPosition - transform.position).normalized;
                touchPosition = transform.position + direction * maxConnectionDistance;
            }

            Collider2D hitCollider = Physics2D.OverlapCircle(touchPosition, 0.3f, LayerMask.GetMask("No Cut"));

            // Check if the line ends over another circle that is not the same as the starting circle, is not already connected and is not an enemy that has already been discovered
            if (hitCollider != null && hitCollider.gameObject != gameObject && hitCollider.CompareTag("Circle") && !connectedCircles.Contains(hitCollider.gameObject) && hitCollider.GetComponent<CircleTouch>() != null && !hitCollider.GetComponent<CircleTouch>().IsDiscovered())
            {
                currentLineRenderer.SetPosition(1, hitCollider.transform.position);
                glow.SetActive(false);
                currentLine.GetComponent<LineTouch>().SetCircles(gameObject, hitCollider.gameObject);
                currentLine.GetComponent<LineTouch>().CreateLineCollider();
                
                // Add the connected circle to the list and also add this circle to the other circle's list
                connectedCircles.Add(hitCollider.gameObject);
                if (!isEnemy)
                {
                    spriteManager.ChangeEmotion(Emotion.Connected);
                }
                CircleTouch otherCircleScript = hitCollider.GetComponent<CircleTouch>();
                if (otherCircleScript != null)
                {
                    otherCircleScript.AddConnectedCircle(gameObject);
                    otherCircleScript.spriteManager.ChangeEmotion(Emotion.Connected);
                }

                // If the circle is an enemy, mark it as discovered and infect the other circle
                if (isEnemy)
                {
                    enemyDiscovered = true;
                    spriteManager.ChangeBase(Base.Enemy);
                    currentLine.GetComponent<LineTouch>().InfectLine();
                    if (otherCircleScript != null)
                    {
                        if (otherCircleScript.NumberOfConnections() < 3 && !otherCircleScript.IsEnemy())
                        {
                            otherCircleScript.Infect(true);
                        }
                    }
                }
                // Remove infection from a non-enemy circle if it was infected and is not connected to an infected/enemy circle anymore
                else if (otherCircleScript != null && otherCircleScript.IsInfected() && !otherCircleScript.IsEnemy() && !otherCircleScript.IsConnectedToInfectedOrEnemy())
                {
                    otherCircleScript.Infect(false);
                }
            }
            else
            {
                Destroy(currentLine);
                glow.SetActive(false);
            }
        }

        // Once discovered or infected and connected to an enemy, circles start automatically connecting to each other
        if (enemyDiscovered || (isInfected && IsConnectedToEnemy()))
        {
            timeToInfect += Time.deltaTime;

            if (timeToInfect >= nextInfectionTime)
            {
                timeToInfect = 0f;
                nextInfectionTime = Random.Range(infectionTime, infectionTime*1.5f);

                Collider2D[] collidedCircles = Physics2D.OverlapCircleAll(transform.position, maxConnectionDistance, LayerMask.GetMask("No Cut"));
                List<GameObject> potentialConnections = new List<GameObject>();

                foreach (Collider2D col in collidedCircles)
                {
                    CircleTouch colCircleScript = col.GetComponent<CircleTouch>();

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
                    currentLine.GetComponent<LineTouch>().SetCircles(gameObject, circleToConnect);
                    currentLine.GetComponent<LineTouch>().AnimateLine(transform.position, circleToConnect.transform.position);
                    connectedCircles.Add(circleToConnect);
                    CircleTouch otherCircleScript = circleToConnect.GetComponent<CircleTouch>();
                    currentLine.GetComponent<LineTouch>().InfectLine();
                    if (otherCircleScript != null)
                    {
                        otherCircleScript.AddConnectedCircle(gameObject);
                        otherCircleScript.Infect(true);
                    }
                }
                // If there are no potential circles to connect to and the circle is infected but not an enemy, it should break a random good line that is connected to it
                else if (potentialConnections.Count == 0 && isInfected && !isEnemy)
                {
                    List<GameObject> potentialGoodLinesToBreak = new List<GameObject>();

                    foreach (Transform line in linesParent.transform)
                    {
                        LineTouch lineScript = line.GetComponent<LineTouch>();
                        if (lineScript != null && lineScript.IsGood() && ((lineScript.GetCircle0() == gameObject && lineScript.GetCircle1() != null) || (lineScript.GetCircle1() == gameObject && lineScript.GetCircle0() != null)))
                        {
                            potentialGoodLinesToBreak.Add(line.gameObject);
                        }
                    }

                    if (potentialGoodLinesToBreak.Count > 0)
                        {
                            GameObject lineToBreak = potentialGoodLinesToBreak[Random.Range(0, potentialGoodLinesToBreak.Count)];
                            LineTouch lineScript = lineToBreak.GetComponent<LineTouch>();
                            if (lineScript != null)
                            {
                                lineScript.BreakGoodLine();
                            }
                        }
                }
            }
        }
        // If the circle is infected but not an enemy and not connected to an enemy, it should break a random good line that is connected to it
        else if (isInfected && !IsConnectedToEnemy() && !isEnemy)
        {
            timeToInfect += Time.deltaTime;

            if (timeToInfect >= nextInfectionTime)
            {
                timeToInfect = 0f;
                nextInfectionTime = Random.Range(infectionTime, infectionTime*1.5f);

                List<GameObject> potentialGoodLinesToBreak = new List<GameObject>();

                foreach (Transform line in linesParent.transform)
                {
                    LineTouch lineScript = line.GetComponent<LineTouch>();
                    if (lineScript != null && lineScript.IsGood() && ((lineScript.GetCircle0() == gameObject && lineScript.GetCircle1() != null) || (lineScript.GetCircle1() == gameObject && lineScript.GetCircle0() != null)))
                    {
                        potentialGoodLinesToBreak.Add(line.gameObject);
                    }
                }

                if (potentialGoodLinesToBreak.Count > 0)
                {
                    GameObject lineToBreak = potentialGoodLinesToBreak[Random.Range(0, potentialGoodLinesToBreak.Count)];
                    LineTouch lineScript = lineToBreak.GetComponent<LineTouch>();
                    if (lineScript != null)
                    {
                        lineScript.BreakGoodLine();
                    }
                }
            }
        }
        else
        {
            timeToInfect = 0f;
        }
    }

    public bool PositionIsOverCircle()
    {
        if (currentTouch == null) return false;

        Vector3 touchPosition = Camera.main.ScreenToWorldPoint(currentTouch.position.ReadValue());
        touchPosition.z = 0;
        float distance = Vector3.Distance(touchPosition, transform.position);
        return distance <= GetComponent<CircleCollider2D>().radius + 0.3f;
    }

    public void AddConnectedCircle(GameObject circle)
    {
        if (!connectedCircles.Contains(circle))
        {
            connectedCircles.Add(circle);
            
            // If this circle is an enemy, mark the connected circle as infected if it has less than 3 connections and is not an enemy
            CircleTouch circleScript = circle.GetComponent<CircleTouch>();
            if (circleScript != null && isEnemy)
            {
                enemyDiscovered = true;
                circleScript.BacktrackLineInfection();
                circle.GetComponent<SpriteManager>().ChangeBase(Base.Enemy);
                if (circleScript.NumberOfConnections() < 3 && !circleScript.IsEnemy())
                {
                    circleScript.Infect(true);
                }
            }
        }
    }

    public void RemoveConnectedCircle(GameObject circle)
    {
        if (connectedCircles.Contains(circle))
        {
            connectedCircles.Remove(circle);
        }

        // Remove infection from the circle if it was infected and is not connected to an infected/enemy circle anymore and has at least 1 connection with another non-infected or non-enemy circle
        if (connectedCircles.Count > 0 && IsInfected() && !IsConnectedToInfectedOrEnemy())
        {
            Infect(false);
        }

        //Changes Expression to Alone if not an enemy and has 0 Connections;
        if(connectedCircles.Count == 0 && !isEnemy)
        {
            spriteManager.ChangeEmotion(Emotion.Alone);
        }
    }

    bool IsConnectedToEnemy()
    {
        foreach (GameObject circle in connectedCircles)
        {
            CircleTouch circleScript = circle.GetComponent<CircleTouch>();
            if(circleScript != null && circleScript.IsEnemy())
            {
                return true;
            }
        }
        return false;
    }

    public bool IsConnectedToInfectedOrEnemy()
    {
        foreach (GameObject circle in connectedCircles)
        {
            CircleTouch circleScript = circle.GetComponent<CircleTouch>();
            if(circleScript != null && (circleScript.IsInfected() || circleScript.IsEnemy()))
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
            spriteManager.ChangeBase(Base.Infected); // Light red for infected circles that are not enemies
            circleType.ConvertToEnemy();
        }
        else if (!isEnemy && !infected)
        {
            spriteManager.ChangeBase(Base.Normal); // Back to normal color if the infection is removed
            circleType.ConvertToNormal();
        }

        // If the circle becomes infected and was not already infected, it should also infect all connected circles that are not enemies and have less than 3 connections
        if (infected && !wasAlreadyInfected)
        {
            foreach (GameObject circle in connectedCircles)
            {
                CircleTouch circleScript = circle.GetComponent<CircleTouch>();
                if (circleScript != null && !circleScript.IsEnemy() && circleScript.NumberOfConnections() < 3)
                {
                    circleScript.Infect(true);

                    // Finding the line that connects this circle to the infected circle and marking it as infected as well
                    foreach (Transform line in linesParent.transform)
                    {
                        LineTouch lineScript = line.GetComponent<LineTouch>();
                        if (lineScript != null && ((lineScript.GetCircle0() == gameObject && lineScript.GetCircle1() == circle) || (lineScript.GetCircle1() == gameObject && lineScript.GetCircle0() == circle)))
                        {
                            lineScript.InfectLine();
                            break;
                        }
                    }
                }
            }
        }
    }

    // Backtracks the infection to the line that connected this circle to the one that infected it, so that it also turns red
    public void BacktrackLineInfection()
    {
        currentLine.GetComponent<LineTouch>().InfectLine();
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

    public void SetEnemy(bool enemy)
    {
        isEnemy = enemy;
    }

    // For debugging purposes, visualize the max connection distance in the editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxConnectionDistance);
    }
}
