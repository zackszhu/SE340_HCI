using UnityEngine;
using System.Collections;

public class RotationInput : MonoBehaviour {
    #region PRIVATE_VARIABLE
    private Vector3 m_TargetAngles;
    private Vector3 m_FollowAngles;
    private Vector3 m_FollowVelocity;
    private Quaternion m_OriginalRotation;
    #endregion

    #region PUBLIC_VARIABLE
    public Vector2 rotationRange;
    public float rotationSpeed;
    public float dampingTime;
    #endregion

    // Use this for initialization
    void Awake() {
        m_OriginalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButton(1)) {
            MouseRotate();
        }
    }

    private void MouseRotate() {
        transform.localRotation = m_OriginalRotation;
        float inputH = Input.GetAxis("Mouse X"), inputV = Input.GetAxis("Mouse Y");

        Adjust();

        // with mouse input, we have direct control with no springback required.
        m_TargetAngles.y += inputH * rotationSpeed;
        m_TargetAngles.x += inputV * rotationSpeed;

        // clamp values to allowed range
        m_TargetAngles.y = Mathf.Clamp(m_TargetAngles.y, -rotationRange.y * 0.5f, rotationRange.y * 0.5f);
        m_TargetAngles.x = Mathf.Clamp(m_TargetAngles.x, -rotationRange.x * 0.5f, rotationRange.x * 0.5f);

        // smoothly interpolate current values to target angles
        m_FollowAngles = Vector3.SmoothDamp(m_FollowAngles, m_TargetAngles, ref m_FollowVelocity, dampingTime);

        // update the actual gameobject's rotation
        transform.localRotation = m_OriginalRotation * Quaternion.Euler(m_FollowAngles.x, 0, -m_FollowAngles.y);
    }

    private void Adjust() {
        // wrap values to avoid springing quickly the wrong way from positive to negative
        if (m_TargetAngles.y > 180) {
            m_TargetAngles.y -= 360;
            m_FollowAngles.y -= 360;
        }
        if (m_TargetAngles.x > 180) {
            m_TargetAngles.x -= 360;
            m_FollowAngles.x -= 360;
        }
        if (m_TargetAngles.y < -180) {
            m_TargetAngles.y += 360;
            m_FollowAngles.y += 360;
        }
        if (m_TargetAngles.x < -180) {
            m_TargetAngles.x += 360;
            m_FollowAngles.x += 360;
        }
    }
}
