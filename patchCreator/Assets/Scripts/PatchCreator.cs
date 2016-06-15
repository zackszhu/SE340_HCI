using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;


public class Patch {
    public List<int> VertNum = new List<int>();
    //public List<int> vert2DNum = new List<int>();
    public int[] material = new int[2];
}


public class ModelData {

    private List<Vector3> points3D;
    private List<Vector2> points2D;
    private List<Patch> patches;

    private bool IsValidChar(char c) {
        return ' ' < c && c < '~';
    }


    private string ReadText(StreamReader sr) {
        //skip start
        //char next = Convert.ToChar(sr.Read());
        while (!IsValidChar(Convert.ToChar(sr.Peek()))) sr.Read();
        String result = "";

        while (IsValidChar(Convert.ToChar(sr.Peek()))) {
            result += Convert.ToChar(sr.Read());
        }
        
        return result;

    }

    private int ReadNumber(StreamReader sr) {
        string temp = ReadText(sr);
        Debug.Log(temp);
        return Convert.ToInt32(temp);
    }



    private float ReadFloat(StreamReader sr) {
        return Convert.ToSingle(ReadText(sr));
    }

    //load from data file
    public ModelData( string filePath ) {
        points3D = new List<Vector3>();
        points2D = new List<Vector2>();
        patches = new List<Patch>();
        FileStream fs = new FileStream(filePath,FileMode.Open);
        StreamReader sr = new StreamReader(fs);
        int pointsSize = ReadNumber(sr);
        int patchSize = ReadNumber(sr);
        


        //load points
        //Debug.Log(":" + pointsSize + ":" + patchSize);
        for (int i = 0; i < pointsSize; i++) {
            points3D.Add(new Vector3(ReadFloat(sr),ReadFloat(sr),ReadFloat(sr)));
        }
        Debug.Log("points read over");
        //Debug.Log(points3D.Count);
        for (int i = 0; i < patchSize; i++) {
            int pointCount = ReadNumber(sr);
            patches.Add(new Patch());
            for (int j = 0; j < pointCount; j++) {
                patches[i].VertNum.Add(ReadNumber(sr));
            }
            bool isFront = ReadNumber(sr) > 0 ? true : false;
            if (!isFront)
                patches[i].VertNum.Reverse();
            patches[i].material[0] = ReadNumber(sr);
            patches[i].material[1] = ReadNumber(sr);
            Debug.Log("Patches" + i);
           
        }
        Debug.Log("patches read over");

        for (int i = 0; i < pointsSize; i++)
        {
            points2D.Add(new Vector2(ReadFloat(sr), ReadFloat(sr)));
        }
        fs.Close();
    }


    public List<Vector3> Get3DPatch(int index) {
        //resharper suggestion
        return patches[index].VertNum.Select(pointNum => points3D[pointNum]).ToList();
    }

    public List<Vector2> Get2DPatch(int index) {
        return patches[index].VertNum.Select(pointNum => points2D[pointNum]).ToList();
    }


    public int GetPatchesCount() {
        return patches.Count;
    }
}


public class PatchCreator : MonoBehaviour {
    public GameObject meshSample;
    public GameObject PatchSample;

    public string ModelName;

    private ModelData model;

    //public string DataPath;


    // Use this for initialization


    private void CreatePatch(List<Vector3> verticles) {
        //front 
        var patch = Instantiate(PatchSample, transform.position, Quaternion.identity) as GameObject;

        patch.transform.parent = gameObject.transform;
        patch.transform.localScale = Vector3.one;
        patch.transform.localRotation = Quaternion.identity;

        var front = Instantiate(meshSample, transform.position, Quaternion.identity) as GameObject;
        Debug.Log(verticles.Count);

        front.GetComponent<MyMesh>().SetMesh(verticles, true);
        front.name = "front";
        front.tag = "front";
        front.transform.parent = patch.transform;
        front.transform.localScale = Vector3.one;
        front.transform.localRotation = Quaternion.identity;

        var back = Instantiate(meshSample, transform.position, Quaternion.identity) as GameObject;
        back.GetComponent<MyMesh>().SetMesh(verticles, false);
        back.name = "back";
        back.tag = "back";
        back.transform.parent = patch.transform;
        back.transform.localScale = Vector3.one;
        back.transform.localRotation = Quaternion.identity;
    }


    public void CreatePatches() {
        //Load files
        model = new ModelData(Application.dataPath+"/Data/"+ ModelName+".zzs");

        int Count = model.GetPatchesCount();
        //CreatePatch(model.Get3DPatch(0));
        //CreatePatch(model.Get3DPatch(0));
        
        for (int i = 0; i < Count; i++) {
            CreatePatch(model.Get3DPatch(i));
        }
        

        /*

        List<Vector3> vertices = new List<Vector3>() {
            //front face
            new Vector3(0, 0, 1), //left top front ,0
            new Vector3(1, 0, 1), //right top front ,1
            new Vector3(1, 1, 1), // Left bottom, 2 

            new Vector3(0, 1, 1), // right bottom, 3
                        //new Vector3(-1, 0.5F , 1),
        };*/

        //Application.dataPath;

        // CreatePatch(vertices);
    }

    public void setFile(String DataName) {
        this.ModelName = DataName;
    }


//    void Start() {
//        CreatePatches();
//    }

    // Update is called once per frame
    void Update() {
    }
}