using UnityEngine;

public class House : MonoBehaviour
{
    void Start()
    {
        GameController.HousesScript.Add(this);
    }

}
