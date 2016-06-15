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
            while (reader.Read()) {
                if (reader.IsStartElement() && reader.Name.Equals("Model")) {
                    reader.Read();
                    Debug.Log(reader.Value.Trim());
                    var tmp = Instantiate(PatchPrefab) as GameObject;
                    tmp.GetComponent<PatchCreator>().ModelName = reader.Value.Trim();
                    tmp.GetComponent<PatchCreator>().CreatePatches();
                    _allModels.Add(tmp);
                }
            }
        }
        _allModels.Add(new GameObject());
        _allModels.Add(new GameObject());
        _allModels.Add(new GameObject());
        GetComponent<ModelScrollLayout>().InitLayout(_allModels);
    }
}
