using UnityEngine;
using System.Collections;

public class TubeLayer : MonoBehaviour
{

    private float angleDirectionTube;

    private Vector3 direction;

    private SpriteRenderer tube;

    void OnTriggerStay(Collider info)
    {
        if (info.name.Contains("Tube"))
        {
            tube = info.gameObject.GetComponentInChildren<SpriteRenderer>();

            direction = transform.parent.gameObject.GetComponent<KartController>()._direction;

            angleDirectionTube = Vector3.Angle(direction, info.transform.position - transform.position);

            if (angleDirectionTube < 90)
                tube.sortingOrder = 2;
            else
                tube.sortingOrder = 4;
        }
        else
            tube = null;
    }
}