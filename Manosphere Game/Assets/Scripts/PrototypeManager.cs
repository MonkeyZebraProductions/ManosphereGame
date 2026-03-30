using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;

public class PrototypeManager : MonoBehaviour
{
    [SerializeField] bool isCircleTouch;
    [SerializeField] bool isTouchscreenSimulation;
    [SerializeField] bool randomizeEnemyCircles;
    
    void Awake()
    {
        if (isTouchscreenSimulation)
        {
            EnhancedTouchSupport.Enable();
            TouchSimulation.Enable();
        }
    }

    // Choose up to 3 random circles to be enemies at the start of the game
    void Start()
    {
        Application.targetFrameRate = 30;

        if (randomizeEnemyCircles)
        {
            List<CircleTypes> allCircleTypes = new List<CircleTypes>();
            allCircleTypes.AddRange(FindObjectsByType<CircleTypes>(FindObjectsSortMode.None));
               
            for (int i = 0; i < 3; i++)
            {
                CircleTypes randomCircleTypes = allCircleTypes[Random.Range(0, allCircleTypes.Count)];
                randomCircleTypes.SetToCloseted();
                
                Circle randomCircle = randomCircleTypes.gameObject.GetComponent<Circle>();
                CircleTouch randomCircleTouch = randomCircleTypes.gameObject.GetComponent<CircleTouch>();
                if (randomCircleTouch != null)
                {
                    randomCircleTouch.SetEnemy(true);
                }
                if (randomCircle != null)
                {
                    randomCircle.SetEnemy(true);
                }
                allCircleTypes.Remove(randomCircleTypes);
            }
           
        }
    }

    void Update()
    {
        // Press R to reset the game
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Restart();
        }

        // Press Q to quit the game
        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            Application.Quit();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
