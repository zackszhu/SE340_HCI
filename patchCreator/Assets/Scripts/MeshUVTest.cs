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

    public float gapOffset  = 0.01f;


    public void SetMesh(List<Vector3> vertices, bool isFront, bool TouchAble = false) //touchAble 的offset要更大一些
    {
        var mesh = gameObject.GetComponent<MeshFilter>().mesh;
     
        var len = vertices.Count;
        var temp = new Vector3[len];
        vertices.CopyTo(temp);


        if (!isFront)
        {
            Array.Reverse(temp);
        }


        var triangles = new int[(len - 2) * 3];
        for (var i = 0; i < len - 2; i++)
        {
            triangles[i * 3 + 0] = 0;
            triangles[i * 3 + 1] = (i + 1) % len;
            triangles[i * 3 + 2] = (i + 2) % len;
        }

        
        Vector2[] uvs =
        {
            new Vector3(0,1),
            new Vector3(0.433f,0.75f),
            new Vector3(0.183f,0.316f),
        };
        

        mesh.Clear();
        //assignment 

        mesh.vertices = temp;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        //mesh.uv2 = uvs;
        mesh.Optimize();
        mesh.RecalculateNormals();
        MakeGap(mesh.normals[0],TouchAble);
    }

    public class Temp {
        public List<Vector3> X = new List<Vector3>();/*= new List<Vector3>() {
            new Vector3(1,2,3)
        };*/
        public int I = 10;
        public int T = 5;
        public float ss = 55.0f;
        public string kk = "123465";
        private int j = 5;

    }


    private void MakeGap(Vector3 normal,bool TouchAble)
    {

        var t = new Temp();
        var XmlString = PersistTool.Obj2Xml(t);
       
        Debug.Log(XmlString);

        Temp gg = PersistTool.Xml2Obj<Temp>(XmlString);

        gg.X.Add(new Vector3(3.2f, 3, 3));

        Debug.Log("gg" + PersistTool.Obj2Xml(gg));
        

        var temp = new Vector3(normal.x, normal.y, normal.z);



        temp.Normalize();

        gameObject.transform.Translate(temp * gapOffset);
    }


    void Awake () {
        /*
        List<Vector3> vertices = new List<Vector3>() {
            new Vector3(0, 0, 0), 
            new Vector3(-0.5f, 0, 0), 
            new Vector3(-0.5f, 0, 0.5f), 
        };
        SetMesh(vertices, true);
        */

        Patches.GetComponent<PatchCreator>().CreatePatches();

    }


	
	// Update is called once per frame
	void Update () {
	
	}
}
