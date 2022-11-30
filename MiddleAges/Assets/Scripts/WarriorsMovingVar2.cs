using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorsMovingVar2 : MonoBehaviour
{
    public int lenStep;
    public GameObject pointOfWar;

    void Start()
    {
        Vector3 warPos;
        int stepx, stepz, lim;//lim is (max length of warriors's line)/2

        transform.position = GameObject.Find("Player").transform.position;

        if (transform.childCount > 24)
            lim = 4;//max is 48 warriors
        else if (transform.childCount > 8)
            lim = 2;
        else
            lim = 1;

        stepx = -lim * lenStep;
        stepz = -lim * lenStep;

        for (int i = 0; i < transform.childCount; i++)
        {
            warPos = new Vector3(stepx, 0, stepz);

            if (warPos != new Vector3(0, 0, 0))
            {
                transform.GetChild(i).gameObject.transform.localPosition = warPos;
                Instantiate(pointOfWar, GameObject.Find("Player").transform);
                pointOfWar.transform.localPosition = warPos;
                transform.GetChild(i).gameObject.GetComponent<Warrior>().index = i;
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

    private void Update()
    {
        //transform.position = GameObject.Find("Player").transform.position;
    }
}
