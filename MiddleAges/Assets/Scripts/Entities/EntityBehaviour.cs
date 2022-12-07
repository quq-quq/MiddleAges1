using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehaviour : MonoBehaviour
{
    protected CharacterController controller;
    public Transform myTransform;

    // если надо плавно мен€ть скорость то мен€ть надо значение targetSpeed
    protected float speed;
    [SerializeField] private float targetSpeed;

    protected virtual void Start()
    {
        myTransform = transform;//при вызове transform unity будет искать прикрепленный к Gќ обьект что займет больше времени чем доступ по ссылке
        controller = myTransform.GetComponent<CharacterController>();
    }

    private float x, z, angleToLookAtCamera;
    protected virtual void Update()
    {
        x = CameraMoving.CamTransform.position.x - myTransform.position.x;
        z = CameraMoving.CamTransform.position.z - myTransform.position.z;
        angleToLookAtCamera = (Mathf.Rad2Deg * Mathf.Atan2(x, z) + 360) % 360;
        myTransform.eulerAngles = new Vector3(0, angleToLookAtCamera + 180, 0); //смотрим спиной на камеру

        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime);//плавно мен€ем скорость

        controller.Move(Vector3.down * 20 * Time.deltaTime);//гравитаци€
    }

    public void SetSlowlingCurse(float Slowling)
    {
        if (targetSpeed - Slowling > 0) targetSpeed -= Slowling;
        else targetSpeed = 0;
    }
    public void SetSpeed(float speed)
    {
        targetSpeed = speed;
    }
}