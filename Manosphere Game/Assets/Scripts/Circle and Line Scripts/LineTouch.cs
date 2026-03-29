using UnityEngine;
using System.Collections;

public class LineTouch : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] int lineHealth = 6;
    [SerializeField] float animationSpeed = 6f;
    [SerializeField] float thickness = 0.1f;
    [SerializeField] float endPointMargin = 0.75f;

    [SerializeField] int BreakScore = 50;
    [SerializeField] float ScoreTick = 0.5f;

    GameObject circle0;
    GameObject circle1;
    bool breakable;
    bool animating;
    float animationProgress;
    Vector3 startPos;
    Vector3 endPos;

    private ScoreManager scoreManager;

    void Start()
    {
        animationProgress = 0f;
        scoreManager = FindFirstObjectByType<ScoreManager>();
        StartCoroutine(IncrementScore());
    }

    public void AnimateLine(Vector3 start, Vector3 end)
    {
        startPos = start;
        endPos = end;
        lineRenderer.SetPosition(0, startPos);
        lineRenderer.SetPosition(1, startPos);
        animationProgress = 0f;
        animating = true;
    }

    void Update()
    {
        if (animating)
        {
            animationProgress += Time.deltaTime * animationSpeed;
            lineRenderer.SetPosition(1, Vector3.Lerp(startPos, endPos, animationProgress));

            if (animationProgress >= 1f)
            {
                lineRenderer.SetPosition(1, endPos);
                animating = false;
                animationProgress = 0f;
                CreateLineCollider();
            }
        }
    }

    IEnumerator IncrementScore()
    {
        if (circle0 != null && circle1 != null)
        {
            scoreManager.ChangeScore(!breakable, 1);
        }
        yield return new WaitForSeconds(ScoreTick);
        StartCoroutine(IncrementScore());
    }

    public void SetCircles(GameObject c0, GameObject c1)
    {
        circle0 = c0;
        circle1 = c1;
    }

    public GameObject GetCircle0()
    {
        return circle0;
    }

    public GameObject GetCircle1()
    {
        return circle1;
    }

    public void InfectLine()
    {
        GetComponent<LineRenderer>().startColor = Color.red;
        GetComponent<LineRenderer>().endColor = Color.red;
        breakable = true;
    }

    // Creates a collider using the position 0 and 1 of the line renderer
    public void CreateLineCollider()
    {
        Vector3 startPos = lineRenderer.GetPosition(0);
        Vector3 endPos = lineRenderer.GetPosition(1);

        BoxCollider2D lineCollider = gameObject.AddComponent<BoxCollider2D>();
        lineCollider.isTrigger = true;

        // Set the size of the collider to match the length of the line minus a small amount to prevent it from extending inside the circles
        float lineLength = Vector3.Distance(startPos, endPos);
        lineCollider.size = new Vector2(lineLength - endPointMargin, thickness);

        // Position the collider at the midpoint of the line
        Vector3 midPoint = (startPos + endPos) / 2;
        lineCollider.transform.position = midPoint;

        // Rotate the collider to match the angle of the line
        float angle = Mathf.Atan2(endPos.y - startPos.y, endPos.x - startPos.x) * Mathf.Rad2Deg;
        lineCollider.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Set collider offset to zero so it aligns with the line
        lineCollider.offset = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (breakable && other.CompareTag("Cut"))
        {
            lineHealth--;
            StartCoroutine(Flash());

            if (lineHealth <= 0)
            {
                // Remove the line from the connected circles' lists
                CircleTouch circle0Script = circle0.GetComponent<CircleTouch>();
                CircleTouch circle1Script = circle1.GetComponent<CircleTouch>();

                if (circle0Script != null)
                {
                    circle0Script.RemoveConnectedCircle(circle1);
                }

                if (circle1Script != null)
                {
                    circle1Script.RemoveConnectedCircle(circle0);
                }
                scoreManager.AddOneTimeScore(BreakScore);
                Destroy(gameObject);
            }
        }
    }

    IEnumerator Flash()
    {
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        yield return new WaitForSeconds(0.1f);
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        yield return new WaitForSeconds(0.1f);
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        yield return new WaitForSeconds(0.1f);
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
    }

    public void BreakGoodLine()
    {
        if (!breakable)
        {
            CircleTouch circle0Script = circle0.GetComponent<CircleTouch>();
            CircleTouch circle1Script = circle1.GetComponent<CircleTouch>();

            if (circle0Script != null)
            {
                circle0Script.RemoveConnectedCircle(circle1);
            }

            if (circle1Script != null)
            {
                circle1Script.RemoveConnectedCircle(circle0);
            }

            Destroy(gameObject);
        }
    }

    public bool IsGood()
    {
        return breakable == false;
    }
}
