using System.Collections;
using UnityEngine;

/* Barricade is like a wooden plank that can be at a defence point stopping enemies from entering.
   PlayerDefencePointBehaviour script checks if player keeps hold of F, if they hold it long enough a barricade spawns, rotates a certain amount, then moves to position to barricade defence point. 
 */
[RequireComponent(typeof(HealthComponent))]
public class Barricade : MonoBehaviour
{
    //Components.
    UIBehaviour uiBehaviour;
    HealthComponent healthComponent;

    [Header("Defence Point Info")]
    public DefencePoint defencePoint; //Assigned at spawn in defencepoint script.
    public int index;

    private void Awake()
    {
        uiBehaviour = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBehaviour>();
        healthComponent = GetComponent<HealthComponent>();
        healthComponent.OnDeathEvent += DestroyBarricade;
    }

    //Repairing defence point.
    public IEnumerator MoveToPosition(Transform targetPosition, float moveSpeed)
    {
        float percentageComplete = 0;

        while (percentageComplete <= 1)
        {
            percentageComplete += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(transform.position, targetPosition.position, percentageComplete);

            yield return null;
        }
    }
    public IEnumerator RotateIntoPosition(float targetAngle, float moveSpeed)
    {
        float percentageComplete = 0;

        while (percentageComplete <= 1)
        {
            percentageComplete += Time.deltaTime * moveSpeed;

            float angle = Mathf.Lerp(0, targetAngle, percentageComplete);
            transform.localEulerAngles = new Vector3(0, 0, angle);

            yield return null;
        }
    }
    public IEnumerator BarricadeDefencePoint(Transform targetPosition, float targetAngle, float moveSpeed)
    {
        yield return RotateIntoPosition(targetAngle, moveSpeed);
        yield return MoveToPosition(targetPosition, moveSpeed);
    }

    //Destroying Barricade.
    public void DestroyBarricade()
    {
        if (defencePoint.barricadeIndex >= 0 && defencePoint.barricadeIndex < defencePoint.TotalNumberOfBarricades)
        {
            GameObject barricadeToDestroy = defencePoint.ActiveBarricades[defencePoint.barricadeIndex].gameObject;
            Destroy(barricadeToDestroy);
            defencePoint.barricadeIndex--;

            if (defencePoint.defencePointPlayerBehaviour.PlayerInRange && !uiBehaviour.IsNotificationMessageOnScreen())
                uiBehaviour.ShowPlayerInteractMessage("Press and hold F to repair window", true,Color.white);

        }
    }
}
