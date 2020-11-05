using Mirror;
using System.Collections;
using UnityEngine;

public class cannonShellScript : MonoBehaviour {
    private void Awake() {
        StartCoroutine(suicide());
        return;
    }

    private IEnumerator suicide() {
        yield return new WaitForSeconds(3f);
        NetworkServer.Destroy(gameObject);
    }
}