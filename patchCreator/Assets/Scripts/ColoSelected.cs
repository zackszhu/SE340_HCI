using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ColoSelected : MonoBehaviour {
    public ColorManager ColorManager;

    public void OnClick() {
        Debug.Log(GetComponent<Image>().color);
        ColorManager.CanColor();
        ColorManager.Color = GetComponent<Image>().color;
    }

    public void OnClickTex() {
        Debug.Log(GetComponent<Image>().color);
        ColorManager.CanTex();
    }

    public void Work() {
        var path = UnityEditor.EditorUtility.OpenFilePanel("Select image to insert", "", "jpg");
        if (path != "") {
            Texture2D tex = new Texture2D(1, 1);
            var bytes = System.IO.File.ReadAllBytes(path);
            tex.LoadImage(bytes);
           
            ColorManager.Tex = tex;
            ColorManager.Name = path;
            OnClickTex();
        }
    }

}
