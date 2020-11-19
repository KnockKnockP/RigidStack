using Mirror;
using System.Collections;
using UnityEngine;

public class cannonShellScript : NetworkBehaviour {
    private void Start() {
        if (isServer == true) {
            StartCoroutine(suicide());
        }
        return;
    }

    private IEnumerator suicide() {
        yield return new WaitForSeconds(3f);
        NetworkServer.Destroy(gameObject);
    }
}