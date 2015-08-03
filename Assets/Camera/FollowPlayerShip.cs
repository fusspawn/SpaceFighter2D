using UnityEngine;
using System.Collections;

public class FollowPlayerShip 
    : MonoBehaviour {

    public GameObject Player;
    public float MoveSpeed;

	void Update ()
    {
        if (Player != null)
        {
            Vector3 Pos = Vector3.Lerp(transform.position, Player.transform.position, MoveSpeed * Time.deltaTime);
            Pos.z = -10;
            transform.position = Pos;
        }
	}
}
