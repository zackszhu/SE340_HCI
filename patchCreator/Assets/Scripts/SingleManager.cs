using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

public class SingleManager : MonoBehaviour {

    public GameObject Scroll;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame

    public void Export() {

       
        var selectedObject = Scroll.GetComponent<ModelScrollLayout>().GetSelectModel();
        var patchManager = selectedObject.GetComponent<PatchCreator>();
        patchManager.Export();

    }
}
