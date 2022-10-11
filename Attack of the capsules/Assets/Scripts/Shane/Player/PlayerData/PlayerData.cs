using UnityEngine;

//Player Data only has player points for now - player points are needed to interact with certain items in the game (unlock doors, buy guns ammo etc).
public class PlayerData : MonoBehaviour
{
    //Components.
    UIBehaviour uiBehaviour;

    public int points;

    void Start()
    {
        uiBehaviour = GameObject.FindGameObjectWithTag("UI").GetComponent<UIBehaviour>();
        uiBehaviour.UpdatePointsCounter(points);
    }
    public void AddPoints(int pointsToAdd)
    {
        points += pointsToAdd;
        uiBehaviour.UpdatePointsCounter(points);
    }
    public void RemovePoints(int pointsToRemove)
    {
        points -= pointsToRemove;
        uiBehaviour.UpdatePointsCounter(points);
    }
}
