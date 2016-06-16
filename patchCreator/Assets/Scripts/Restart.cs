using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Restart : MonoBehaviour {

	public void RestartApplication() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // Application.LoadLevel(Application.loadedLevel);
    }
}
