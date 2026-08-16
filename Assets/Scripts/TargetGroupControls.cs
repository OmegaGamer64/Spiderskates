using Unity.Cinemachine;
using UnityEngine;

public class TargetGroupControls : MonoBehaviour
{
    WebLogic webLogic;
    void Start()
    {
        webLogic = FindAnyObjectByType<WebLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        if(webLogic.webState ==WebLogic.WEB_STATE.SHOOTING || webLogic.webState == WebLogic.WEB_STATE.SHOT)
        {
            GetComponent<CinemachineTargetGroup>().Targets[1].Object = webLogic.webEnd.transform;
        }
    }
}
