using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	public float offset;
	public float height;
    public float angle;

	public Transform focus;

	private Vector3 position;

	private Quaternion rotation;

	void LateUpdate() {

        rotation.SetLookRotation(focus.position - transform.position, Vector3.up);
        transform.rotation = rotation;

        position = focus.position;
        position.x += offset * -Mathf.Sin(focus.eulerAngles.y / 360f * 2 * Mathf.PI);
        position.y = height;
        position.z += offset * -Mathf.Cos(focus.eulerAngles.y / 360f * 2 * Mathf.PI);

        transform.position = position;
	}
}
