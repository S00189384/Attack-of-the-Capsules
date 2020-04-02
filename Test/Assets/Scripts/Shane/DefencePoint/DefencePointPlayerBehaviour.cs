using UnityEngine;

public delegate void PlayerEnteredAreaDelegate();
public delegate void PlayerLeftAreaDelegate();

public class DefencePointPlayerBehaviour : PlayerInteractableArea
{
    //Components.
    DefencePoint defencePoint;

    //Events.
    public event PlayerEnteredAreaDelegate PlayerEnteredDefencePointArea;
    public event PlayerLeftAreaDelegate PlayerLeftDefencePointArea;

    [Header("Repair Timing")]
    public float TimeToRepairWindow = 2;
    public float RepairTimer = 0;

    public override void Start ()
    {
        base.Start();
        defencePoint = GetComponentInParent<DefencePoint>();
	}
    public override void Update()
    {
        base.Update();
    }

    //Player enters / leaves area.
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if(other.gameObject.tag =="Player")
        {
            PlayerEnteredDefencePointArea?.Invoke();
        }
    }
    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if(other.gameObject.tag == "Player")
        {
            PlayerLeftDefencePointArea?.Invoke();

            uiBehaviour.HidePlayerInteractMessage();
            RepairTimer = 0;
        }
    }

    //Player stays - check if they repair window.
    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject == player)
        {
            CheckIfPlayerTriesToRepairWindow();
        }      
    }
    private void CheckIfPlayerTriesToRepairWindow()
    {
        if (!defencePoint.IsFullyBarricaded())
        {
            uiBehaviour.ShowPlayerInteractMessage("Press and hold F to repair window", true);

            if (PlayerIsInteracting)
            {
                RepairTimer += Time.deltaTime;

                if (RepairTimer >= TimeToRepairWindow)
                {
                    defencePoint.SpawnBarricade();
                    RepairTimer = 0;

                    if (defencePoint.IsFullyBarricaded())
                        uiBehaviour.HidePlayerInteractMessage();
                }
            }
        }
    }
}
