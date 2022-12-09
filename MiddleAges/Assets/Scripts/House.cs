using UnityEngine;

public class House : MonoBehaviour
{
    void Start()
    {
        GameController.HousesScripts.Add(this);
    }

}
