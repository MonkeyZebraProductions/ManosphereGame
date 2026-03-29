using UnityEngine;
using UnityEngine.UI;

public class RandomisePFP : MonoBehaviour
{
    [SerializeField] Sprite[] PFPsprites;
    private Image PFP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PFP = GetComponent<Image>();
        PFP.sprite = PFPsprites[Random.Range(0,PFPsprites.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
