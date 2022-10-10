using UnityEngine;

//Base Weapon Script.
[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
    //Components.
    public Animator animator;
    public AudioSource audioSource;
    protected UIBehaviour uiBehaviour;
    public InventoryUISlot inventoryUISlot;

    [Header("Player Inventory")]
    public string nameOfWeapon;
    public Weapon weaponPrefab;
    public int playerInventoryIndex;

    [Header("Weapon Position")]
    public Vector3 defaultWeaponPosition;

    [Header("Weapon Targets")]
    public float damage;
    public LayerMask targetMask;

    public virtual void Start()
    {

    }

    public virtual void Awake()
    {
        uiBehaviour = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBehaviour>();
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void Update()
    {

    }

    public virtual void OnDisable()
    {

    }

}
