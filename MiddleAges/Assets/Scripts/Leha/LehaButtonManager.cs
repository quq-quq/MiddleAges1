using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class LehaButtonManager : MonoBehaviour
{
    [SerializeField] public int currentFloor;
    LehaSaveManager saveManager;
    Button[] buttons;
    int isWin = 0;
    void Start()
    {
        Cursor.visible = true;
        saveManager = GetComponent<LehaSaveManager>();
        isWin = PlayerPrefs.GetInt("isWin");

        if(isWin == 0)//defeat
        {
            int lastChP = saveManager.GetLastCheckPoint();
            currentFloor = lastChP == 0 ? lastChP : lastChP - 1;
        }
        if(isWin == 1)//win
        {
            currentFloor = saveManager.GetLastCurrentFloor();
            Debug.Log("win " + currentFloor);
            Debug.Log(PlayerPrefs.GetInt("lastCurrentFloor"));
        }

        buttons = GetComponentsInChildren<Button>();
        foreach (var btn in buttons)
        {
            btn.onClick.AddListener(FloorUp);
            
        }
        UpdateButtons();
        
    }
    void FloorUp() //повышение этажа при нажатии кнопки
    {
        currentFloor++;
        Debug.Log("FloorUp " + currentFloor);
        saveManager.SaveFloor(currentFloor);
        UpdateButtons();
    }
    void UpdateButtons()
    {
        foreach (var btn in buttons)
        {
            btn.gameObject.GetComponent<LehaLevels>().CheckFloor();
            //Debug.Log( + btn.name);
            //Debug.Log("HMM");
        }
    }


    public void ReloadScene()//просто для теста, потом можно удалить
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //UpdateButtons();
    }
    public void ReloadButton()//просто для теста, потом можно удалить
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UpdateButtons();
    }

}
