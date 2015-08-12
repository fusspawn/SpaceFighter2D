using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Collections.Generic;
using Devdog.InventorySystem;

public class PlayerController 
    : MonoBehaviour, IInventoryPlayerController {
    public ShipStats Stats;
    public float MaxSpeed;
    public float CurrentSpeed;
    public float TurnSpeed;
    public float DragRate;
    public float FireRate = 0.5f;
    public float ShotSpeed = 10;
    public float ShotLife = 5;
    public Color Col;
    public float ScrollScale = 10f;
    public bool Left;
    public bool Right;
    public bool Forward; 

    public float LastShot = 0;

    public GameObject ProjectilePrefab;
    public List<GameObject> LeftThruster;
    public List<GameObject> RightThruster;
    Rigidbody2D Body;

    public List<TurretController> TurretList;
    public AudioSource EngineAudio;


	// Use this for initialization
	void Start () {
        EngineAudio = GetComponent<AudioSource>();
        Body = GetComponent<Rigidbody2D>();
        Stats = GetComponent<ShipStats>();
        GameManager.PlayerStats = Stats;
        GameManager.PlayerObject = gameObject;  
        GameObject.Find("MainCamera").GetComponent<FollowPlayerShip>().Player = gameObject;
        GameManager.ActivePlayers.Add(gameObject);
        //transform.position = new Vector2(GameManager.Instance.WorldXBounds / 2,
        //    GameManager.Instance.WorldYBounds / 2);
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        Cmd_ProcessMovement(Input.GetKey(KeyCode.A),
                Input.GetKey(KeyCode.D),
                Input.GetKey(KeyCode.W));
        Cmd_ProcessShoot(Input.GetMouseButton(0), 
                Camera.main.ScreenToWorldPoint(Input.mousePosition));
	}


    private void Cmd_ProcessShoot(bool ShouldShoot, Vector3 TargetPos)
    {
        foreach (TurretController Turret 
            in GetComponentsInChildren<TurretController>())
        {
            Turret.WantsToFire = ShouldShoot;
            Turret.TrackTarget = TargetPos;
        }
    }

    private void Cmd_ProcessThrust(bool thrust)
    {        
        if (thrust)
        { 
            if ( Stats.CurrentSpeed < Stats.MaxSpeed)
            {
                Stats.CurrentSpeed = Mathf.Lerp(Stats.CurrentSpeed, Stats.MaxSpeed, Time.deltaTime);
            }

            Body.AddForce(transform.up * Stats.CurrentSpeed);
        }
        else {
            if(Stats.CurrentSpeed > 0f)
                Stats.CurrentSpeed = Mathf.Lerp(Stats.CurrentSpeed, 0f, Time.deltaTime);
            if (Stats.CurrentSpeed < 0.1f)
                Stats.CurrentSpeed = 0f;
        }
    }

    private void Cmd_ProcessRotation(bool left, bool right)
    {
        if (right)
            Body.AddTorque(-Stats.TurnSpeed);
        else if (left)
            Body.AddTorque(Stats.TurnSpeed);
    }

    private void Cmd_ProcessMovement(bool left, bool right, bool forward) {
        Left = left;
        Right = right;
        Forward = forward;
        Cmd_ProcessRotation(left, right);
        Cmd_ProcessThrust(forward);
    }

    void Update() {
         UpdateThrusters();
    }

    private void UpdateThrusters()
    {
        if (!Right && !Forward)
            foreach(var thrust in LeftThruster)
                thrust.SetActive(false);
        else
            foreach (var thrust in LeftThruster)
                thrust.SetActive(true);

        if (!Left && !Forward)
            foreach (var thrust in RightThruster)
                thrust.SetActive(false);
        else
            foreach (var thrust in RightThruster)
                thrust.SetActive(true);

        if (Forward)
        {
            foreach (var thrust in LeftThruster)
                thrust.SetActive(true);

            foreach (var thrust in RightThruster)
                thrust.SetActive(true);
        }

        if (Right || Left || Forward)
        {
            if (!EngineAudio.isPlaying)
            {
                EngineAudio.Play();
            }
        }
        else
        {
            if (EngineAudio.isPlaying)
                EngineAudio.Stop();
        }
    }

    public void SetActive(bool set)
    {
        gameObject.SetActive(false);
    }
}
