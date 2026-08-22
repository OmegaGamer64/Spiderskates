using UnityEngine;
using static WebLogic;

public class WebEndLogic : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        WebLogic parent = GetComponentInParent<WebLogic>();

        if (collision && parent.webState == WEB_STATE.SHOOTING)
        {

            

            if (collision.CompareTag("Enemy"))
            {

                collision.GetComponentInParent<EnemyController>().enemyState = EnemyController.ENEMY_STATE.GRAPPLED;
                parent.webState = WEB_STATE.GRAPPLING;

                parent.grappledEnemy = collision.gameObject;

            }

            else if (collision.CompareTag("Ground")
                      || collision.CompareTag("Wall")
                      || collision.CompareTag("WallLeft")
                      || collision.CompareTag("WallRight")
                      || collision.CompareTag("Untagged"))
            {

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
}
