using System;
using System.Collections.Generic;
using UnityEngine;

public class MyMesh : MonoBehaviour {
    // Use this for initialization

    public float gapOffset = 0.01f;   //防止重叠的凸出


    public void SetMesh(List<Vector3> vertices, bool isFront, bool touchAble = false) {
        var mesh = gameObject.GetComponent<MeshFilter>().mesh;

        var len = vertices.Count;
        var temp = new Vector3[len];
        vertices.CopyTo(temp);


        if (!isFront) {
            Array.Reverse(temp);
        }


        var triangles = new int[(len - 2)*3];
        for (var i = 0; i < len - 2; i++) {
            triangles[i*3 + 0] = 0;
            triangles[i*3 + 1] = (i + 1)%len;
            triangles[i*3 + 2] = (i + 2)%len;
        }

        mesh.Clear();

        mesh.vertices = temp;
        mesh.triangles = triangles;
        mesh.Optimize();
        mesh.RecalculateNormals();
        MakeGap(mesh.normals[0],touchAble);
    }

    private void MakeGap(Vector3 normal,bool touchAble) {
        var temp = new Vector3(normal.x, normal.y, normal.z);
        temp.Normalize();
        gameObject.transform.Translate(temp*gapOffset*1.5f);
        MakeRigid();

    }


    private void Start() {


    }


    private void MakeRigid() {
        gameObject.AddComponent<Rigidbody>();
        var rigidbody = gameObject.GetComponent<Rigidbody>();

        rigidbody.isKinematic = true;

        gameObject.AddComponent<MeshCollider>();


        //rigidbody.useGravity = false;
    }

    // Update is called once per frame
    private void Update() {}
}