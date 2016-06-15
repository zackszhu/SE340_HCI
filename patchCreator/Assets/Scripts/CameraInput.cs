using UnityEngine;
using System.Collections;

public class CameraInput : MonoBehaviour {
    #region PRIVATE_VARIABLES
    private float _distance;
    private float _fov;
    private Camera _camera;
    #endregion

    #region PUBLIC_VARIABLES
    public float ZoomSpeed;
    public GameObject Model;
    #endregion

    public void MyAwake() {
        _camera = GetComponent<Camera>();
        StartCoroutine(MyUpdate());
    }

    IEnumerator MyUpdate() {
        while (true) {
            SetZoom();
            yield return new WaitForEndOfFrame();
        }
    }

    private void SetZoom() {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
            // _camera.fieldOfView = _camera.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
            Model.GetComponent<ModelScrollLayout>().Selected.transform.position -= new Vector3(0, 0, ZoomSpeed) * Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"));
            //Model.position -= new Vector3(0, 0, ZoomSpeed);
        }
    }
}
