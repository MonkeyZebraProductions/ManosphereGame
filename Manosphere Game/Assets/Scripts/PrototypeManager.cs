using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PrototypeManager : MonoBehaviour
{
    // Choose 3 random circles to be enemies at the start of the game
    void Start()
    {
        Circle[] allCircles = FindObjectsByType<Circle>(FindObjectsSortMode.None);
        for (int i = 0; i < 3; i++)
        {
            Circle randomCircle = allCircles[Random.Range(0, allCircles.Length)];
            randomCircle.SetEnemy(true);
        }
    }

    void Update()
    {
        // Press R to reset the game
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Press Q to quit the game
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Application.Quit();
        }
    }
}
