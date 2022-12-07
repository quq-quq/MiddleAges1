using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehaviour : MonoBehaviour
{
    protected CharacterController controller;
    public Transform myTransform;

    // ���� ���� ������ ������ �������� �� ������ ���� �������� targetSpeed
    public float defaultSpeed = 12f;
    protected float currentSpeed;
    protected float targetSpeed;

    protected virtual void Start()
    {
        currentSpeed = targetSpeed = defaultSpeed;
        myTransform = transform;//��� ������ transform unity ����� ������ ������������� � G� ������ ��� ������ ������ ������� ��� ������ �� ������
        controller = myTransform.GetComponent<CharacterController>();
    }

    private float x, z, angleToLookAtCamera;
    protected virtual void Update()
    {
        x = CameraMoving.CamTransform.position.x - myTransform.position.x;
        z = CameraMoving.CamTransform.position.z - myTransform.position.z;
        angleToLookAtCamera = (Mathf.Rad2Deg * Mathf.Atan2(x, z) + 360) % 360;
        myTransform.eulerAngles = new Vector3(0, angleToLookAtCamera + 180, 0); //������� ������ �� ������

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime);//������ ������ ��������

        controller.Move(Vector3.down * 20 * Time.deltaTime);//����������
    }

    public void SetDefaultSpeed() => targetSpeed = currentSpeed;
}