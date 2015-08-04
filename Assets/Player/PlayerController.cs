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
    [SyncVar]
    public float FireRate = 0.5f;
    [SyncVar]
    public float ShotSpeed = 10;

    public float LastShot = 0;

    public GameObject ProjectilePrefab;
    Rigidbody2D Body;

	// Use this for initialization
	void Start () {
        Body = GetComponent<Rigidbody2D>();
        if (isLocalPlayer)
            GameObject.Find("MainCamera").GetComponent<FollowPlayerShip>().Player = gameObject;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (isLocalPlayer)
        {
            Cmd_ProcessMovement(Input.GetKey(KeyCode.A),
                Input.GetKey(KeyCode.D),
                Input.GetKey(KeyCode.W));

            Cmd_ProcessShoot(Input.GetKey(KeyCode.Space));
        }
	}

    private void Cmd_ProcessShoot(bool v)
    {
        if (v && LastShot >= FireRate) {
            LastShot = 0;

            GameObject BulletInstance = (GameObject)Instantiate(ProjectilePrefab,  transform.position + transform.up * 2, Quaternion.identity);
            BulletInstance.GetComponent<Rigidbody2D>().AddForce(transform.up * ShotSpeed);
            NetworkServer.Spawn(BulletInstance);
        }

        LastShot += Time.deltaTime;
    }

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

    private void Cmd_ProcessRotation(bool left, bool right)
    {
        if (right)
            Body.AddTorque(-TurnSpeed);
        else if (left)
            Body.AddTorque(TurnSpeed);
    }

    [Command]
    private void Cmd_ProcessMovement(bool left, bool right, bool forward) {
        Cmd_ProcessRotation(left, right);
        Cmd_ProcessThrust(forward);
    }
}
