using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts;
using UnityEditor;
using UnityEngine.Networking;

[Serializable]
public class Patch {
    public List<int> VertNum = new List<int>();

    //public List<int> vert2DNum = new List<int>();
    public int[] TouchAble = new int[2]; //正反面分别能不能碰到
    public int[] Material = new int[2];  //正反面材质是什么
}

[Serializable]
public class Display {
    
    public int Id;   //材质id
    public int Type;  //0 - color, 1 - texture
    public string Name;  //材质文件名
    public Vector3 Rgb;  //颜色
    public List<Vector2> Uv;
}

[Serializable]
public class ModelData {
    //全局变量好被序列化和反序列化。

    public List<Vector3> Points3D = new List<Vector3>();  //
    public List<Vector2> Points2D = new List<Vector2>();
    public List<Patch> Patches = new List<Patch>();
    public List<Display> Displays = new List<Display>();



    //----- 以下变量都是为了
    private float _scale3D = 1.0f;                //xyz中最大的跨度
    private Vector3 _center3D = new Vector3(0,0,0);
    private float _scale2D = 1.0f;
    private Vector2 _center2D = new Vector2(0,0);



    public ModelData() {

    }

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
    public ModelData( StreamReader sr ) {



        int pointsSize = ReadNumber(sr);
        int patchSize = ReadNumber(sr);
        //load points
        //Debug.Log(":" + pointsSize + ":" + patchSize);
        for (int i = 0; i < pointsSize; i++)
        {
            Points3D.Add(new Vector3(ReadFloat(sr), ReadFloat(sr), ReadFloat(sr)));
        }
        Debug.Log("points read over");
        //Debug.Log(points3D.Count);
        for (int i = 0; i < patchSize; i++)
        {
            int pointCount = ReadNumber(sr);
            Patches.Add(new Patch());
            for (int j = 0; j < pointCount; j++)
            {
                Patches[i].VertNum.Add(ReadNumber(sr));
            }
            bool isFront = ReadNumber(sr) > 0 ? true : false;
            if (!isFront)
                Patches[i].VertNum.Reverse();
            Patches[i].Material[0] = ReadNumber(sr);
            Patches[i].Material[1] = ReadNumber(sr);
            Debug.Log("Patches" + i);

        }
        Debug.Log("patches read over");

        for (int i = 0; i < pointsSize; i++)
        {
            Points2D.Add(new Vector2(ReadFloat(sr), ReadFloat(sr)));
        }
    }



    public List<Vector3> Get3DPatch(int index) {
        //resharper suggestion
        return Patches[index].VertNum.Select(pointNum => Points3D[pointNum]).ToList();
    }

    public List<Vector2> Get2DPatch(int index) {
        return Patches[index].VertNum.Select(pointNum => Points2D[pointNum]).ToList();
    }




    public void InitCenterAndScale() {

        //init 3d scale
        Vector3 min3D = new Vector3(1000000f, 1000000f, 1000000f);
        Vector3 max3D = new Vector3(-1000000f, -1000000f, -1000000f);
        foreach (var point3D in Points3D) {
            min3D.x = Mathf.Min(min3D.x, point3D.x);
            min3D.y = Mathf.Min(min3D.y, point3D.y);
            min3D.z = Mathf.Min(min3D.z, point3D.z);

            max3D.x = Mathf.Max(max3D.x, point3D.x);
            max3D.y = Mathf.Max(max3D.y, point3D.y);
            max3D.z = Mathf.Max(max3D.z, point3D.z);
        }
        _center3D = (min3D + max3D)*0.5f;
        var temp3D = max3D - min3D;
        _scale3D = Mathf.Max(temp3D.x, temp3D.y, temp3D.z);


        //init 2d scale
        Vector3 min2D = new Vector2(1000000f, 1000000f);
        Vector3 max2D = new Vector2(-1000000f, -1000000f);
        foreach (var point2D in Points2D) {
            min2D.x = Mathf.Max(min2D.x, point2D.x);
            min2D.y = Mathf.Max(min2D.y, point2D.y);

            max2D.x = Mathf.Max(max2D.x, point2D.x);
            max2D.y = Mathf.Max(max2D.y, point2D.y);
        }
        _center2D = (min2D + max2D)*0.5f;
        var temp2D = max2D - min2D;
        _scale2D = Mathf.Min(temp2D.x,temp2D.y);

        Debug.Log(_center3D);

    }


    public int GetPatchesCount() {
        return Patches.Count;
    }
}


public class PatchCreator : MonoBehaviour {
    public GameObject meshSample;
    public GameObject PatchSample;

    public string ModelName;

    private ModelData model;



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


    public void LoadData(string filePath) {   //根据文件名载入
        FileStream fs = new FileStream(filePath, FileMode.Open);
        StreamReader sr = new StreamReader(fs);

        if (filePath.EndsWith(".zzs"))
            model = new ModelData(sr);
        else if (filePath.EndsWith(".json"))
            model = JsonUtility.FromJson<ModelData>(sr.ReadToEnd());
        else if (filePath.EndsWith(".xml"))
            model = PersistTool.Xml2Obj<ModelData>(sr.BaseStream);
    }

    public void SaveData(string filePath) {
        
    }


    public void CreatePatches() {
        //Load files
        LoadData(Application.dataPath+"/Data/"+ ModelName+"out.json");

        int Count = model.GetPatchesCount();
        //CreatePatch(model.Get3DPatch(0));
        //CreatePatch(model.Get3DPatch(0));
        
        for (int i = 0; i < Count; i++) {
            CreatePatch(model.Get3DPatch(i));
        }

        model.InitCenterAndScale();

        SaveModel(Application.dataPath + "/Data/" + ModelName + ".json");
       
    }

    public void SetFile(String DataName) {
        this.ModelName = DataName;
    }

    public void LoadModel() {
        
    }

    public void SaveModel(string filePath) {
        FileStream fs = new FileStream(filePath, FileMode.Create);
        var sw = new StreamWriter(fs);

        if (filePath.EndsWith(".xml")) {
            sw.Write(PersistTool.Obj2Xml(model));
        }

        if (filePath.EndsWith(".json")) {
            sw.Write(JsonUtility.ToJson(model));
            
        }
        sw.Close();
        fs.Close();
        //JsonMapper.
    }

    void Update() {


    }
}