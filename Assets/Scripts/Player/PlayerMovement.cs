using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;
    private Vector3 movement;
    [SerializeField]
    private float jumpPower = 5f;
    [SerializeField]
    private Transform charaRay;
    [SerializeField]
    private float charaRayRange = 0.2f;
    private bool isGround;
    private Vector3 input;
    [SerializeField]
    private float speed = 6f;
    [SerializeField]
    private Rigidbody playerRigidbody;
    private bool isGroundCollider = false;

    [SerializeField]
    private Transform stepRay;
    [SerializeField]
    private float stepDistance = 0.5f;
    [SerializeField]
    private float stepOffset = 0.3f;
    [SerializeField]
    private float slopeLimit = 65f;
    //　昇れる段差の位置から飛ばすレイの距離
    [SerializeField]
    private float slopeDistance = 1f;
    // ヒットした情報を入れる場所
    private RaycastHit stepHit;

    int floorMask;
    float camRayLength = 100f;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        movement = Vector3.zero;
        isGround = false;
        playerRigidbody = GetComponent<Rigidbody>();

        floorMask = LayerMask.GetMask("Floor");
    }

    private void Update()
    {
        //　キャラクターが接地していない時はレイを飛ばして確認
        if (!isGroundCollider)
        {
            if (Physics.Linecast(charaRay.position, (charaRay.position - transform.up * charaRayRange)))
            {
                isGround = true;
                playerRigidbody.useGravity = true;
            }
            else
            {
                isGround = false;
                playerRigidbody.useGravity = false;
            }
            Debug.DrawLine(charaRay.position, (charaRay.position - transform.up * charaRayRange), Color.red);
            //			Debug.Log(ground);
        }

        //　キャラクターコライダが接地、またはレイが地面に到達している場合
        if (isGroundCollider || isGround)
        {
            //　地面に接地してる時は初期化
            if (isGroundCollider)
            {
                movement = Vector3.zero;

                ////　着地していたらアニメーションパラメータと２段階ジャンプフラグをfalse
                //anim.SetBool("Jump", false);
                playerRigidbody.useGravity = true;

                //　レイを飛ばして接地確認の場合は重力だけは働かせておく、前後左右は初期化
            }
            else
            {
                movement = new Vector3(0f, movement.y, 0f);
            }

            input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

            //　方向キーが多少押されている
            if (input.magnitude > 0f)
            {
                //anim.SetFloat("IsWalking", input.x != 0f);

                transform.LookAt(transform.position + input);

                //　昇れる段差を表示
                Debug.DrawLine(transform.position + new Vector3(0f, stepOffset, 0f), 
                    transform.position + new Vector3(0f, stepOffset, 0f) + transform.forward * slopeDistance, Color.green);

                //　ステップ用のレイが地面に接触しているかどうか
                if (Physics.Linecast(stepRay.position, stepRay.position + stepRay.forward * stepDistance,
                    out stepHit, LayerMask.GetMask("Floor")))
                {

                    //　進行方向の地面の角度が指定以下、または昇れる段差より下だった場合の移動処理

                    if (Vector3.Angle(transform.up, stepHit.normal) <= slopeLimit
                        || (Vector3.Angle(transform.up, stepHit.normal) > slopeLimit
                            && !Physics.Linecast(transform.position + new Vector3(0f, stepOffset, 0f),
                            transform.position + new Vector3(0f, stepOffset, 0f) + transform.forward * slopeDistance, LayerMask.GetMask("Floor")))
                    )
                    {
                        movement = new Vector3(0f, ((Quaternion.FromToRotation(Vector3.up, stepHit.normal) * transform.forward) * speed).y, 0f) + transform.forward * speed;
                        Debug.Log(Vector3.Angle(transform.up, stepHit.normal));
                    }
                    else
                    {
                        movement += transform.forward * speed;
                    }

                    Debug.Log(Vector3.Angle(Vector3.up, stepHit.normal));

                    //　ステップ用のレイが地面に接触していなければ
                }
                else
                {
                    movement = transform.forward * speed + new Vector3(0f, movement.y, 0f);
                }
            }
            else
            {
                //anim.SetBool("IsWalking", walking);
            }

            //　ジャンプ
            if (Input.GetButtonDown("Jump")
                //&& !anim.GetCurrentAnimatorStateInfo(0).IsName("Jump")
                //&& !anim.IsInTransition(0)      //　遷移途中にジャンプさせない条件
                )
            {
                //anim.SetBool("Jump", true);
                movement.y += jumpPower;
                playerRigidbody.useGravity = false;
            }
        }

        Turning();

        if (!isGroundCollider && !isGround)
        {
            movement.y += Physics.gravity.y * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        //　キャラクターを移動させる処理
        playerRigidbody.MovePosition(transform.position + movement * Time.deltaTime);
    }

    void Turning()
    {
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit floorHit;

        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
        {
            Vector3 playerToMouse = floorHit.point - transform.position;
            //playerToMouse.y = 0f;

            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
            playerRigidbody.MoveRotation(newRotation);
        }
    }

    void OnCollisionEnter()
    {
        Debug.DrawLine(charaRay.position, charaRay.position + Vector3.down, Color.blue);

        //　他のコライダと接触している時は下向きにレイを飛ばしFloorレイヤーの時だけ接地とする
        if (Physics.Linecast(charaRay.position, charaRay.position + Vector3.down, LayerMask.GetMask("Floor")))
        {
            isGroundCollider = true;
        }
        else
        {
            isGroundCollider = false;
        }
    }

    //　接触していなければ空中に浮いている状態
    private void OnCollisionExit()
    {
        isGroundCollider = false;
    }
}


//using UnityEngine;

//public class PlayerMovement : MonoBehaviour
//{
//    public float speed = 6f;

//    Vector3 movement;
//    Animator anim;
//    Rigidbody playerRigidbody;
//    int floorMask;
//    float camRayLength = 100f;

//    private void Awake()
//    {
//        floorMask = LayerMask.GetMask("Floor");
//        anim = GetComponent<Animator>();
//        playerRigidbody = GetComponent<Rigidbody>();
//    }

//    private void FixedUpdate()
//    {
//        float h = Input.GetAxisRaw("Horizontal");
//        float v = Input.GetAxisRaw("Vertical");

//        Move(h, v);
//        Turning();
//        Animating(h, v);
//    }

//    private void Move(float h, float v)
//    {
//        movement.Set(h, 0f, v);

//        movement = movement.normalized * speed * Time.deltaTime;

//        playerRigidbody.MovePosition(transform.position + movement);
//    }

//    void Turning()
//    {
//        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

//        RaycastHit floorHit;

//        if (Physics.Raycast(camRay, out floorHit, camRayLength, floorMask))
//        {
//            Vector3 playerToMouse = floorHit.point - transform.position;
//            playerToMouse.y = 0f;

//            Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
//            playerRigidbody.MoveRotation(newRotation);
//        }
//    }

//    void Animating(float h, float v)
//    {
//        bool walking = h != 0f || v != 0f;
//        anim.SetBool("IsWalking", walking);
//    }

//    private void OnCollisionStay(Collision collision)
//    {
//        // 衝突した面の、接触した点における法線を取得
//        movement = collision.contacts[0].normal;
//    }
//}
