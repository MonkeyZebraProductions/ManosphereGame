using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public struct CircleStruct
{
    public LocalizedStringTable LocalizedStringTable;
    public Sprite CircleSprite;
}
public enum CircleEnum
{
    Gamer,
    Sports,
    Film,
    Music,
    Enemy
}
public class CircleTypes : MonoBehaviour
{
    public CircleStruct GamerStruct;
    public CircleStruct SportsStruct;
    public CircleStruct FilmStruct;
    public CircleStruct MusicStruct;
    public CircleStruct EnemyStuct;

    [SerializeField] 
    private CircleEnum CircleEnum;

    public CircleStruct ChosenStuct;

    private void Awake()
    {
        switch (CircleEnum)
        {
            case CircleEnum.Gamer:
                ChosenStuct = GamerStruct;
                break;
        }
        Debug.Log(ChosenStuct.ToString());
        GetComponentInChildren<SelectRandomString>().circleType = this;
        GetComponentInChildren<SelectRandomString>().enabled = true;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
