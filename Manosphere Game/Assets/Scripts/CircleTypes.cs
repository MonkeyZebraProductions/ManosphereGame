using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public struct CircleStruct
{
    public LocalizedStringTable LocalizedStringTable;
    //public Sprite CircleSprite;
}
public enum CircleEnum
{
    Gamer,
    Sports,
    Film,
    Music,
    Enemy,
    Closeted
}
public class CircleTypes : MonoBehaviour
{
    public CircleStruct GamerStruct;
    public CircleStruct SportsStruct;
    public CircleStruct FilmStruct;
    public CircleStruct MusicStruct;
    public CircleStruct EnemyStuct;
    public CircleStruct ClosetedStuct;

    [SerializeField] 
    private CircleEnum CircleEnum;

    public CircleStruct ChosenStuct;
    private CircleStruct originalStruct;

    private SelectRandomString randomString;

    private void Awake()
    {
        switch (CircleEnum)
        {
            case CircleEnum.Gamer:
                ChosenStuct = originalStruct = GamerStruct;
                break;
        }
        randomString = GetComponentInChildren<SelectRandomString>();
        randomString.circleType = this;
        randomString.enabled = true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConvertToEnemy()
    {
        randomString.enabled = false;
        ChosenStuct = EnemyStuct;
        randomString.enabled=true;

    }

    public void ConvertToNormal()
    {
        randomString.enabled = false;
        ChosenStuct = originalStruct;
        randomString.enabled = true;
    }
}
