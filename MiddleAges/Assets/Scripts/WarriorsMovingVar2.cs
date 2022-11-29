using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorsMovingVar2 : MonoBehaviour
{
    public int len;


    void Start()
    {
        Vector3 warPos;
        int stepx = -len, stepz = -len, lim = 0, lim1 = transform.childCount, n = 0;
        while (lim1 > 0)
        {
            lim1 -= 8 * (n + 1);
            n += 1;
            lim += 1;
        }



        Debug.Log(lim);

        for(int i = 0; i < transform.childCount; i++)
        {
            warPos = new Vector3(stepx, 0, stepz);

            if(warPos != new Vector3 (0, 0, 0)) 
                transform.GetChild(i).gameObject.transform.localPosition = warPos;


            if (stepx < len && stepz < len)
                stepx += len;
            else if (stepx == len && stepz < len)
            {
                stepz += len;
                stepx = -len;
            }


            //подсчет позиций:
            //if (stepx < len)
            //    stepx += len;
            //else if (stepx == len && stepz < len)
            //{
            //    stepz += len;
            //    stepx = -len;
            //}

            //if(stepx == 0 && stepz == 0)
            //    stepx += len;

            //if( i % 8 == 0)
            //    len = 

        }
        }

    // Update is called once per frame
    void Update()
    {
        
    }
}
