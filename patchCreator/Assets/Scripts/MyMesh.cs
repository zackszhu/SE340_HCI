using System;
using System.Collections.Generic;
using UnityEngine;

public class MyMesh : MonoBehaviour {
    // Use this for initialization

    public float gapOffset = 0.01f;


    public void SetMesh(List<Vector3> vertices, bool isFront) {
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
        /*
        Vector2[] uvs =
        {
            new Vector3(0,0),
            new Vector3(1,0),
            new Vector3(1,1),
            new Vector3(0,1)
        };
        */


        mesh.Clear();
        //assignment 

        mesh.vertices = temp;
        //mesh.vertices = vertices;
        //mesh.SetTriangles(triangles.ToList());
        mesh.triangles = triangles;
        //mesh.uv2 = uvs;
        mesh.Optimize();
        mesh.RecalculateNormals();
        MakeGap(mesh.normals[0]);
    }

    private void MakeGap(Vector3 normal) {
        var temp = new Vector3(normal.x, normal.y, normal.z);


        temp.Normalize();

        gameObject.transform.Translate(temp*gapOffset);
    }


    private void Start() {
        //return;
        /*
        Vector3[] vertices = new Vector3[]
        {
            //front face

            new Vector3(0,0,1), //left top front ,0
            new Vector3(100,0,1),  //right top front ,1
            new Vector3(100,100,1),  // Left bottom, 2 
            new Vector3(0,100,1),  // right bottom, 3
        };

        

        SetMesh(vertices,false);
        */
        /*
        gameObject.AddComponent<Cloth>();
        Cloth temp = gaeObject.GetComponent<Cloth>();
        temp.useGravity = false;
        */
    }


    private void makeRigid() {
        gameObject.AddComponent<Rigidbody>();
        var rigidbody = gameObject.GetComponent<Rigidbody>();

        rigidbody.isKinematic = true;

        gameObject.AddComponent<MeshCollider>();


        //rigidbody.useGravity = false;
    }

    // Update is called once per frame
    private void Update() {}
}