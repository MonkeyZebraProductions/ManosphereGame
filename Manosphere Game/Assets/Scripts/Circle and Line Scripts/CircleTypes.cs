using UnityEngine;
using UnityEngine.Localization;

[System.Serializable]
public struct CircleStruct
{
    public LocalizedStringTable LocalizedStringTable;
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
    public CircleStruct ClosetedStruct;

    [SerializeField] 
    public CircleEnum StartingCircleEnum;

    public CircleStruct ChosenStuct;
    private CircleStruct originalStruct;

    private SelectRandomString randomString;

    private void Awake()
    {

        CircleEnum[] circleEnums = { CircleEnum.Closeted, CircleEnum.Gamer, CircleEnum.Film, CircleEnum.Music, CircleEnum.Sports };

        StartingCircleEnum = circleEnums[Random.Range(0, circleEnums.Length)];
        switch (StartingCircleEnum)
        {
            case CircleEnum.Gamer:
                ChosenStuct = originalStruct = GamerStruct;
                break;
            case CircleEnum.Sports:
                ChosenStuct = originalStruct = SportsStruct;
                break;
            case CircleEnum.Film:
                ChosenStuct = originalStruct = FilmStruct;
                break;
            case CircleEnum.Closeted:
                ChosenStuct = originalStruct = ClosetedStruct;
                break;
            case CircleEnum.Music:
                ChosenStuct = originalStruct = MusicStruct;
                break;
        }

        randomString = GetComponentInChildren<SelectRandomString>();
        randomString.circleType = this;
        randomString.enabled = true;
    }

    public void ConvertToEnemy()
    {
        randomString.enabled = false;
        ChosenStuct = EnemyStuct;
        randomString.enabled = true;
    }

    public void ConvertToNormal()
    {
        randomString.enabled = false;
        ChosenStuct = originalStruct;
        randomString.enabled = true;
    }
}
