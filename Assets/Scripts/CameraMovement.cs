using System;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour
{
    public Transform player;
    public Transform web;
    public Vector3 offset;
    public float xLock;
    public float yLock = 3;
    Vector3 targetPosition;
    public float damping = 1;

    private Vector3 vel = Vector3.zero;

    void Start()
    {
        player = FindAnyObjectByType<PlayerController>().transform;
        web = FindAnyObjectByType<WebLogic>().transform;
    }
    void LateUpdate()
    {
        if (web.GetComponent<WebLogic>().webState == WebLogic.WEB_STATE.SHOOTING ||
            web.GetComponent<WebLogic>().webState == WebLogic.WEB_STATE.SHOT)
        {
            Vector2 webEndPos = web.GetComponent<WebLogic>().webEnd.transform.position;

            Vector2 combinedEnd = new Vector2(Mathf.Lerp(player.position.x,webEndPos.x,.25f) 
                , Mathf.Lerp(player.position.y, webEndPos.y, .25f));

            targetPosition = new Vector3(combinedEnd.x + offset.x, combinedEnd.y + offset.y, offset.z);
        }
        else
        {
            targetPosition = new Vector3(player.position.x + offset.x, player.position.y + offset.y, offset.z);
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref vel, damping);
    }
}
