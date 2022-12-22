using UnityEngine;

public class House : EntityBehaviour
{
    void Start()
    {
        hpCurrent = hpDefault;
        GameController.HouseScripts.Add(this);
    }
    public override void Die()
    {
        GameController.HouseScripts.Remove(this);
        gameObject.SetActive(false);
        Destroy(gameObject, courotineTime + 0.1f);
    }
}
