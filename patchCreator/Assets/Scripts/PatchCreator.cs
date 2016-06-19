using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Assets.Scripts;
using UnityEditor;
using UnityEngine.Networking;
using Application = UnityEngine.Application;
using Color = UnityEngine.Color;


[Serializable]
public class DisplayConfig {
    // public int Id;   //材质id
    public int Id = 0;
    public int Type = -1; //0 - 无视, -1 ~color , 1~n ~ 贴图编号 
    public Color Argb = Color.white; //颜色
    public string Name; //材质文件名

    public List<Vector2> Uv;
    [NonSerialized] public GameObject Object; //对应的对象
}



[Serializable]
public class Patch {
    public List<int> VertNum = new List<int>();
    public int[] TouchAble = new int[2]; //正反面分别能不能被着色
    public int[] DisplayId = new int[2]; //正反面材质是什么, 默认0的话就无视

    [NonSerialized] public List<Vector3> ModelPoints = new List<Vector3>();
    [NonSerialized] public List<Vector2> PaperPoints = new List<Vector2>();
    //[NonSerialized] public DisplayConfig Displaycfg;
}


[Serializable]
public class ModelData {
    //全局变量好被序列化和反序列化。

    public List<Vector3> Points3D = new List<Vector3>(); //
    public List<Vector2> Points2D = new List<Vector2>();
    public List<Patch> Patches = new List<Patch>();
    public List<DisplayConfig> Displays = new List<DisplayConfig>();
    //public List<DisplayConfig> Displays = new List<DisplayConfig>();

    public float ExpectedScale3D = 3.0f;
    //----- 以下变量都是为了
    private float _scale3D = 1.0f; //xyz中最大的跨度
    private Vector3 _center3D = new Vector3(0, 0, 0);
    private float _scale2D = 1.0f;
    private Vector2 _center2D = new Vector2(0, 0);

