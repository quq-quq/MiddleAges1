using UnityEngine;

public class CameraMoving : MonoBehaviour 
{

	[SerializeField] private Transform target;
	[SerializeField] private Vector3 offset;
	[SerializeField] private float zAngleToLookAtPlayer = 30;//угол под которым камера смотрит на игрока по оси x
	[SerializeField] private float RotateSensitivity = 3;   // чувствительность поворота камеры
	[SerializeField] private float limit = 80;              // ограничение вращения по Y
	[SerializeField] private float zoomSensivity = 0.25f;   // чувствительность при увеличении колесиком мышки
	[SerializeField] private float zoomMax = 10;            // макс. увеличение
	[SerializeField] private float zoomMin = 3;             // мин. увеличение

	public static Transform CamTransform;

	void Start () 
	{
		CamTransform = transform;
		limit = Mathf.Abs(limit);
		if(limit > 90) limit = 90;

		offset = new Vector3(offset.x, offset.y, -Mathf.Abs(zoomMax)/2);
		CamTransform.position = target.position + offset;
	}

	private float X;
	void Update ()
	{
		if(Input.GetAxis("Mouse ScrollWheel") > 0) offset.z += zoomSensivity;
		else if(Input.GetAxis("Mouse ScrollWheel") < 0) offset.z -= zoomSensivity;
		offset.z = Mathf.Clamp(offset.z, -Mathf.Abs(zoomMax), -Mathf.Abs(zoomMin));

        if (Input.GetMouseButton(0))
        {
			if(Input.GetAxis("Mouse Y")!=0 ) offset.y += Input.GetAxis("Mouse Y");
			offset.y = Mathf.Clamp(offset.y, -limit, limit);
		}
        else
        {
			X = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * RotateSensitivity;
			CamTransform.localEulerAngles = new Vector3(zAngleToLookAtPlayer, X, 0);
		}
		CamTransform.position = transform.localRotation * offset + target.position;
	}
}