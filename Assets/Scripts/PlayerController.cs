using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    Animator animator;

    public float moveSpeed = 5.0f; //移動スピード
    public float jumpForce = 8.0f; //ジャンプパワー
    public float gravity = 20.0f; //重力
    float recoverTime = 0.0f;

    Vector3 moveDirection = Vector3.zero; //移動成分

    public GameObject body; //点滅対象
    bool isDamage; //ダメージフラグ

    public int life = 10;
    

    void Start()
    {
        //各コンポーネントを取得
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (controller.isGrounded)
        {
            if (Input.GetAxis("Vertical") > 0.0f)
            {
                //水平方向
                moveDirection.z = Input.GetAxis("Vertical") * moveSpeed;

            }
            else
            {
                moveDirection.z = 0;
            }
        }
        
        //垂直方向
        transform.Rotate(0, Input.GetAxis("Horizontal") * 3, 0);

        //もしゲームステータスがPlayingかgameClearじゃないならなにもしない
        if (GameManager.gameState != GameState.playing || GameManager.gameState != GameState.gameclear)
        {
            //return;

            //もし〇キーが押されたら△に動く
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) MoveToLeft();
            if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) MoveToRight();
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) MoveToUp();
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) MoveToDown();
            if (Input.GetKeyDown(KeyCode.Space)) Jump();

        }

        if (IsStun())
        {
            moveDirection.x = 0;
            moveDirection.z = 0;

            //復活までの時間をカウント
            recoverTime -= Time.deltaTime;

            IsDamage();
        }

        //重力分の力を毎フレーム追加
        moveDirection.y -= gravity * Time.deltaTime;

        //移動実行
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        controller.Move(globalDirection * Time.deltaTime);

        //接地していたらYはリセット
        if(controller.isGrounded)moveDirection.y = 0;
        
    }

    public void MoveToLeft()
    {
        if (IsStun()) return;
        if (controller.isGrounded) ;

    }

    public void MoveToRight()
    {
        if (IsStun()) return;
        if (controller.isGrounded) ;

    }

    public void MoveToUp()
    {
        if (IsStun()) return;

    }

    public void MoveToDown()
    {
        if (IsStun()) return;

    }

    void Jump()
    {
        if (IsStun()) return;
        if (controller.isGrounded)
        {
            moveDirection.y = jumpForce;

            //ジャンプトリガーを設定
            animator.SetTrigger("jump");
        }
    }

    bool IsStun()
    {
        return recoverTime > 0.0f || life <= 0;
    }

    
    //CharaControllerに衝突判定が生じたときの処理
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (IsStun()) return;

        //ぶつかった相手がEnemyかEnemyBulletなら
        if (hit.gameObject.CompareTag("Enemy")|| hit.gameObject.CompareTag("EnemyBullet"))
        {
            //体力をマイナス
            life--;
            recoverTime = 0.5f;

            if (life <= 0)
            {
                
                GameManager.gameState = GameState.gameover;
                Destroy(gameObject, 0.5f); //少し時間差で自分を消滅
            }
            //接触したEnemyを削除
            Destroy(hit.gameObject);
        }
    }

    void IsDamage()
    {
        //その時のゲーム進行時間で正か負かの値を算出
        float val = Mathf.Sin(Time.time * 50);
        //正の周期なら表示
        if (val >= 0) body.SetActive(true);
        //負の周期なら非表示
        else body.SetActive(false);
    }
}
