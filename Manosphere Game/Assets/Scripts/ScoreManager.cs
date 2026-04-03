using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI ScoreText;
    [SerializeField] private int ScoreIncrease = 1;
    public int TotalScore;

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

    public void EndingScene()
    {
        PlayerPrefs.SetInt("FinalScore", TotalScore);

        if (TotalScore >= 4000)
        {
            SceneManager.LoadScene("GoodEndingScene");
        }
        else
        {
            SceneManager.LoadScene("BadEndingScene");
        }
    }
}
