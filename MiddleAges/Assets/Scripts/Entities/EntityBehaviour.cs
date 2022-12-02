using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehaviour : MonoBehaviour
{
    protected CharacterController controller;

    // если надо плавно менять скорость то менять надо значение targetSpeed
    protected float speed;
    [SerializeField] private float targetSpeed;

    protected virtual void Start()
    {
        controller = transform.GetComponent<CharacterController>();
    }

    private float x, z, angleToLookAtCamera;
    protected virtual void Update()
    {
        x = CameraMoving.CamTransform.position.x - transform.position.x;
        z = CameraMoving.CamTransform.position.z - transform.position.z;
        angleToLookAtCamera = (Mathf.Rad2Deg * Mathf.Atan2(x, z) + 360) % 360;
        transform.eulerAngles = new Vector3(0, angleToLookAtCamera + 180, 0); //смотрим спиной на камеру

        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime);//плавно меняем скорость


        controller.Move(Vector3.down*2 * speed * Time.deltaTime);//гравитация
    }

    public void SetCurse()
    {
        targetSpeed = 0;
    }
}