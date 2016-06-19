using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using UnityEngine;

public class MyMesh : MonoBehaviour {
    // Use this for initialization

    private float GapOffset = 0.0001f;   //防止重叠的凸出
    public int Index;
    public bool IsFront;
    public ModelData Model;

    private Color _original = Color.white;

    public void SetOriginal(Color original) {
        var dId = Model.Patches[Index].DisplayId[IsFront ? 0 : 1];
        if (dId <= 0)
            dId = Model.CreateDisplay();
        DisplayConfig display = Model.Displays[dId];

        display.Type = -1;
        _original = original;
        display.Argb = original;

        Model.Patches[Index].DisplayId[IsFront ? 0 : 1] = dId;
    }

    public bool SetTexture() {
        List<Vector2> imgVerts = new List<Vector2>();


        //FUCK! 转成世界坐标
        var obj = GameObject.FindWithTag("imageBack");

        var t = obj.GetComponent<MeshFilter>().mesh.vertices;

        foreach (var vert in t) {
            var temp = obj.transform.TransformPoint(vert);
            Debug.Log(temp);
            imgVerts.Add(new Vector2(temp.x, temp.y));
        }

        //Debug.Log(JsonUtility.ToJson(GameObject.FindWithTag("imageBack").GetComponent<MeshFilter>().mesh.vertices));

        List<Vector2> uvs = new List<Vector2>();
        var meshObj = GameObject.FindWithTag("imageMesh");

        var v = meshObj.GetComponent<MeshFilter>().mesh.vertices;
        foreach (var uv in v)
        {
            var temp = meshObj.transform.TransformPoint(uv);
            Debug.Log(temp);
            uvs.Add(new Vector2(temp.x, temp.y));
        }

        string imagePath = GameObject.FindWithTag("color").GetComponent<ColorManager>().Name;

        //Debug.Log(JsonUtility.ToJson(GameObject.FindWithTag("imageMesh").GetComponent<MeshFilter>().mesh.vertices));

        Debug.Log(imagePath);
        return SetTexture(imagePath,imgVerts, uvs);

    }


    public bool SetTexture(string fileName, List<Vector2> imgVerts, List<Vector2> uvs) {
        float minX = 100000000.0f , minY = 100000000.0f, maxX = -100000000.0f, maxY = -100000000.0f;
        foreach (var verts in imgVerts) {
            minX = Mathf.Min(verts.x, minX);
            minY = Mathf.Min(verts.y, minY);
            maxX = Mathf.Max(verts.x, maxX);
            maxY = Mathf.Max(verts.y, maxY);
        }
        float XWidth = maxX - minX;
        float YWidth = maxY - minY;


        Vector2 BasePoint = new Vector2(minX,minY);
        List<Vector2> paperUv = new List<Vector2>();

        foreach (var uv in uvs) {
            var temp = new Vector2((uv.x - BasePoint.x)/XWidth, (uv.y - BasePoint.y)/YWidth);
            //temp.y = 1 - temp.y;
            if (temp.x < 0 || temp.y < 0 || temp.x > 1 || temp.y > 1) {
                GameObject.FindWithTag("imageMesh").GetComponent<MeshRenderer>().material.color = Color.red;
                StartCoroutine(WaitAndChange());
                return false;
            }
            paperUv.Add(temp);
        }




        var dId = Model.Patches[Index].DisplayId[IsFront ? 0 : 1];
        if (dId <= 0)
            dId = Model.CreateDisplay();
        DisplayConfig display = Model.Displays[dId];


        display.Type = 1;
        display.Name = fileName;
        display.Uv = paperUv;   //TODO 显示要倒一下

        //GameObject.Find("Confirm").GetComponent<Close>().CloseThis();
        Model.Patches[Index].DisplayId[IsFront ? 0 : 1] = dId;
        ApplyUvAndTexture();


        return true;
    }

    IEnumerator WaitAndChange() {
        yield return new WaitForSeconds(0.5f);
        GameObject.FindWithTag("imageMesh").GetComponent<MeshRenderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }


    public void ApplyUvAndTexture() {
        var dId = Model.Patches[Index].DisplayId[IsFront ? 0 : 1];
        DisplayConfig display = Model.Displays[dId];

        if (display.Type < 0) return;

        Texture2D tex = new Texture2D(1, 1);
        var bytes = System.IO.File.ReadAllBytes(display.Name);
        tex.LoadImage(bytes);

        
        var temp =new Vector2[display.Uv.Count ];
        display.Uv.CopyTo(temp);

        

        if (!IsFront) {
            Array.Reverse(temp);
        }

        var m = GetComponent<MeshRenderer>().material;

        m.mainTexture = tex;
        m.color = Color.white;
        //GetComponent<MeshRenderer>().material.colo
        var mesh = gameObject.GetComponent<MeshFilter>().mesh;
        //mesh.uv= new Vector2[] { new Vector2(0,0),new Vector2(0,1),new Vector2(1,1),   };
        mesh.uv = temp;
        mesh.Optimize();


    }


