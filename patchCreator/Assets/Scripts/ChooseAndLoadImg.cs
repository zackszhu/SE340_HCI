using UnityEngine;
using System.Collections;

public class ChooseAndLoadImg : MonoBehaviour {
    public ColorManager ColorManager;

}


/*
public class ChooseAndLoadImg : MonoBehaviour {
    public ColorManager ColorManager;

    public void Work() {
        var path = EditorUtility.OpenFilePanel("Select image to insert", "", "jpg");
        if (path != "") {
            Texture2D tex = new Texture2D(1, 1);
            var bytes = System.IO.File.ReadAllBytes(path);
            tex.LoadImage(bytes);
            ColorManager.Tex = tex;
            ColorManager.Name = path;
        }
    }
}



*/