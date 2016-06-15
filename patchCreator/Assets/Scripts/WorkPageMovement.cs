using UnityEngine;
using System.Collections;

public class WorkPageMovement : MonoBehaviour {
    public float MoveDuration;

    public void MoveIn() {
        StartCoroutine(MoveInsideCoroutine());
    }

    IEnumerator MoveInsideCoroutine() {
        GetComponent<RectTransform>().sizeDelta = new Vector2(960, 1080);
        var _startTime = Time.time;
        var size = GetComponent<RectTransform>().sizeDelta;
        while (true) {
            var rate = (Time.time - _startTime) / MoveDuration;
            GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(size, new Vector2(960, 540), rate);
            if (rate >= 1) {
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }


    public void MoveOut() {
        StartCoroutine(MoveOutsideCoroutine());
    }

    IEnumerator MoveOutsideCoroutine() {
        var _startTime = Time.time;
        var scale = transform.localScale;
        var destScale = scale * 2;
        destScale.x = scale.x;
        while (true) {
            var rate = (Time.time - _startTime) / MoveDuration;
            transform.localScale = Vector3.Lerp(scale, destScale, rate);
            if (rate >= 1) {
                StartCoroutine(MoveBack());
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator MoveBack() {
        var _startTime = Time.time;
        var position = transform.position;
        var dest = new Vector3(0, 0, -100);
        while (true) {
            var rate = (Time.time - _startTime) / MoveDuration;
            transform.position = Vector3.Lerp(position, dest, rate);
            if (rate >= 1) {
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }

    }
}
