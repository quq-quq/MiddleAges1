using UnityEngine;

public class House : MonoBehaviour
{
    void Awake()
    {
        GameController.HousesScripts.Add(this);
    }

}
