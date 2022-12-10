using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehaviour : MonoBehaviour
{
    protected static float courotineTime = 0.5f;// интервал между обновлением существ за которыми идем/которых бьем

    [System.NonSerialized] public Transform myTransform;
    protected CharacterController controller;
    protected Animator anim;

    protected AttackScript weapon;

    public int hpDefault;
    protected int hpCurrent;

    // если надо плавно менять скорость то менять надо значение targetSpee
    [System.NonSerialized] public Vector3 movementVector = Vector3.zero;
    public float speedDefault = 12f;
    protected float speedCurrent;
    protected float speedTarget;

    protected float startScale;

    protected void BaseStart()
    {
        hpCurrent = hpDefault;
        speedCurrent = speedTarget = speedDefault;
        myTransform = transform;

        controller = myTransform.GetComponent<CharacterController>();

        weapon = myTransform.GetChild(0).GetComponent<AttackScript>();
        anim = myTransform.GetComponent<Animator>();
        startScale = myTransform.localScale.x;
    }

    private float x, z, angleToLookAtCamera;
    protected void BaseUpdate()
    {
        x = CameraMoving.CamTransform.position.x - myTransform.position.x;
        z = CameraMoving.CamTransform.position.z - myTransform.position.z;
        angleToLookAtCamera = (Mathf.Rad2Deg * Mathf.Atan2(x, z) + 360) % 360;
        myTransform.eulerAngles = new Vector3(0, angleToLookAtCamera + 180, 0); //смотрим спиной на камеру
    }
    protected void BaseFixedUpdate()
    {
        movementVector.y = -10f;
        speedCurrent = Mathf.Lerp(speedCurrent, speedTarget, Time.fixedDeltaTime*5);//плавно меняем скорость
        controller.Move(movementVector * speedCurrent * Time.fixedDeltaTime);
    }

    public void SetDamage(int damage)
    {
        if (hpCurrent - damage > 0) hpCurrent -= damage;
        else
        {
            hpCurrent = 0;
            Die();
        }
    }
    public virtual void Die()
    {

        print("Some one Die");
        //пытаемся вызвать метод Die
        myTransform.GetComponent<Player>()?.Die();
        myTransform.GetComponent<Warrior>()?.Die();
        myTransform.GetComponent<Enemy>()?.Die();
        myTransform.GetComponent<Shaman>()?.Die();
        myTransform.GetComponent<House>()?.Die();
    }

    public void PutDamage()
    {
        weapon.PutDamage();
    }

}