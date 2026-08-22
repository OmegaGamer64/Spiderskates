using UnityEngine;

public class EnemyController : MonoBehaviour
{
 public enum ENEMY_STATE {IDLE, AGGRESSIVE, GRAPPLED, DEAD};

    public ENEMY_STATE enemyState = ENEMY_STATE.IDLE;

}
