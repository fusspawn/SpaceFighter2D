using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class DisableOwnerOnlyComponents : NetworkBehaviour {

    public List<MonoBehaviour> DisableForRemotePlayer = new List<MonoBehaviour>();
	// Use this for initialization
	void Start () {
        if (isLocalPlayer) {
            return;
        }

        foreach (var comp in DisableForRemotePlayer) {
            comp.enabled = false;
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
