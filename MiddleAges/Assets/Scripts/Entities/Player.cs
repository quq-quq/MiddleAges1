using UnityEngine;

public class Player : EntityBehaviour
{ 
    public static Transform plTransform;
    public static Vector3 MovementVector = Vector3.zero;
    public static Quaternion MovingAngle = Quaternion.identity;//поможет вычислить место война, учитывая поворот игрока

    private void Awake()
    {
        plTransform = transform;
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void FixedUpdate()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            MovementVector = plTransform.TransformDirection(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));
            MovingAngle = Quaternion.Euler(0, Mathf.Atan2(MovementVector.x, MovementVector.z) * Mathf.Rad2Deg, 0);
            controller.Move(MovementVector * speed * Time.deltaTime);
        }
    }
    //public void OnGUI()//debug окно в игре
    //{
    //    GUI.Label(
    //       new Rect(
    //           Screen.width - 205,                   // x, left offset
    //           40,                  // y, bottom offset
    //           200f,                // width
    //           60f                 // height
    //       ),
    //       plTransform.position + "  " + plTransform.forward +"\n" +
    //       MovementVector.normalized,             // the display text
    //       GUI.skin.textArea        // use a multi-line text area
    //    );
    //}
}
