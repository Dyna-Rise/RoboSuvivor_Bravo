using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    public int enemyHP = 5; //Hipポイント
    public float enemySpeed = 5.0f;
    public float times; //時間
    float movep = 0; //移動補完値（進捗率）
    bool isSDamege; //ダメージ中のフラグ
    float recoverTime = 0.0f;
    public GameObject body; //点滅されるBody
    public GameObject gate;

    Vector3 moveDirection = Vector3.zero;
    CharacterController controller;

    GameObject player; //playerをInspectorから設定
    NavMeshAgent agent; //NavMeshAgentコンポーネント

    public float detectionRanege = 80f; //Playerを検知する距離

    bool isAttack; //攻撃中のフラグ
    public float attackRange = 30f; //攻撃可能距離
    public float stopRange = 5f; //近接限界距離

    public GameObject bulletPrefab; //弾のプレハブ
    //public GameObject gate; //弾の発射位置
    public float bulletSpeed = 100f;
    public float fireInterval = 2.0f;
    bool lockOn = true; //ターゲットを向くかどうかの判定

    float timer; //時間経過

    GameObject gameManager; //GameManagerオブジェクト

    Rigidbody rbody; //Rigidbodyコンポーネント
    AudioSource audio; //AudioSourceコンポーネント
    //Animator animator; //Animatorコンポーネント

    void Start()
    {
        rbody = GetComponent<Rigidbody>();    // Rigidbodyを得る
        player = GameObject.FindGameObjectWithTag("Player"); //Playerをタグで取得
        controller = GetComponent<CharacterController>();

        audio = GetComponent<AudioSource>();

        //animator = GetComponent<Animator>();
        //animator.SetBool("Active", true);
    }

    // Update is called once per frame
    void Update()
    {
        //playingモードの場合は何もしない
        if (GameManager.gameState == GameState.playing) return;

        //PlayerまたはBossがいない場合は何もしない
        if (player == null || body == null) return;

        //enemyHPが0以下なら何もしない
        if (enemyHP <= 0) return;

        //isDamageがONなら点滅処理(BattleCart参照)
        if (isSDamege)
        {
            moveDirection.x = 0;
            moveDirection.z = 0;
            recoverTime -= Time.deltaTime; //時間を減らす

            Blinking();
        }

        //プレイヤーとの距離をつねに測る
        Vector3 playerPos = player.transform.position;
        Vector3 bossPos = transform.position;
        float dist = Vector3.Distance(playerPos, bossPos); //PlayerとBossの距離計測
        Debug.Log("距離" + dist);

        float ds = dist / times; //1秒あたりの移動距離
        float df = ds * Time.deltaTime; //1フレームあたりの移動距離
        movep += df / dist; //終点までの進捗率を更新（0～1.0f）



        if (lockOn) //lockOnがON(true)の時
        {
            //Transform.LookAtでプレイヤーを向く
            body.transform.LookAt(playerPos);

            if (dist <= detectionRanege)   //プレイヤーが検知内
            {
                //playerに向かって歩く
                //Lerp用移動
                transform.position = Vector3.Lerp(bossPos, playerPos, movep);
            }

            if (dist <= attackRange) //プレイヤーが攻撃範囲内
            {
                //playerに向かって歩く
                transform.position = Vector3.Lerp(bossPos, playerPos, movep);

                Shot(); //攻撃メソッド
            }
                
            void Shot()
            {
                if (!isAttack) return; 
                isAttack = true; //攻撃中フラグを立てる
                // Gateの回転にX軸90度だけ回転
                Quaternion bulletRotation = gate.transform.rotation * Quaternion.Euler(90, 0, 0);
                GameObject obj = Instantiate(
                    bulletPrefab,
                    gate.transform.position,
                    bulletRotation);

                //弾のRigidbodyを取得
                Rigidbody rbody = obj.GetComponent<Rigidbody>();
                // 計算した方向にショット
                rbody.AddForce(gate.transform.forward * bulletSpeed, ForceMode.Impulse);

                StartCoroutine(FireInterval()); //一定時間待つコルーチン
                Invoke("AttackOff", timer); //一定時間待ってから攻撃フラグ解除
            }


            //残数の回復コルーチン
            IEnumerator FireInterval()
            {
                //一定時間待つ
                yield return new WaitForSeconds(fireInterval);
            }

             //攻撃フラグ解除
            void AttackOFF()
            {
                isAttack = false; //攻撃フラグ解除
            }
        }
       
       
            // Mathf.Atan2(dz, dx) * Mathf.Rad2Deg; //ラジアンを度に変換
            //(isAttack)なら何もしない
            //攻撃範囲内では移動はゆっくり　※例えば通常の半分
            //timerで計測し、時間が来たら攻撃コルーチン
            //　→プレイヤーが攻撃範囲外
            //通常のスピードでプレイヤーを追ってくる
            //→プレイヤーが検知外
            //　・NavMeshAgentの動きがとまる
    }
    //点滅処理
    void Blinking()
    {
        //正負をSin関数で算出
        float val = Mathf.Sin(Time.deltaTime * 50);

        //正の周期なら表示、負の周期なら非表示
        if (val >= 0) body.SetActive(true);
        else body.SetActive(false);
    }

    bool isDamage()
    {
        //recoverTimeがHPが0より大きい場合はisDamageフラグがON
        bool damaged = recoverTime > 0.0f || enemyHP >= 0;

        //isDamageフラグがOFFの場合はPlayerのbodyを確実に表示
        if (!damaged) body.SetActive(true);

        //damagedフラグをリターン
        return damaged;
    }
}