using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private int ScoreIncrease = 1;
    public int TotalScore;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ScoreText.text = TotalScore.ToString();
    }

    //use for when lines are out
    public void ChangeScore(bool increaseScore, int scoreMultiplier)
    {
        TotalScore += (increaseScore ? 1 : -1) * ScoreIncrease * scoreMultiplier;
    }

    //Use for breaking connections
    public void AddOneTimeScore(int score)
    {
        TotalScore += score;
        ScoreText.text = TotalScore.ToString();
    }
}
