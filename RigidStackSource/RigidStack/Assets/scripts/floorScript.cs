﻿using UnityEngine;

public class floorScript : MonoBehaviour {
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("platform") == false) {
            FindObjectOfType<objectScript>().reset();
        }
        return;
    }
}