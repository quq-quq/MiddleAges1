using UnityEngine;

public class House : MonoBehaviour
{
    void Start()
    {
        GameController.instance.HousesScripts.Add(this);
    }

}
