using System;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomTweetStat : MonoBehaviour
{
   
    [SerializeField] TextMeshProUGUI ResharkText,BiteText,CommentText;
    [SerializeField] int MinValue;
    [SerializeField] int MaxValue = 999999999;
    [SerializeField] int ValueDeviationPercentage = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //TweetText = GetComponent<TextMeshProUGUI>();
        int tweetValue = Random.Range(MinValue, MaxValue);
        ResharkText.text = Format(Random.Range(tweetValue-tweetValue/ValueDeviationPercentage, tweetValue + tweetValue/ValueDeviationPercentage));
        BiteText.text = Format(Random.Range(tweetValue - tweetValue / ValueDeviationPercentage, tweetValue + tweetValue / ValueDeviationPercentage));
        CommentText.text = Format(Random.Range(tweetValue - tweetValue / ValueDeviationPercentage, tweetValue + tweetValue / ValueDeviationPercentage));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private static readonly Dictionary<int, string> units = new Dictionary<int, string>()
    {
        {0, ""},
        {1,"K"},
        {2,"M"},
        {3,"B"},
        {4,"T"},

    };
    private static readonly int charA = Convert.ToInt32('a');

    public static string Format(double value)
    {
        if (value < 1d)
        {
            return "0";
        }

        var n = (int)Math.Log(value, 1000);
        var m = value / Math.Pow(1000, n);
        var unit = "";
        if (n < units.Count)
        {
            unit = units[n];
        }
        else
        {
            var unitInt = n - units.Count;
            var secondUnit = unitInt % 26;
            var firstUnit = unitInt / 26;
            unit = Convert.ToChar(firstUnit + charA).ToString() + Convert.ToChar(secondUnit + charA).ToString();
        }

        return (Math.Floor(m * 100) / 100).ToString("0.#") + unit;
    }
}
