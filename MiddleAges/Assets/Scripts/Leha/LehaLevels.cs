using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LehaLevels : MonoBehaviour
{
    [SerializeField] int floor;// "этаж" уровня, можно выбрать только один уровень на одном этаже
    //[SerializeField] int level;
    [SerializeField] private string sceneName;

    Button btn;
    LehaButtonManager btnManager;

    void Start () 
    {
        btnManager = GetComponentInParent<LehaButtonManager>();
		btn = GetComponent<Button>();
        btn.onClick.AddListener(ClickButton);
        CheckFloor();
	}
	void ClickButton()
    {
        if(CheckFloor())
		    Debug.Log ("You have clicked the button! " + btn.name);
            LoadLevel();
            
	}
    public bool CheckFloor()
    {
        if(btnManager.currentFloor != floor)
        {
            SetInteract();
            return false;
        }
        else
        {
            SetInteract(true);
            return true;
        }
    }
    void SetInteract(bool val = false)
    {
        btn.interactable = val;
    }
    

    private void LoadLevel()
    {
        SceneManager.LoadScene(sceneName);
    }
}
