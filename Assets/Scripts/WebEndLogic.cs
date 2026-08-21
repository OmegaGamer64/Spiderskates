using UnityEngine;
using static WebLogic;

public class WebEndLogic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision && collision.CompareTag("Ground") 
                      || collision.CompareTag("Wall")
                      || collision.CompareTag("WallLeft")
                      || collision.CompareTag("WallRight")
                      || collision.CompareTag("Untagged"))
        {
            WebLogic parent = GetComponentInParent<WebLogic>();
            parent.webState = WEB_STATE.SHOT;

            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

            SpringJoint2D webSpring = GetComponent<SpringJoint2D>();
            parent.webSpring = webSpring;
            webSpring.enabled = true;
            webSpring.connectedBody = parent.playerController.rb;
            webSpring.distance = (Vector2.Distance(parent.playerController.transform.position, 
                parent.webEnd.transform.position) * (parent.defaultDistanceFromWeb / 100));

        }
    }
}