    public void SetMesh(List<Vector3> vertices, ModelData model, int index, bool isFront, bool touchAble = false) {
        

        Model = model;
        Index = index;
        IsFront = isFront;

        if (index >= 0) {
            var dId = model.Patches[index].DisplayId[isFront ? 0 : 1];
            var disPlayer = model.Displays[dId];
            if (disPlayer.Type == -1) {
                _original = disPlayer.Argb;
                GetComponent<MeshRenderer>().material.color = _original;
            }
        }

        gameObject.transform.position = Vector3.zero;
        var mesh = gameObject.GetComponent<MeshFilter>().mesh;

        var len = vertices.Count;
        var temp = new Vector3[len];
        vertices.CopyTo(temp);


        if (isFront) {   //不知道为什么，他是反的
            Array.Reverse(temp);
        }


        var triangles = new int[(len - 2)*3];
        for (var i = 0; i < len - 2; i++) {
            triangles[i*3 + 0] = 0;
            triangles[i*3 + 1] = (i + 1)%len;
            triangles[i*3 + 2] = (i + 2)%len;
        }


        mesh.Clear();

        //Vector3 tempNormal = Vector3.Cross((temp[1] - temp[0]), (temp[2] - temp[1])).normalized;
        

        mesh.vertices = temp;
        mesh.triangles = triangles;
        mesh.Optimize();
        mesh.RecalculateNormals();
       
        Vector3 tempNormal = mesh.normals[0];

        MakeGap(tempNormal, touchAble);

        //Debug.Log(gameObject.transform.position + " " +  index + isFront.ToString() + " " + tempNormal +" " + JsonUtility.ToJson(Model.Patches[Index]));
    }

    private void MakeGap(Vector3 normal,bool touchAble) {
        var temp = new Vector3(normal.x, normal.y, normal.z);
        temp.Normalize();

        if (touchAble) {
            
            gameObject.transform.Translate(temp*GapOffset*1.5f);
            //Debug.Log("fuck");
            //MakeRigid();
            MakeRigid();
        }
        else {
            gameObject.transform.Translate(temp * GapOffset);
        }
        //Debug.Log("p:"+Index + IsFront.ToString() + gameObject.transform.position + " "+ JsonUtility.ToJson(Model.Patches[Index]));
    }


    private void Start() {
       // _original = Color.white;

    }


    private void MakeRigid() {
        gameObject.AddComponent<Rigidbody>();
        var rigidbody = gameObject.GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        gameObject.AddComponent<MeshCollider>();
        //rigidbody.useGravity = false;
    }

    void OnMouseOver() {
        if (GameObject.FindGameObjectWithTag("color").GetComponent<ColorManager>().canColor) {
            GetComponent<MeshRenderer>().material.color = GameObject.FindGameObjectWithTag("color").GetComponent<ColorManager>().Color;
            Debug.Log(_original);
        }
    }
    void OnMouseExit() {
        if (GameObject.FindGameObjectWithTag("color").GetComponent<ColorManager>().canColor) {
            GetComponent<MeshRenderer>().material.color = _original;
        }
    }

    void SetMeshData() {
       //访问model设置数据
    }

    void SetMeshRender() {
        if (GameObject.FindGameObjectWithTag("color").GetComponent<ColorManager>().canColor) {
            GetComponent<MeshRenderer>().material.color = GameObject.FindGameObjectWithTag("color").GetComponent<ColorManager>().Color;

            SetOriginal(GameObject.FindGameObjectWithTag("color").GetComponent<ColorManager>().Color);
            var m = GetComponent<MeshRenderer>().material;
            m.color = _original;
            m.mainTexture = null;
        }
        else if (GameObject.FindGameObjectWithTag("color").GetComponent<ColorManager>().canTex) {

            GameObject.FindGameObjectWithTag("color").GetComponent<ColorManager>().StartTex(Model.Patches[Index].PaperPoints, this);
        }
    }



    void OnMouseDown() {
        SetMeshData();
        SetMeshRender();
    }
    // Update is called once per frame
    private void Update() {}
}