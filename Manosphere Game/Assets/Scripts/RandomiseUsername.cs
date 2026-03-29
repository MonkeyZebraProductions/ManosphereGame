using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
//using UnityEngine.TextCore;

public class RandomiseUsername : MonoBehaviour
{

    [SerializeField] TextAsset UsernameFile;
    List<string> usernames;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        usernames = new List<string>();
        usernames.AddRange(UsernameFile.text.Split());
        for (int i = usernames.Count - 1; i >= 0; i--)
        {
            if (string.IsNullOrEmpty(usernames[i]))
            {
                usernames.RemoveAt(i);
            }
        }
        GetComponent<TextMeshProUGUI>().text = usernames[Random.Range(0, usernames.Count)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
