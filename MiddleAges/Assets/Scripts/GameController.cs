using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    public int StartCapitanIndex = 0;

    [SerializeField] private Text DieText;
    [SerializeField] private Text WinText;
    private Text text;
    [SerializeField] private Image blackImg;

    [SerializeField] private Transform[] WarriorsTeams;
    [SerializeField] private int lenStep;

    public static List<Vector3>[] WarriorPositions;

    public static List<Warrior> WarriorsScript = new ();
    public static List<EntityBehaviour> EnemiesScript = new();
    public static List<Shaman> ShamansScript = new();
    public static List<Player> PlayersScript = new ();
    public static List<House> HousesScript = new();

    [NonSerialized] public float VignetteIntensity = 0;
    private Vignette Vignette;

    private delegate void UpdateMethods();
    private UpdateMethods update;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        Cursor.visible = false;
        WarriorPositions = new List<Vector3>[WarriorsTeams.Length];
        for(int i = 0; i < WarriorsTeams.Length; ++i)
            CalculateWarriorsPos(i);

        update = DefaulUpdate;
        Vignette = transform.GetChild(0).GetComponent<PostProcessVolume>().profile.GetSetting<Vignette>();

        Player.instance.DeactivatePlayer();
        Player.instance = PlayersScript[StartCapitanIndex];
        Player.instance.ActivatePlayer();
        CameraMoving.target = PlayersScript[0].transform;
    }


    public void CalculateWarriorsPos(int capitanIdx)
    {
        WarriorPositions[capitanIdx] = new List<Vector3>();
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
                WarriorPositions[capitanIdx].Add(warPos);
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
        update();
        Vignette.intensity.Override(Mathf.Lerp(Vignette.intensity, VignetteIntensity, Time.deltaTime * 2));
    }
    private void DefaulUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && PlayersScript.Count > 0)
        {
            Player.instance.DeactivatePlayer();
            Player.instance = PlayersScript[0];
            Player.instance.ActivatePlayer();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && PlayersScript.Count > 1)
        {
            Player.instance.DeactivatePlayer();
            Player.instance = PlayersScript[1];
            Player.instance.ActivatePlayer();
        }
    }
    private void EndGameUpdate()
    {
        Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, new Color(0, 0, 0, 1), Time.deltaTime);
        text.color = Color.Lerp(text.color, Color.red, Time.deltaTime);
        blackImg.color = Color.Lerp(blackImg.color, Color.black, Time.deltaTime);
    }

    public void RemoveCurse(int shamanIndex)
    {
        for (int i = 0; i < WarriorsScript.Count; ++i)
            WarriorsScript[i].RemoveCurse(shamanIndex);

        for (int i = 0; i < PlayersScript.Count; ++i)
            if (PlayersScript[i] != null)
                PlayersScript[i].RemoveCurse(shamanIndex);
    }

    public void PlayerDie()
    {
        bool isAllPlayersDie = true;
        foreach (var player in PlayersScript)
            if (player != null)
            {
                isAllPlayersDie = false;
                Player.instance = player;
                Player.instance.ActivatePlayer();
            }

        if (isAllPlayersDie)
        {
            text = DieText;
            StartCoroutine(DieTimer(false));
            update = EndGameUpdate;
        }
    }

    public void CheckWin()
    {
        if(EnemiesScript.Count == 0)
        {
            bool isAllDie = true;
            foreach (var shaman in ShamansScript)
                if (shaman != null)
                    isAllDie = false;
            if (isAllDie)
            {
                text = WinText;
                StartCoroutine(DieTimer(true));
                update = EndGameUpdate;
            }
        }
    }

    private IEnumerator DieTimer(bool win)
    {
        yield return new WaitForSeconds(5f);
        if (win) Win();
        else Defeat();
    }
    private void Defeat()
    {

    }
    private void Win()
    {

    }
}
