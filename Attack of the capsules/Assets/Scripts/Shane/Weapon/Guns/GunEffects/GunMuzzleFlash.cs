using UnityEngine;

//Activated when gun is fired.
//Turns a sprite on and off at end of gun.
public class GunMuzzleFlash : MonoBehaviour
{
    System.Random rng = new System.Random();
    public GameObject flashHolder;
    public Sprite[] flashSprites;
    public SpriteRenderer[] spriteRenderers;
    public float flashTime;


    private void Start()
    {
        Deactivate();
    }

    public void Activate()
    {
        flashHolder.SetActive(true);
        int flashSpriteIndex = rng.Next(0, flashSprites.Length);
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
        }
        Invoke("Deactivate", flashTime);
    }

    public void Deactivate()
    {
        flashHolder.SetActive(false);
    }

}
