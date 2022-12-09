using UnityEngine;

public class EntityBehaviour : MonoBehaviour
{
    protected CharacterController controller;
    public Transform myTransform;
    protected Animator anim;

    // если надо плавно мен€ть скорость то мен€ть надо значение targetSpeed
    public float defaultSpeed = 12f;
    protected float currentSpeed;
    protected float targetSpeed;
    protected float startScale;

    protected virtual void Start()
    {
        currentSpeed = targetSpeed = defaultSpeed;
        myTransform = transform;//при вызове transform unity будет искать прикрепленный к Gќ обьект что займет больше времени чем доступ по ссылке
        controller = myTransform.GetComponent<CharacterController>();
        anim = myTransform.GetComponent<Animator>();
        startScale = myTransform.localScale.x;
    }

    private float x, z, angleToLookAtCamera;
    protected virtual void Update()
    {
        x = CameraMoving.CamTransform.position.x - myTransform.position.x;
        z = CameraMoving.CamTransform.position.z - myTransform.position.z;
        angleToLookAtCamera = (Mathf.Rad2Deg * Mathf.Atan2(x, z) + 360) % 360;
        myTransform.eulerAngles = new Vector3(0, angleToLookAtCamera + 180, 0); //смотрим спиной на камеру

        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime*2f);//плавно мен€ем скорость

        controller.Move(Vector3.down * 20 * Time.deltaTime);//гравитаци€
    }

    public void SetDefaultSpeed() => targetSpeed = currentSpeed;
}