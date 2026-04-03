using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class PrototypeManager : MonoBehaviour
{
    [SerializeField] bool isCircleTouch;
    [SerializeField] bool isTouchscreenSimulation;
    [SerializeField] bool randomizeEnemyCircles;
    [SerializeField] bool isTutorial;

    public UnityEvent AllLinesCut;
    public UnityEvent AllInfectedCured;

    private SpriteManager[] spriteManagers;

    void Awake()
    {
        if (isTouchscreenSimulation)
        {
            EnhancedTouchSupport.Enable();
            TouchSimulation.Enable();
        }
    }

    // Choose up to 4 random circles to be enemies at the start of the game
    void Start()
    {
        Application.targetFrameRate = 30;

        if (randomizeEnemyCircles)
        {
            List<CircleTypes> allCircleTypes = new List<CircleTypes>();
            allCircleTypes.AddRange(FindObjectsByType<CircleTypes>(FindObjectsSortMode.None));
               
            for (int i = 0; i < 4; i++)
            {
                CircleTypes randomCircleTypes = allCircleTypes[Random.Range(0, allCircleTypes.Count)];
                randomCircleTypes.SetToCloseted();
                Circle randomCircle = randomCircleTypes.gameObject.GetComponent<Circle>();
                CircleTouch randomCircleTouch = randomCircleTypes.gameObject.GetComponent<CircleTouch>();
                //randomCircleTypes.gameObject.GetComponent<Circle>().SetEnemy(true);
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

        // Press M to go to the menu
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(0);
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

    public void DetectLineCuts()
    {
        StartCoroutine(NoMoreLines());
    }

    public void DetectInfected()
    {
        spriteManagers = FindObjectsByType<SpriteManager>(FindObjectsSortMode.None);
        StartCoroutine(NoMoreInfected());
    }

    IEnumerator NoMoreLines()
    {
        Line[] lines = FindObjectsByType<Line>(FindObjectsSortMode.None);
        LineTouch[] touchs = FindObjectsByType<LineTouch>(FindObjectsSortMode.None);
        yield return new WaitForSeconds(0.2f);
        if (lines.Length==0  && touchs.Length == 0)
        {
            AllLinesCut.Invoke();
        }
        else
        {
            StartCoroutine(NoMoreLines());
        }
    }

    IEnumerator NoMoreInfected()
    {
        yield return new WaitForSeconds(0.2f);
        int infectedCount = 0;
        foreach (SpriteManager sprite in spriteManagers)
        {
            if(sprite.BaseRenderer.sprite == sprite.InfectedBase)
            {
                infectedCount ++;
            }
        }

        if (infectedCount != 0)
        {
            StartCoroutine(NoMoreInfected());
        }
        else
        {
            AllInfectedCured.Invoke();
        }
    }
}
