using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModelScrollLayout : MonoBehaviour {
    public float AnimatedTime;
    public GameObject Selected;

    private Vector3[] _positions;
    private Vector3 _somewhereLeft;
    private Vector3 _somewhereRight;
    private int _cursor;
    private List<GameObject> _models;
    private bool _selected;

	// Use this for initialization
	void Awake () {
	    _selected = false;
	    _cursor = 0;
	    _positions = new Vector3[7] {
            new Vector3(-2500, 0, 2500), 
	        new Vector3(-1300, 0, 2000),
            new Vector3(-650, 0, 1500),
            new Vector3(0, 0, 900),
            new Vector3(650, 0, 1500),
            new Vector3(1300, 0, 2000),
            new Vector3(2500, 0, 2500)
        };
        _somewhereLeft = new Vector3(-3000, 0, 2500);
        _somewhereRight = new Vector3(3000, 0, 2500);
        _models = new List<GameObject>();
	}

    public GameObject GetSelectModel() {
        return _models[_cursor + 3];
    }


    public void InitLayout(List<GameObject> models) {
        foreach (var model in _models) {
            Destroy(model);
        }
        _models = models;
        _cursor = 0;
        HandleLayout();
    }

    public void InitSubLayout(List<GameObject> models) {
        foreach (var model in _models) {
            model.transform.position = _somewhereLeft;
        }
        _models = models;
        _cursor = 0;
        HandleLayout();
    }

    private void HandleLayout(bool animated = false) {
        if (_models.Count <= 6) {
            return;
        }
        _models[_cursor + 3].GetComponent<RotationInput>().enabled = true;
        Selected = _models[_cursor + 3];
        for (int i = 0; i < _models.Count; i++) {
            if (i < _cursor) {
                _models[i].transform.position = _somewhereLeft;
            }
            if (i >= _cursor + 7) {
                _models[i].transform.position = _somewhereRight;
            }
        }
        for (int i = 0; i < 7; i++) {
            if (!animated) {
                _models[i + _cursor].transform.position = _positions[i];
            }
            else {
                StartCoroutine(ModelMoveTo(_models[i + _cursor], _positions[i], AnimatedTime));
            }
        }
    }

    IEnumerator ModelMoveTo(GameObject model, Vector3 dest, float time) {
        var _timeStartLerping = Time.time;
        var position = model.transform.position;
        while (true) {
            var rate = (Time.time - _timeStartLerping) / time;
            model.transform.position = Vector3.Lerp(position, dest, rate);
            if (rate > 1) {
                yield break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void ModelMoveOut() {
        for (int i = 0; i < _models.Count; i++) {
            if (i < _cursor + 3) {
                StartCoroutine(ModelMoveTo(_models[i], _somewhereLeft, AnimatedTime));
            }
            if (i > _cursor + 3) {
                StartCoroutine(ModelMoveTo(_models[i], _somewhereRight, AnimatedTime));
            }
        }
        _selected = true;
    }

    public void MyAwake() {
        StartCoroutine(MyUpdate());
    }

    private void HandleScroll(int scrollNum) {
        _models[_cursor + 3].GetComponent<RotationInput>().enabled = false;
        _cursor = Mathf.Clamp(_cursor + scrollNum, 0, _models.Count - 7);
        HandleLayout(true);
    }

    IEnumerator MyUpdate() {
        while (!_selected) {
            if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
                HandleScroll(-Mathf.RoundToInt(Mathf.Sign(Input.GetAxis("Mouse ScrollWheel"))));
            }
            yield return new WaitForEndOfFrame();
        }
    }

    public void LoadMore() {
        GetComponent<ModelGroupCreation>().GetSubModels(_models[_cursor + 3].GetComponent<PatchCreator>().ModelName);
    }

    public void LoadBack() {
        InitLayout(GetComponent<ModelGroupCreation>()._allModels);
    }
}
