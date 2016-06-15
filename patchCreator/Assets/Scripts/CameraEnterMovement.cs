using UnityEngine;
using System.Collections;

public class CameraEnterMovement : MonoBehaviour {
    public GameObject TitlePage;
    public Vector3 EndPosition;
    public Vector3 EndRotation;
    public float TransitionDuration;

    private float _timeStartLerping;

    public void EnterApp() {
        StartCoroutine(Entering());

    }

    IEnumerator Entering() {
        _timeStartLerping = Time.time;
        var position = transform.position;
        var rotation = TitlePage.transform.rotation;
        while (true) {
            var rate = (Time.time - _timeStartLerping)/TransitionDuration;
            transform.position = Vector3.Lerp(position, EndPosition, rate);
            TitlePage.transform.rotation = Quaternion.Lerp(rotation, Quaternion.Euler(EndRotation), rate);
            if (rate > 1) {
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}