    //TODO 组装


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
        //Debug.Log(temp);
        return Convert.ToInt32(temp);
    }

    private float ReadFloat(StreamReader sr) {
        return Convert.ToSingle(ReadText(sr));
    }

    //load from data file
    public ModelData(StreamReader sr) {
        int pointsSize = ReadNumber(sr);
        int patchSize = ReadNumber(sr);
        //load points
        //Debug.Log(":" + pointsSize + ":" + patchSize);
        for (int i = 0; i < pointsSize; i++) {
            Points3D.Add(new Vector3(ReadFloat(sr), ReadFloat(sr), ReadFloat(sr)));
        }
        //Debug.Log("points read over");
        //Debug.Log(points3D.Count);
        for (int i = 0; i < patchSize; i++) {
            int pointCount = ReadNumber(sr);
            Patches.Add(new Patch());
            for (int j = 0; j < pointCount; j++) {
                Patches[i].VertNum.Add(ReadNumber(sr));
            }
            bool isFront = ReadNumber(sr) > 0 ? true : false;
            if (!isFront)
                Patches[i].VertNum.Reverse();
            //Patches[i].Display[0] = ReadNumber(sr);
            //Patches[i].Display[1] = ReadNumber(sr);
            //Debug.Log("Patches" + i);
        }
        //Debug.Log("patches read over");

        for (int i = 0; i < pointsSize; i++) {
            Points2D.Add(new Vector2(ReadFloat(sr), ReadFloat(sr)));
        }
    }


    //获得归一化之后的点
    public List<Vector3> GetPatchModelPoints(int index) {
        return Patches[index].ModelPoints;
    }

   


    public List<Vector2> Get2DPatch(int index) {
        return Patches[index].VertNum.Select(pointNum => Points2D[pointNum]).ToList();
    }


    public void InitScaleAndPoints() {
        InitCenterAndScale();
        InitNormalizePoints();
    }


    private void InitCenterAndScale() {
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
        _scale2D = Mathf.Min(temp2D.x, temp2D.y);
        //Debug.Log(_center3D);
    }


    private void InitNormalizePoints() {
        foreach (var patch in Patches) {
            foreach (var point3DId in patch.VertNum) {
                var temp = (new Vector3(Points3D[point3DId].x, Points3D[point3DId].y, Points3D[point3DId].z) - _center3D)/
                           _scale3D * ExpectedScale3D;
                patch.ModelPoints.Add(temp);
            }
        }
        
        foreach (var patch in Patches) {
            foreach (var point2DId in patch.VertNum) {
                //var temp = (new Vector2(Points2D[point2DId].x, Points2D[point2DId].y) - _center2D)/_scale2D;
                patch.PaperPoints.Add(Points2D[point2DId]);
            }
        }
    }
    
    private int ARgb2Int(Color argb) {
        return (((int) (argb.r*255)) << 16) + (((int) (argb.g*255)) << 8) + (((int) (argb.b*255)));
    }

    private List<PointF> UVModel2Image(List<Vector2> uvs) {
        var result = new List<PointF>();
        foreach (var uv in uvs) {
            result.Add(new PointF(uv.x, 1 - uv.y));
        }
        //TODO LINE EXPRESSION
        return result;
    }


    public List<ImageGenerator.Face> GetExportData(int PaperSize = 512) {
        var result = new List<ImageGenerator.Face>();

        foreach (var patch in Patches) {
            var face = new ImageGenerator.Face();
            foreach (var paperPoionts in patch.PaperPoints) {
                var temp = new Point((int) (paperPoionts.x*PaperSize), (int) (( 1-paperPoionts.y)*PaperSize));
                face.Points.Add(temp);
            }

            //Front
            var display = Displays[patch.DisplayId[0]];


            face.MaterialFront = display.Type;

            if (patch.TouchAble[0] == 0)
                face.MaterialFront = -2;
            if (display.Type == -1) // color         
                face.ColorFront = ARgb2Int(display.Argb);
            else if (display.Type >= 0) {
                face.Uvfront = UVModel2Image(display.Uv);
                face.ImageFront = display.Name;
            }

            //Back

            display = Displays[patch.DisplayId[1]];



            face.MaterialBack = display.Type;
            if (patch.TouchAble[1] == 0)
                face.MaterialBack = -2;

            if (display.Type == -1) // color         
                face.ColorBack = ARgb2Int(display.Argb);
            else if (display.Type >=0 ){
                face.Uvback = UVModel2Image(display.Uv);
                face.ImageBack = display.Name;
            }
            result.Add(face);
        }
        return result;
    } 


    public int GetPatchesCount() {
        return Patches.Count;
    }

    public int CreateDisplay() {
        Displays.Add(new DisplayConfig());
        return Displays.Count - 1;
    }
}


public class PatchCreator : MonoBehaviour {
    public GameObject meshSample;
    public GameObject PatchSample;

    public string ModelName;

    public ModelData model;

    private Bitmap[] bm = new Bitmap[2];



    public void CreatePatch(List<Vector3> verticles, int index, int[] touchAble) {
        //front 
        var patch = Instantiate(PatchSample, transform.position, Quaternion.identity) as GameObject;

        patch.transform.parent = gameObject.transform;
        patch.transform.localScale = Vector3.one;
        patch.transform.localRotation = Quaternion.identity;

        if (touchAble[0] > 0) {
            var front = Instantiate(meshSample, transform.position, Quaternion.identity) as GameObject;
            //Debug.Log(verticles.Count);
            front.GetComponent<MyMesh>().SetMesh(verticles, model, index, true, touchAble[0] > 0);
            front.name = "front";
            //front.tag = "front";
            front.transform.parent = patch.transform;
            front.transform.localScale = Vector3.one;
            front.transform.localRotation = Quaternion.identity;
        }

        if (touchAble[1] > 0) {
            var back = Instantiate(meshSample, transform.position, Quaternion.identity) as GameObject;
            back.GetComponent<MyMesh>().SetMesh(verticles, model, index, false, touchAble[1] > 0);
            back.name = "back";
            //back.tag = "back";
            back.transform.parent = patch.transform;
            back.transform.localScale = Vector3.one;
            back.transform.localRotation = Quaternion.identity;
        }


    }


