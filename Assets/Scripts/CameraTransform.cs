using UnityEngine;
using System.Collections;

public class CameraTransform : MonoBehaviour {
	
	private Vector3 target;
	private float x;
	private float y;
	private float distance;
	
	// Use this for initialization
	private void Start () {
		
		target = GameObject.Find("Arena").transform.position;
		distance = -20.0F;
		
		x = transform.eulerAngles.y;
		y = transform.eulerAngles.x;
		
		transform.rotation = Quaternion.Euler(y, x, 0);
        transform.position = Quaternion.Euler(y, x, 0) * new Vector3(0.0F, 0.0F, distance) + target;
	}
	
	private void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) Application.LoadLevel( "MainMenu" );
	}
	
	// Update is called once per frame
	private void LateUpdate () {
	
		if (Input.GetMouseButton(1)) {
			x += Input.GetAxis("Mouse X") * 5.0F;
			y -= Input.GetAxis("Mouse Y") * 3.0F;
			
			y = Mathf.Clamp(y, 25, 60);
			x = Mathf.Clamp(x, -60, 60);

        	transform.rotation = Quaternion.Euler(y, x, 0);
        	transform.position = Quaternion.Euler(y, x, 0) * new Vector3(0.0F, 0.0F, distance) + target;
		}
		
		if (Input.GetMouseButton(2)) {

			target -= transform.right * Input.GetAxis("Mouse X") * 0.5F;
			target -= Vector3.Project(transform.forward, new Vector3(transform.forward.x, 0.0F, transform.forward.z)) * Input.GetAxis("Mouse Y") * 0.5F;
			
			target.x = Mathf.Clamp(target.x, 0.0F, 10.0F);
			target.z = Mathf.Clamp(target.z, 0.0F, 10.0F);
			
			transform.position = Quaternion.Euler(y, x, 0) * new Vector3(0.0F, 0.0F, distance) + target;
		}
		
		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			if (distance >= -10.0F) return;
			distance += 1.0F;
			transform.position = Quaternion.Euler(y, x, 0) * new Vector3(0.0F, 0.0F, distance) + target;
		}
		
		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			if (distance <= -20.0F) return;
			distance -= 1.0F;
			transform.position = Quaternion.Euler(y, x, 0) * new Vector3(0.0F, 0.0F, distance) + target;
		}
		
	}
	
}
