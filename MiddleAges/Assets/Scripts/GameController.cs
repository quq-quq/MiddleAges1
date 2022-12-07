using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public static Vector3[] WarriorPositions;
    [SerializeField] private Transform allWarriorsParent;
    [SerializeField] private int lenStep;

    public static List<EntityBehaviour> WarriorsScript = new List<EntityBehaviour>();
    public static List<EntityBehaviour> EnemiesScript = new List<EntityBehaviour>();
    public static List<House> HousesScripts = new List<House>();

    private void Start()
    {
        instance = this;
        CalculateWarriorsPos();
    }

    private void CalculateWarriorsPos()
    {
        WarriorPositions = new Vector3[allWarriorsParent.childCount];

        Vector3 warPos;
        int stepx, stepz, lim;//lim is (max length of warriors's line)/2

        if (allWarriorsParent.childCount > 24)
            lim = 4;//max is 48 warriors
        else if (allWarriorsParent.childCount > 8)
            lim = 2;
        else
            lim = 1;

        stepx = -lim * lenStep;
        stepz = -lim * lenStep;

        for (int i = 0; i < allWarriorsParent.childCount; i++)
        {
            warPos = new Vector3(stepx, 0, stepz);
            if (warPos != Vector3.zero)
            {
                WarriorPositions[i] = warPos;
                allWarriorsParent.GetChild(i).GetComponent<Warrior>().index = i;
            }


            //calculating positions
            if (stepx < lim * lenStep && stepz <= lim * lenStep)
                stepx += lenStep;
            else if (stepx == lim * lenStep && stepz < lim * lenStep)
            {
                stepz += lenStep;
                stepx = -lim * lenStep;
            }

            if (stepx == 0 && stepz == 0)
                stepx += lenStep;
            //this is alright
        }
    }

    public void OneEntityDie(EntityBehaviour entity)
    {
        WarriorsScript.Remove(entity);
        entity.gameObject.SetActive(false);
        CalculateWarriorsPos();
    }

    public void RemoveCurse()
    {
        for (int i = 0; i < WarriorsScript.Count; i++)
        {
            WarriorsScript[i].SetSpeed(15f);
            Player.plTransform.GetComponent<Player>().SetSpeed(15f);
        }
    }

}
