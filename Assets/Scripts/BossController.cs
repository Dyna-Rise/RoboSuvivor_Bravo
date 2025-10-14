using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class BossController : MonoBehaviour
{
    public int enemyHP = 5; //Hipポイント
    public float enemySpeed = 5.0f;
    bool isSDamege; //ダメージ中のフラグ
    float recoverTime = 0.0f;
    public GameObject body; //点滅されるBody

    Vector3 moveDirection = Vector3.zero;
    CharacterController controller;

    GameObject player; //playerをInspectorから設定
    NavMeshAgent navMeshAgent; //NavMeshAgentコンポーネント

    public float detectionRanege = 80f; //Playerを検知する距離

    bool isAttack; //攻撃中のフラグ
    public float attackRange = 30f; //攻撃可能距離
    public float stopRange = 5f; //近接限界距離

    public GameObject bulletPrefab; //弾のプレハブ
    public GameObject gate; //弾の発射位置
    public float bulletSpeed = 100f;
    public float fireInterval = 2.0f;
    bool lockOn = true; //ターゲットを向くべき

    float timer; //時間経過

    GameObject gameManager; //GameManagerオブジェクト

    Rigidbody rbody; //Rigidbodyコンポーネント
    AudioSource audio; //AudioSourceコンポーネント
    Animator animator; //Animatorコンポーネント

    void Start()
    {
        rbody = GetComponent<Rigidbody>();    // Rigidbodyを得る
        player = GameObject.FindGameObjectWithTag("Player"); //Playerをタグで取得
        controller = GetComponent<CharacterController>();

        audio = GetComponent<AudioSource>();

        animator = GetComponent<Animator>();
        animator.SetBool("Active", true);
    }

    // Update is called once per frame
    void Update()
    {
        //playingモードの場合は何もしない
        if (GameManager.gameState == GameState.playing) return;

        //Playerがいない場合は何もしない
        if (player == null) return;

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
        Vector3 playerpos = player.transform.position;
        Vector3 bosspos = body.transform.position;
        float d = Vector3.Distance(playerpos, bosspos); //PlayerとBossの距離
        Debug.Log("距離" + d);

        if (d > detectionRanege)   //プレイヤーが検知内
        {
            //lockOnがON(true)の時
            if (lockOn)
            {
                //Transform.LookAtでプレイヤーを向く
                body.transform.LookAt(playerpos);
                //playerに向かって歩く

            }

            // Mathf.Atan2(dz, dx) * Mathf.Rad2Deg; //ラジアンを度に変換

            //　→プレイヤーが攻撃範囲内
            //(isAttack)なら何もしない
            //攻撃範囲内では移動はゆっくり　※例えば通常の半分
            //timerで計測し、時間が来たら攻撃コルーチン
            //　→プレイヤーが攻撃範囲外
            //通常のスピードでプレイヤーを追ってくる
            //→プレイヤーが検知外
            //　・NavMeshAgentの動きがとまる

        }

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
        //recoverTimeが作動中かHPが0より大きい場合はisDamageフラグがON
        bool damaged = recoverTime > 0.0f || enemyHP >= 0;

        //isDamageフラグがOFFの場合はPlayerのbodyを確実に表示
        if (!damaged) body.SetActive(true);

        //damagedフラグをリターン
        return damaged;
    }
}