using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LehaSaveManager : MonoBehaviour
{
    [SerializeField] int[] checkPoints;
    [SerializeField] int lastCheckPoint;
    [SerializeField] int lastCurrentFloor;

    void Awake()
    {
        lastCheckPoint = PlayerPrefs.GetInt("lastCheckPoint");
        lastCurrentFloor = PlayerPrefs.GetInt("lastCurrentFloor");
    }

    private void SaveCheckPoint()
    {
        PlayerPrefs.SetInt("lastCheckPoint", lastCheckPoint);
    }
    private void SaveLastCurrentFloor()
    {
        Debug.Log("Set" + lastCurrentFloor);
        // LehaSaveStatic.lastCurrentFloor = lastCurrentFloor;
        PlayerPrefs.SetInt("lastCurrentFloor", lastCurrentFloor);
    }
    public int GetLastCheckPoint()
    {
        return lastCheckPoint;
    }
    public int GetLastCurrentFloor()
    {
        Debug.Log("Get " + lastCurrentFloor);
        return lastCurrentFloor;
    }
    public void SaveFloor(int currentFloor)
    {
        foreach (int checkPoint in checkPoints)
        {
            if(checkPoint == currentFloor && lastCheckPoint < currentFloor)
            {
                lastCheckPoint = currentFloor;
                SaveCheckPoint();
            }
            lastCurrentFloor = currentFloor;
            SaveLastCurrentFloor();
        }
    }
    public void ClearSaves()
    {
        lastCheckPoint = 0;
        SaveCheckPoint();
        lastCurrentFloor = 0;
        SaveLastCurrentFloor();
        PlayerPrefs.SetInt("isWin", 0);
    }
    
}
