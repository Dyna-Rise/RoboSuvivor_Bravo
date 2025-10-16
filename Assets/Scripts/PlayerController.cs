using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;

    public float moveSpeed = 5.0f; //移動スピード
    public float jumpForce = 8.0f; //ジャンプパワー
    public float gravity = 20.0f; //重力
    float recoverTime = 0.0f;

    Vector3 moveDirection = Vector3.zero; //移動成分

    public GameObject body; //点滅対象
    bool isDamage; //ダメージフラグ

    public static int life = 10;
    

    void Start()
    {
        //各コンポーネントを取得
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
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

        //もし地面に接地していたら
        if (controller.isGrounded)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // ローカル座標系での移動方向
            Vector3 moveDirection = new Vector3(horizontal, 0, vertical);

            // 正規化して速度を一定に保つ
            if (moveDirection.magnitude > 1)
            {
                moveDirection.Normalize();
            }

            // TransformDirectionでローカル座標からワールド座標に変換
            // これにより、プレイヤーの回転が考慮される
            Vector3 worldMoveDirection = transform.TransformDirection(moveDirection);

            // CharacterController.Moveにワールド座標での移動量を渡す
            controller.Move(worldMoveDirection * moveSpeed * Time.deltaTime);

        }

        //もしスタン中なら
        if (IsStun())
        {
            //復活までの時間をカウント
            recoverTime -= Time.deltaTime;

            //点滅処理
            Blinking();

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
    }

    public void MoveToRight()
    {
        if (IsStun()) return;
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

        }
    }

    public int Life()
    {
        return life;
    }

    bool IsStun()
    {
        //recoverTimeが作動中かLifeが0になった場合はStunフラグがON
        bool stun = recoverTime > 0.0f || life <= 0;
        //StunフラグがOFFの場合はボディを確実に表示
        if (!stun) body.SetActive(true);
        //Stunフラグをリターン
        return stun;
    }

    //CharaControllerに衝突判定が生じたときの処理
    private void OnTriggerEnter(Collider hit)
    {
        if (IsStun()) return;

        //ぶつかった相手がEnemyかEnemyBulletなら
        if (hit.gameObject.CompareTag("Enemy") || hit.gameObject.CompareTag("EnemyBullet"))
        {
            Debug.Log("atata");

            //体力をマイナス
            life--;
            recoverTime = 0.5f;

            if (life <= 0)
            {
                GameManager.gameState = GameState.gameover;
                Destroy(gameObject, 0.5f); //少し時間差で自分を消滅
            }
            
        }
    }

    void Blinking()
    {
        //その時のゲーム進行時間で正か負かの値を算出
        float val = Mathf.Sin(Time.time * 50);
        //正の周期なら表示
        if (val >= 0) body.SetActive(true);
        //負の周期なら非表示
        else body.SetActive(false);
    }


}
