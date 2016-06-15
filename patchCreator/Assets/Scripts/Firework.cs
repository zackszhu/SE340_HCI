using UnityEngine;
using System.Collections;

public class Firework : MonoBehaviour {
    public GameObject[] Fireworks;

    public void FireworkStart() {
        foreach (var firework in Fireworks) {
            firework.SetActive(true);
        }
    }
}
