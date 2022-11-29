using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorsMovingVar2 : MonoBehaviour
{
    public int len;


    void Start()
    {
        Vector3 warPos;
        int stepx = -len, stepz = -len, lim = transform.childCount/16;
        Debug.Log(lim);

        for(int i = 0; i < transform.childCount; i++)
        {
            warPos = new Vector3(stepx, 0, stepz);

            if(warPos != new Vector3 (0, 0, 0)) 
                transform.GetChild(i).gameObject.transform.localPosition = warPos;

            //подсчет позиций:
            if (stepx < len)
                stepx += len;
            else if (stepx == len && stepz < len)
            {
                stepz += len;
                stepx = -len;
            }

            if(stepx == 0 && stepz == 0)
                stepx += len;

            //if( i % 8 == 0)
            //    len = 

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
