using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour
{
    //　SetPositionスクリプト
    private EnemySetPosition setPosition;
    private CharacterController enemyController;
    //private Animator animator;
    //　目的地
    private Vector3 destination;
    //　歩くスピード
    private float walkSpeed;
    //　速度
    private Vector3 velocity;
    //　移動方向
    private Vector3 direction;
    //　到着フラグ
    private bool arrived;
    //　スタート位置
    private Vector3 startPosition;
    //　待ち時間
    [SerializeField]
    private float waitTime = 5f;
    //　経過時間
    private float elapsedTime;
    //　敵の状態
    private EnemyState state;
    //　追いかけるキャラクター
    private Transform playerTransform;

    public enum EnemyState
    {
        walk,
        wait,
        chase
    };

    void Start()
    {
        enemyController = GetComponent<CharacterController>();
        //animator = GetComponent<Animator>();
        setPosition = GetComponent <EnemySetPosition> ();
        setPosition.CreateRandomPosition();
        velocity = Vector3.zero;
        arrived = false;
        elapsedTime = 0f;
        SetState("wait");
    }

    void Update()
    {
        //　見回りまたはキャラクターを追いかける状態
        if (state == EnemyState.walk || state == EnemyState.chase)
        {
            if (state == EnemyState.chase)
            {
                setPosition.SetDestination(playerTransform.position);
            }

            if (enemyController.isGrounded)
            {
                velocity = Vector3.zero;
                //animator.SetFloat("Speed", 2.0f);
                direction = (setPosition.GetDestination() - transform.position).normalized;
                transform.LookAt(new Vector3(setPosition.GetDestination().x, transform.position.y, setPosition.GetDestination().z));
                velocity = direction * walkSpeed;
            }

            //　目的地に到着したかどうかの判定
            if (Vector3.Distance(transform.position, setPosition.GetDestination()) < 0.7f)
            {
                SetState("wait");
                //animator.SetFloat("Speed", 0.0f);
            }            
        }
        else if (state == EnemyState.wait)
        {
            elapsedTime += Time.deltaTime;

            //　待ち時間を越えたら次の目的地を設定
            if (elapsedTime > waitTime)
            {
                SetState("walk");
            }
        }
        

        velocity.y += Physics.gravity.y * Time.deltaTime;
        enemyController.Move(velocity * Time.deltaTime);
    }

    public void SetState(string mode, Transform obj = null)
    {
        if(mode == "walk")
        {
            arrived = false;
            elapsedTime = 0f;
            state = EnemyState.walk;
            setPosition.CreateRandomPosition();
        }else if (mode == "chase")
        {
            state = EnemyState.chase;
            //　待機状態から追いかける場合もあるのでOff
            arrived = false;
            //　追いかける対象をセット
            playerTransform = obj;
        }
        else if (mode == "wait")
        {
            elapsedTime = 0f;
            state = EnemyState.wait;
            arrived = true;
            velocity = Vector3.zero;
            //animator.SetFloat("Speed", 0f);
        }
    }

    public EnemyState GetState()
    {
        return state;
    }
}