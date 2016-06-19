using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class ModelGroupCreation : MonoBehaviour {
    public GameObject PatchPrefab;

    public List<GameObject> _allModels;
    
    void Awake() {
        _allModels = new List<GameObject>();
        _allModels.Add(new GameObject());
        _allModels.Add(new GameObject());
        _allModels.Add(new GameObject());
        using (XmlReader reader = XmlReader.Create(Application.dataPath + "/Data/models.xml")) {
            while (reader.ReadToFollowing("Model")) {
                if (reader.HasAttributes) continue;
                var tmp = Instantiate(PatchPrefab) as GameObject;
                var name = reader.ReadInnerXml().Trim();
                tmp.GetComponent<PatchCreator>().LoadModelByModelName(name);
                tmp.GetComponent<PatchCreator>().CreatePatches();
                _allModels.Add(tmp);
            }
        }
        _allModels.Add(new GameObject());
        _allModels.Add(new GameObject());
        _allModels.Add(new GameObject());
        GetComponent<ModelScrollLayout>().InitLayout(_allModels);
    }

    public void GetSubModels(string fatherName) {
        var ret = new List<GameObject>();
        ret.Add(new GameObject());
        ret.Add(new GameObject());
        ret.Add(new GameObject());
        using (XmlReader reader = XmlReader.Create(Application.dataPath + "/Data/models.xml")) {
            while (reader.ReadToFollowing("Model")) {
                if (reader.HasAttributes && reader.GetAttribute("father").Equals(fatherName)) {
                    var tmp = Instantiate(PatchPrefab) as GameObject;
                    var name = reader.ReadInnerXml().Trim();
                    tmp.GetComponent<PatchCreator>().LoadModelByModelName(name);
                    tmp.GetComponent<PatchCreator>().CreatePatches();
                    ret.Add(tmp);
                }
            }
        }
        ret.Add(new GameObject());
        ret.Add(new GameObject());
        ret.Add(new GameObject());
        GetComponent<ModelScrollLayout>().InitSubLayout(ret);
    }
}
