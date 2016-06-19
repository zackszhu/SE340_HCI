using UnityEngine;
using System.Collections;

public class Close : MonoBehaviour {
    public void CloseThis() {



        if (GameObject.FindWithTag("color").GetComponent<ColorManager>().Close()) {
            gameObject.SetActive(false);
        }
        
    }
}
