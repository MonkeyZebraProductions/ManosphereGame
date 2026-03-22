using UnityEngine;

public class Line : MonoBehaviour
{
    GameObject circle0;
    GameObject circle1;

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
    }
}
