using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameController : MonoBehaviour
{
    public static GameController instance;


    [SerializeField] private Text DieText;
    [SerializeField] private Text WinText;
    private Text text;
    [SerializeField] private UnityEngine.UI.Image blackImg;

    [SerializeField] private int lenStep;

    [SerializeField] private Transform[] WarriorTeams;
    public static List<Vector3>[] WarriorPositions;
    public static List<Warrior> WarriorScripts;

    public Player[] PlayerScripts;
    public int StartCapitanIndex = 0;

    public static List<EntityBehaviour> EnemyScripts;
    public static List<Shaman> ShamanScripts;
    public static List<House> HouseScripts;

    [NonSerialized] public float VignetteIntensity = 0;
    [NonSerialized] public Vignette Vignette;

    private void Awake()
    {//при перезапуске сцены статичные поля не перепичываются, поэтому инициализация сдесь
        instance = this;
        WarriorScripts = new();
        EnemyScripts = new();
        ShamanScripts = new();
        HouseScripts = new();
    }
    private void Start()
    {
        if (WarriorTeams == null) Debug.LogError("Attach each parent of warriors as one element in List \"Warrior Teams\" in GameController");
        if (PlayerScripts == null) Debug.LogError("Attach each Capitan/Player as one element in List \"Warrior Teams\" in GameController");

        //Cursor.visible = false;
        WarriorPositions = new List<Vector3>[WarriorTeams.Length];
        for(int i = 0; i < WarriorTeams.Length; ++i)
            CalculateWarriorsPos(i);

        Vignette = transform.GetChild(0).GetComponent<PostProcessVolume>().profile.GetSetting<Vignette>();
        if (Vignette == null) Debug.LogError("Can't find PostProcessVolume in child 0");

        if(Player.instance != null) 
            Player.instance.DeactivatePlayer();
        Player.instance = PlayerScripts[StartCapitanIndex];
        Player.instance.ActivatePlayer();
        CameraMoving.target = Player.instance.transform;

        if (Vignette == null) Debug.LogError("Can't find PostProcessVolume in child 0");
    }


    public void CalculateWarriorsPos(int capitanIdx)
    {
        WarriorPositions[capitanIdx] = new List<Vector3>();
        Vector3 warPos;
        int stepx, stepz, lim;//lim is (max length of warriors's line)/2

        if (WarriorTeams[capitanIdx].childCount > 24)
            lim = 4;//max is 48 warriors
        else if (WarriorTeams[capitanIdx].childCount > 8)
            lim = 2;
        else
            lim = 1;

        stepx = -lim * lenStep;
        stepz = -lim * lenStep;

        for (int i = 0; i < WarriorTeams[capitanIdx].childCount; i++)
        {
            warPos = new Vector3(stepx, 0, stepz);
            if (warPos != Vector3.zero)
            {
                WarriorPositions[capitanIdx].Add(warPos);
                WarriorTeams[capitanIdx].GetChild(i).GetComponent<Warrior>().index = i;
                WarriorTeams[capitanIdx].GetChild(i).GetComponent<Warrior>().MyCapitanIndex = capitanIdx;
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
        Vignette.intensity.Override(Mathf.Lerp(Vignette.intensity, VignetteIntensity, Time.deltaTime * 2));

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.O))
            Win();
        else if (Input.GetKeyDown(KeyCode.P))
            Defeat();
#endif

        if (Input.GetKeyDown(KeyCode.Alpha1))
            ActivatePlayer(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ActivatePlayer(1);
    }

    private void ActivatePlayer(int playerIndex)
    {
        if (PlayerScripts[playerIndex] == null) return;

        if (Player.instance != null)
            Player.instance.DeactivatePlayer();
        Player.instance = PlayerScripts[playerIndex];
        Player.instance.ActivatePlayer();
    }

    public void RemoveCurse(int shamanIndex)
    {
        foreach (var warrior in WarriorScripts)
            warrior.RemoveCurse(shamanIndex);

        foreach (var player in PlayerScripts)
            if (player != null)
                player.RemoveCurse(shamanIndex);
    }

    public void CheckDefeat()
    {
        bool isAllPlayersDie = true;

        foreach (var player in PlayerScripts)
            if (player != null)
            {
                isAllPlayersDie = false;
                Player.instance = player;
                Player.instance.ActivatePlayer();
            }

        if (isAllPlayersDie)
            Defeat();
    }

    public void CheckWin()
    {
        if(EnemyScripts.Count == 0)
        {
            bool isAllDie = true;
            foreach (var shaman in ShamanScripts)
                if (shaman != null)
                    isAllDie = false;

            if (isAllDie)
                Win();
        }
    }

    private IEnumerator EndGameThings()
    {
        while (blackImg.color != Color.black)
        {
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, new Color(0, 0, 0, 1), Time.deltaTime);
            text.color = Color.Lerp(text.color, Color.red, Time.deltaTime);
            blackImg.color = Color.Lerp(blackImg.color, Color.black, Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        SceneManager.LoadScene("Menu");
    }
    private void Defeat()
    {
        text = DieText;
        StartCoroutine(EndGameThings());
        PlayerPrefs.SetInt("isWin", 0);
    }
    private void Win()
    {
        text = WinText;
        StartCoroutine(EndGameThings());
        PlayerPrefs.SetInt("isWin", 1);
    }
}
