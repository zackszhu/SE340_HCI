using UnityEngine;
using System.Collections;

public class CameraInput : MonoBehaviour {
    #region PRIVATE_VARIABLES
    private float _distance;
    private float _fov;
    private Camera _camera;
    #endregion

    #region PUBLIC_VARIABLES
    public float MoveSpeed;
    public Transform TargetGameObject;

    public float ZoomSpeed;
    #endregion

    // Use this for initialization
    void Awake() {
        #region PRIVATE_INIT
        _distance = 10f;
        _fov = 60f;
        _camera = GetComponent<Camera>();
        #endregion
        transform.position = new Vector3(0, _distance, 0);
        transform.LookAt(TargetGameObject);
    }

    // Update is called once per frame
    void Update() {
        SetZoom();
    }

    private void SetZoom() {
        if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
            _camera.fieldOfView = _camera.fieldOfView - Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
        }
    }
}
