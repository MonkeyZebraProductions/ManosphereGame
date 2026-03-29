using UnityEngine;


public class SpriteManager : MonoBehaviour
{
    [Header("Base Sprites")]
    [SerializeField] private Sprite[] DefaultBases;
    private Sprite chosenDefaultBase;
    [SerializeField] private Sprite InfectedBase;
    [SerializeField] private Sprite EnemyBase;
    [SerializeField] private SpriteRenderer BaseRenderer;

    [Header("Emotion Sprites")]
    [SerializeField] private Sprite AloneEmotion;
    [SerializeField] private Sprite ConnectedEmotion;
    [SerializeField] private Sprite HappyEmotion;
    [SerializeField] private Sprite InfectedEmotion;
    [SerializeField] private SpriteRenderer EmotionRenderer;

    [Header("Accessories Sprites")]
    [SerializeField] private Sprite[] DefaultAccessories;
    [SerializeField] private SpriteRenderer AccessoryRenderer;
    private int accessoryIndex;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chosenDefaultBase = DefaultBases[Random.Range(0, DefaultBases.Length)];
        BaseRenderer.sprite = chosenDefaultBase;
        EmotionRenderer.sprite = AloneEmotion;
        accessoryIndex = Random.Range(0, DefaultAccessories.Length);
        AccessoryRenderer.sprite = DefaultAccessories[accessoryIndex];
    }

    public void ChangeEmotion(Emotion newEmotion)
    {
        switch(newEmotion)
        {
            case Emotion.Alone:
                EmotionRenderer.sprite = AloneEmotion;
                break;
            case Emotion.Connected:
                EmotionRenderer.sprite = ConnectedEmotion;
                break;
            case Emotion.Happy:
                EmotionRenderer.sprite= HappyEmotion;
                break;
            case Emotion.Infected:
                EmotionRenderer.sprite = InfectedEmotion;
                break;
        }
    }

    public void ChangeBase(Base newBase)
    {
        switch(newBase)
        {
            case Base.Normal:
                BaseRenderer.sprite = chosenDefaultBase;
                if(accessoryIndex <=3 && !AccessoryRenderer.enabled)
                {
                    AccessoryRenderer.enabled = true;
                }
                break;
            case Base.Infected:
                BaseRenderer.sprite = InfectedBase;
                if (accessoryIndex <= 3 && AccessoryRenderer.enabled)
                {
                    AccessoryRenderer.enabled = false;
                }
                ChangeEmotion(Emotion.Infected);
                break;
            case Base.Enemy:
                BaseRenderer.sprite = EnemyBase;
                if (accessoryIndex <= 3 && AccessoryRenderer.enabled)
                {
                    AccessoryRenderer.enabled = false;
                }
                ChangeEmotion(Emotion.Infected);
                break;
        }
    }

    public void MoveSpriteLayer(bool moveToFront)
    {
        EmotionRenderer.sortingLayerName = moveToFront ? "Popup" : "Default";
        BaseRenderer.sortingLayerName = moveToFront ? "Popup" : "Default";
        AccessoryRenderer.sortingLayerName = moveToFront ? "Popup" : "Default";
    }
}
