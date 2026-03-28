using UnityEngine;

public class SetPopupPosition : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField] private Transform NorthTransform;
    [SerializeField] private Transform SouthTransform;
    [SerializeField] private Transform EastTransform;
    [SerializeField] private Transform WestTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.parent.position);
        float screenWidth = Camera.main.pixelWidth;
        float screenHeight = Camera.main.pixelHeight;

        if(screenPos.x > screenWidth*0.75)
        {
            rectTransform.position = WestTransform.position;
        }
        if(screenPos.x<screenWidth*0.25)
        {
            rectTransform.position = EastTransform.position;
        }
        if(screenPos.y > screenHeight*0.75)
        {
            rectTransform.position = SouthTransform.position;
        }
        if(screenPos.y < screenHeight*0.25)
        {
            rectTransform.position = NorthTransform.position;
        }
        Debug.Log(screenPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
