using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    [SerializeField] private Transform allWarriorsParent;
    [SerializeField] private int lenStep;

    [NonSerialized] public Vector3[] WarriorPositions;
    [NonSerialized] public List<EntityBehaviour> WarriorsScript = new List<EntityBehaviour>();
    [NonSerialized] public List<EntityBehaviour> EnemiesScript = new List<EntityBehaviour>();
    [NonSerialized] public List<House> HousesScripts = new List<House>();

    [NonSerialized] public Vignette Vignette;
    [NonSerialized] public float VignetteIntensity;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        CalculateWarriorsPos();
        Vignette = transform.GetComponentInChildren<PostProcessVolume>().profile.GetSetting<Vignette>();
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
    private void FixedUpdate()
    {
        Vignette.intensity.Override(Mathf.Lerp(Vignette.intensity, VignetteIntensity, Time.fixedDeltaTime*2));
    }
    public void RemoveCurse()
    {
        Player.instance.SetDefaultSpeed();
        for (int i = 0; i < WarriorsScript.Count; i++)
            WarriorsScript[i].SetDefaultSpeed();
        VignetteIntensity = 0;
    }

}
