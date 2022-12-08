using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LehaSaveManager : MonoBehaviour
{
    [SerializeField] int[] savedFloors;
    [SerializeField] int lastFloor;

    void Awake()
    {
        lastFloor = PlayerPrefs.GetInt("lastFloor");
    }
    // void Start()
    // {
    //     lastFloor = PlayerPrefs.GetInt("lastFloor");
    // }

    public void SaveIntFloor()
    {
        PlayerPrefs.SetInt("lastFloor", lastFloor);
    }
    public int GetLastFloor()
    {
        return lastFloor;
    }
    public void SaveFloor(int currentFloor)
    {
        foreach (int savedFloor in savedFloors)
        {
            if(savedFloor == currentFloor && lastFloor < currentFloor)
            {
                lastFloor = currentFloor;
                SaveIntFloor();
            }
        }
    }
    public void ClearSaves()
    {
        lastFloor = 0;
        SaveIntFloor();
    }
    
}
