using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColorPick : MonoBehaviour {
    public float MoveDuration;
    public GameObject ColorPicker;

    public void ScaleUp() {
        StartCoroutine(Scaling(ColorPicker.GetComponent<RectTransform>().sizeDelta, true));
    }

    public void ScaleDown() {
        gameObject.SetActive(true);
        ColorPicker.SetActive(false);
        StartCoroutine(Scaling(new Vector2(80f, 80f)));
    }

    IEnumerator Scaling(Vector2 s, bool up = false) {
        var _startTime = Time.time;
        var size = GetComponent<RectTransform>().sizeDelta;
        while (true) {
            var rate = (Time.time - _startTime) / MoveDuration;
            GetComponent<RectTransform>().sizeDelta = Vector2.Lerp(size, s, rate);
            if (rate >= 1) {
                if (up) {
                    gameObject.SetActive(false);
                    ColorPicker.SetActive(true);
                }
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

}
