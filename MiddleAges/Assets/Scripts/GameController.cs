using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public int StartCapitanIndex = 0;

    [SerializeField] private Transform[] WarriorsTeams;
    [SerializeField] private int lenStep;

    public static Vector3[,] WarriorPositions;
    public static List<Warrior> WarriorsScript = new ();
    public static List<EntityBehaviour> EnemiesScript = new();
    public static List<Player> CapitansScripts = new();
    public static List<House> HousesScripts = new();

    public float VignetteIntensity = 0;
    private Vignette vignette;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        WarriorPositions = new Vector3[WarriorsTeams.Length, WarriorsTeams[0].childCount];
        CalculateWarriorsPos(0);
        CalculateWarriorsPos(1);
        vignette = transform.GetComponentInChildren<PostProcessVolume>().profile.GetSetting<Vignette>();
        Player.instance.DeactivatePlayer();
        Player.instance = CapitansScripts[0];
        Player.instance.ActivatePlayer();
        CameraMoving.target = CapitansScripts[0].transform;
    }
    private void CalculateWarriorsPos(int capitanIdx)
    {

        Vector3 warPos;
        int stepx, stepz, lim;//lim is (max length of warriors's line)/2

        if (WarriorsTeams[capitanIdx].childCount > 24)
            lim = 4;//max is 48 warriors
        else if (WarriorsTeams[capitanIdx].childCount > 8)
            lim = 2;
        else
            lim = 1;

        stepx = -lim * lenStep;
        stepz = -lim * lenStep;

        for (int i = 0; i < WarriorsTeams[capitanIdx].childCount; i++)
        {
            warPos = new Vector3(stepx, 0, stepz);
            if (warPos != Vector3.zero)
            {
                WarriorPositions[capitanIdx, i] = warPos;
                WarriorsTeams[capitanIdx].GetChild(i).GetComponent<Warrior>().index = i;
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
        vignette.intensity.Override(Mathf.Lerp(vignette.intensity, VignetteIntensity, Time.deltaTime*2));
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Player.instance.DeactivatePlayer();
            Player.instance = CapitansScripts[0];
            Player.instance.ActivatePlayer();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Player.instance.DeactivatePlayer();
            Player.instance = CapitansScripts[1];
            Player.instance.ActivatePlayer();
        }
    }

    public void RemoveCurse()
    {
        for (int i = 0; i < WarriorsScript.Count; i++)
            WarriorsScript[i].SlowlingCurse = true;
        for (int i = 0; i < CapitansScripts.Count; i++)
            CapitansScripts[i].ChangeSpeed();
        VignetteIntensity = 0;
    }

}
