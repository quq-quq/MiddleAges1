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
    void Start()
    {
        saveManager = GetComponent<LehaSaveManager>();
        currentFloor = saveManager.GetLastFloor();
        buttons = GetComponentsInChildren<Button>();
        // foreach (Button btn in GetComponentsInChildren<Button>())
        // {
            
        // }
        foreach (var btn in buttons)
        {
            btn.onClick.AddListener(FloorUp); 
        }
        UpdateButtons();
        
    }
    void FloorUp() //повышение этажа при нажатии кнопки
    {
        currentFloor++;
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
