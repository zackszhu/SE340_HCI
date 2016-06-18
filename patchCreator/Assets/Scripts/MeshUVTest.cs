using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Assets.Scripts;
using UnityEditor;



public class MeshUVTest : MonoBehaviour {

    // Use this for initialization
    public GameObject Patches;


    private bool _test = true;
    void Start () {
        /*
        List<Vector3> vertices = new List<Vector3>() {
            new Vector3(0, 0, 0), 
            new Vector3(-0.5f, 0, 0), 
            new Vector3(-0.5f, 0, 0.5f), 
        };
        SetMesh(vertices, true);
        */



    }


	
	// Update is called once per frame
	void Update () {
	    if (_test) {
            // Patches.GetComponent<PatchCreator>().LoadModelByModelName("fengche");
            // Patches.GetComponent<PatchCreator>().CreatePatches();
            // Patches.GetComponent<PatchCreator>().SaveModelByModelName("fengche");
            Patches.GetComponent<PatchCreator>().LoadModelByModelName("cat");
            Patches.GetComponent<PatchCreator>().CreatePatches();
            Debug.Log(Patches.GetComponent<PatchCreator>().model.Points3D.Count);
            _test = false;
	    }
	}
}
