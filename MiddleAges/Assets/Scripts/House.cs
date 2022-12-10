using UnityEngine;

public class House : EntityBehaviour
{
    void Start()
    {
        hpCurrent = hpDefault;
        GameController.HousesScript.Add(this);
    }
    public override void Die()
    {
        GameController.HousesScript.Remove(this);
        gameObject.SetActive(false);
        Destroy(gameObject, courotineTime + 0.1f);
    }
}
