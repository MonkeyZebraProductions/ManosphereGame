using UnityEngine;

public class GlowManager : MonoBehaviour
{
    [SerializeField] GameObject glowObject;
    [SerializeField] SpriteRenderer glowSpriteRenderer;
    [SerializeField] Sprite whiteGlow;
    [SerializeField] Sprite redGlow;
    [SerializeField] Sprite blueGlow;
    [SerializeField] Sprite greenGlow;

    public void TurnOnGlow(string color)
    {
        switch (color)
        {
            case "white":
                glowSpriteRenderer.sprite = whiteGlow;
                break;
            case "red":
                glowSpriteRenderer.sprite = redGlow;
                break;
            case "blue":
                glowSpriteRenderer.sprite = blueGlow;
                break;
            case "green":
                glowSpriteRenderer.sprite = greenGlow;
                break;
            default:
                break;
        }

        glowObject.SetActive(true);
    }

    public void TurnOffGlow()
    {
        glowObject.SetActive(false);
    }
}
