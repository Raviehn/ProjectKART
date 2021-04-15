using UnityEngine;
using System.Collections;

public class HumanController : MonoBehaviour {

    KartController kc;

    RaceManager rm;
    
	void Start () {
        kc = GetComponent<KartController>();        
        rm = GameObject.FindGameObjectWithTag("RaceManager").GetComponent<RaceManager>();
    }
	
    public void GetInput()
    {
        // Input throttle
        if (Input.GetButton("B"))
            kc.throttle = KartController.Throttle.accelerate;
        else if (Input.GetButton("A"))
            kc.throttle = KartController.Throttle.brake;
        else
            kc.throttle = KartController.Throttle.none;

        // Input steering
        if (Input.GetButton("Left"))
            kc.steer = KartController.Steer.left;
        else if (Input.GetButton("Right"))
            kc.steer = KartController.Steer.right;
        else
            kc.steer = KartController.Steer.none;

        // Input action
        if (Input.GetButtonDown("L") && !kc._jump || Input.GetButtonDown("R") && !kc._jump)
            kc._jump = kc._up = true;
    }
}
