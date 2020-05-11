using System.Collections;
using UnityEngine;

//Casing can be ejected from gun when gun is fired.
//Casing fades over time and the destroys itself.
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class BulletCasing : MonoBehaviour
{
    Rigidbody rigidbody;
    AudioSource audioSource;

    [Header("Fading")]
    public float lifetime = 2;
    public float fadeSpeed = 2;

    [Header("Ejection settings")]
    public float minEjectionForce = 1;
    public float maxEjectionForce = 5;

    [Header("Audio on hitting ground")]
    public AudioClip[] hitFloorAudio;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.AddForce(transform.right * Random.Range(minEjectionForce,maxEjectionForce), ForceMode.Impulse);
        rigidbody.AddTorque(Random.onUnitSphere, ForceMode.Impulse);
        StartCoroutine(FadeCasing());
    }

 
    IEnumerator FadeCasing()
    {
        yield return new WaitForSeconds(lifetime);

        Material casingMaterial = GetComponent<MeshRenderer>().material;
        Color originalColour = casingMaterial.color;
        Color targetColour = new Color(originalColour.r, originalColour.g, originalColour.b, 0);
        float percentageComplete = 0;

        while(percentageComplete <= 1)
        {
            percentageComplete += Time.deltaTime * fadeSpeed;
            casingMaterial.color = Color.Lerp(originalColour, targetColour,percentageComplete);
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        audioSource.PlayOneShot(hitFloorAudio[Random.Range(0, hitFloorAudio.Length)]);
    }

}
