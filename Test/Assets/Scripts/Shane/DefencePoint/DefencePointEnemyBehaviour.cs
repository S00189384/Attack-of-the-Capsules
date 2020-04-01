using System.Collections.Generic;
using UnityEngine;

public class DefencePointEnemyBehaviour : MonoBehaviour
{
    System.Random rng = new System.Random();

    DefencePoint defencePoint;

    [Header("Enemies At Defence Point")]
    public List<BreakInEnemy> enemyList = new List<BreakInEnemy>();

    [Header("Enemy Numbers")]
    public int maxNumberOfEnemies;
    public int numberOfEnemiesActive = 0;
    public int numberOfEnemiesInFirstRow = 3;

    [Header("Enemy Positions")]
    public Transform[] enemyPositions = new Transform[6];
    public bool[] IsEnemyInPosition = new bool[6];

    private void Start()
    {
        defencePoint = GetComponentInParent<DefencePoint>();
    }

    //Assigning BreakInEnemy info at spawn.
    public void AddEnemyToList(BreakInEnemy enemy)
    {
        enemyList.Add(enemy);
        numberOfEnemiesActive++;
    }
    public void GiveEnemyTargetPositionAtSpawn(BreakInEnemy enemy)
    {
        for (int i = 0; i < enemyPositions.Length; i++)
        {
            if(!IsEnemyInPosition[i])
            {
                enemy.MoveTo(enemyPositions[i].transform.position);
                IsEnemyInPosition[i] = true;
                enemy.DefencePointIndex = i;
                break;
            }
        }
    }

    //When enemy in list gets killed or enters building.
    public void OnEnemyLeaveDefencePoint(BreakInEnemy enemyToLeave)
    {
        int IndexOfEnemyThatIsLeaving = enemyToLeave.DefencePointIndex;

        //If there are enemies in the back row - And the enemy that is leaving is in the first row.
        //Move one of the back row enemies to the front row.
        if (numberOfEnemiesActive > numberOfEnemiesInFirstRow && enemyToLeave.DefencePointIndex < numberOfEnemiesInFirstRow)
        {
            //Get random enemy in back row.
            List<BreakInEnemy> backRowEnemies = enemyList.FindAll(e => e.DefencePointIndex >= numberOfEnemiesInFirstRow);
            BreakInEnemy enemyToMove = backRowEnemies[rng.Next(0, backRowEnemies.Count)];

            //Move random enemy to front row.
            enemyToMove.MoveTo(enemyToLeave.target);
            IsEnemyInPosition[enemyToMove.DefencePointIndex] = false;
            enemyToMove.DefencePointIndex = enemyToLeave.DefencePointIndex;
        }

        else // No enemies in back row
        {
            IsEnemyInPosition[IndexOfEnemyThatIsLeaving] = false;
        }

        enemyToLeave.IsAtDefencePoint = false;
        enemyList.Remove(enemyToLeave);

        numberOfEnemiesActive--;
    }

    //Does defence point have max number of enemies 
    public bool HasMaxNumberOfEnemies()
    {
        return numberOfEnemiesActive >= maxNumberOfEnemies;
    }

    //If Enemy reaches defence point.
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "BreakInEnemy")
            other.GetComponent<BreakInEnemy>().IsAtDefencePoint = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "BreakInEnemy")
            other.GetComponent<BreakInEnemy>().IsAtDefencePoint = false;
    }
}
