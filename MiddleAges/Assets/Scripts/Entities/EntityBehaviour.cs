using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityBehaviour : MonoBehaviour
{
    protected CharacterController controller;
    [System.NonSerialized] public Transform myTransform;

    protected float damageDefault = 12f;
    protected float damageCurrent;
    public int hp;

    // ���� ���� ������ ������ �������� �� ������ ���� �������� targetSpeed
    public float speedDefault = 12f;
    protected float speedCurrent;
    protected float speedTarget;

    protected virtual void Start()
    {
        speedCurrent = speedTarget = speedDefault;
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

        speedCurrent = Mathf.Lerp(speedCurrent, speedTarget, Time.deltaTime);//������ ������ ��������

    }
    protected virtual void FixedUpdate()
    {
        controller.Move(Vector3.down * 10f * Time.fixedDeltaTime);//����������
    }

    public void TakeDamage(int damage)
    {

    }
}