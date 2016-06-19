using UnityEngine;
using System.Collections;
[RequireComponent(typeof(MeshCollider))]
public class MouseDrag : MonoBehaviour {

    private Vector3 screenPoint;
    private Vector3 offset;
    private Vector3 currPos;


    void Update() {
        if (Input.GetMouseButtonDown(1)) {
            screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

            offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

            Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
            currPos = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;

        }
        if (Input.GetMouseButton(1)) {
            MouseRotation();
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
            // _camera.fieldOfView = _camera.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
            transform.localScale += new Vector3(0.2f, 0.2f, 0.2f) * Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"));
            //Model.position -= new Vector3(0, 0, ZoomSpeed);
        }

    }

    private void MouseRotation() {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        Debug.Log(Vector3.Angle(currPos, curPosition));
        transform.RotateAround(transform.position, Vector3.forward, -Mathf.Sign(Vector3.Cross(currPos, curPosition).z) * Vector3.Angle(currPos, curPosition));
        currPos = curPosition;
    }

    void OnMouseDown() {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));

    }

    void OnMouseDrag() {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;

    }
}
