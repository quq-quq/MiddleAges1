using System.Collections;
using UnityEngine;

public class WarriorsMoving : MonoBehaviour
{
    public static Vector3[] WarriorPositions;

    public int lenStep;

    [SerializeField] private Transform allWarriorsParent;

    private void Start()
    {
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

}