    public void LoadModel(string filePath) {
        //根据文件名载入
        FileStream fs = new FileStream(filePath, FileMode.Open);
        StreamReader sr = new StreamReader(fs);

        try {
            if (filePath.EndsWith(".zzs")) {
                model = new ModelData(sr);
            }
            else if (filePath.EndsWith(".json")) {
                model = JsonUtility.FromJson<ModelData>(sr.ReadToEnd());
            }
            else if (filePath.EndsWith(".xml")) {
                model = PersistTool.Xml2Obj<ModelData>(sr.BaseStream);
            }
            else throw new UnityException("file format load error");
        }
        catch (Exception e) {
            //Debug.LogError(e.Message);
        }

        finally {
            sr.Close();
            fs.Close();
        }
        model.InitScaleAndPoints();
    }

    public void LoadModelByModelName(string modelName) {
        ModelName = modelName;
        LoadModel(Application.dataPath + "/Data/" + modelName + ".json");
    }


    public void CreatePatches() {
        //Load files
        //LoadModel(Application.dataPath+"/Data/"+ ModelName+".json");

        int Count = model.GetPatchesCount();
        //CreatePatch(model.GetPatchModelPoints(0));
        //CreatePatch(model.GetPatchModelPoints(0));


        for (int i = 0; i < Count; i++) {
            CreatePatch(model.GetPatchModelPoints(i), i, model.Patches[i].TouchAble);
        }
    }



    public void SaveModel(string filePath) {
        FileStream fs = new FileStream(filePath, FileMode.Create);
        var sw = new StreamWriter(fs);
       // Debug.Log(filePath);

        try {
            if (filePath.EndsWith(".xml")) {
                sw.Write(PersistTool.Obj2Xml(model));
            }
            else if (filePath.EndsWith(".json")) {
                sw.Write(JsonUtility.ToJson(model));
            }
            else throw new UnityException("file format load error");
        }
        catch (Exception e) {
         //   Debug.LogError(e.Message);
        }
        finally {
            sw.Close();
            fs.Close();
        }
        //JsonMapper.
    }

    public void SaveModelByModelName(string modelName, bool withTime = true) {
        string path = withTime
            ? Application.dataPath + "/Data/" + modelName + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".json"
            : Application.dataPath + "/Data/" + modelName + ".json";

        SaveModel(path);
    }


    public void GenerateBitMap(int paperSize = 512) {
        var exportData = model.GetExportData(paperSize);
        //Debug.Log(JsonUtility.ToJson(exportData) );
        bm[0] = ImageGenerator.Generate(exportData, true, paperSize);
        bm[1] = ImageGenerator.Generate(exportData, false, paperSize);
    }

    public void ExportImage(string filePath) {
        
        bm[0].Save(filePath + "temp_front.jpg");
        bm[1].Save(filePath + "temp_back.jpg");


    }

    public void Export() {
        GenerateBitMap();

        SaveModelByModelName(ModelName);

        ExportImage(Application.dataPath + "/Data/gpdf/" );
        string s = EditorUtility.SaveFilePanel("save pdf to print", Application.dataPath, "untitle.pdf", "pdf");

        string path = Application.dataPath + "/Data/gpdf/";
        string exe = path + "gpdf.exe";
        string argu = path + "temp_front.jpg " + path + "temp_back.jpg " + s;

        //Debug.Log("exe: " + exe + "\narg: " + argu  );
        Process.Start(exe, argu);
        //Debug.Log(s);

    }

    public void Set() {
        
    }




void Update() {
    }
}