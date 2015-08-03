using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class PlayerController 
    : NetworkBehaviour {

    [SyncVar]
    public float MaxSpeed;
    [SyncVar]
    public float CurrentSpeed;
    [SyncVar]
    public float TurnSpeed;
    [SyncVar]
    public float DragRate;

    Rigidbody2D Body;

	// Use this for initialization
	void Start () {
        Body = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (isLocalPlayer)
        {
            Cmd_ProcessRotation(Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.D));
            Cmd_ProcessThrust(Input.GetKey(KeyCode.W));
        }
	}

    [Command]    
    private void Cmd_ProcessThrust(bool thrust)
    {        
        if (thrust)
        {
            if (CurrentSpeed < MaxSpeed)
            {
                CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, Time.deltaTime);
            }

            Body.AddForce(transform.up * CurrentSpeed);
        }
        else {
            if(CurrentSpeed > 0f)
                CurrentSpeed = Mathf.Lerp(CurrentSpeed, 0f, Time.deltaTime);
            if (CurrentSpeed < 0.1f)
                CurrentSpeed = 0f;
        }
    }

    [Command]
    private void Cmd_ProcessRotation(bool left, bool right)
    {
        if (right)
            Body.AddTorque(-TurnSpeed);
        else if (left)
            Body.AddTorque(TurnSpeed);
    }
}
