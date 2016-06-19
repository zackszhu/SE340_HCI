using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ColorManager : MonoBehaviour {
    [HideInInspector] public Color Color;
    [HideInInspector] public Texture2D Tex;
    public bool canColor;
    public bool canTex;
    public GameObject ImagePlane;
    public GameObject MeshPrefab;
    public GameObject Canvas;
    public GameObject FinishButton;
    public string Name;




    public MyMesh currMesh;

    void Start() {
        canColor = false;
        canTex = false;
        Color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }

    public void CanColor() {
        canColor = true;
        canTex = false;
    }

    public void CanTex() {
        canTex = true;
        canColor = false;
    }

    public void StartTex(List<Vector2> paperPoints , MyMesh selectEdMesh ) {

        currMesh = selectEdMesh;

        var obj = Instantiate(ImagePlane);
        obj.GetComponent<MeshRenderer>().material.mainTexture =
            GameObject.FindGameObjectWithTag("color").GetComponent<ColorManager>().Tex;

        


        GameObject.FindWithTag("MainCamera").GetComponent<CameraInput>().CanZoom = false;
        foreach (var o in GameObject.FindGameObjectsWithTag("Respawn")) {
            o.GetComponent<RotationInput>().enabled = false;
        }
        // GameObject.FindWithTag("Respawn").GetComponent<RotationInput>().enabled = false;
        List<Vector3> vertices = new List<Vector3>();
        foreach (var paperPoint in paperPoints) {
            vertices.Add(new Vector3(paperPoint.x, paperPoint.y, 0) * 100);
        }
        var obj2 = Instantiate(MeshPrefab, new Vector3(-250, -250, 795), Quaternion.identity) as GameObject;
        obj2.GetComponent<PatchCreator>().CreatePatch(vertices, -1, new []{1, 1});
        obj2.GetComponent<PatchCreator>().transform.localScale = new Vector3(2, 2, 2);
        var tmp = obj2.GetComponentInChildren<MyMesh>().gameObject;
        tmp.GetComponent<MeshRenderer>().material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        tmp.AddComponent<MouseDrag>();
        Destroy(tmp.GetComponent<MyMesh>());
        //canTex = false;

        //隐藏finish
        FinishButton.SetActive(false);

        Canvas.SetActive(true);
    }


    public bool DoRender() {
        return  currMesh.SetTexture();
    }

    public void Neither() {
        canTex = false;
        canColor = false;
    }

    public bool Close() {
        if (currMesh.SetTexture()) {
            foreach (var obj in GameObject.FindGameObjectsWithTag("imageBack")) {
                Destroy(obj);
            }
            foreach (var pbj in GameObject.FindGameObjectsWithTag("imageMesh")) {
                Destroy(pbj);
                
            }
            //GameObject.Destroy(GameObject.FindWithTag("imageBack"));
            //Destroy(GameObject.FindWithTag("imageMesh"));

            foreach (var o in GameObject.FindGameObjectsWithTag("Respawn"))
            {
                o.GetComponent<RotationInput>().enabled = true;
            }

            FinishButton.SetActive(true);
            return true;
        }
        return false;

    }
}
