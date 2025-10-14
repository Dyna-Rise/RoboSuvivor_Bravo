using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{

    public int enemyHP = 5; //敵のHP
    public float enemySpeed = 5.0f; //敵のスピード
    bool isDamage; //ダメージ中フラグ

    public GameObject body; //点滅されるbody

    GameObject player;      // プレイヤーのTransformをInspectorから設定   [SerializeField] とかpublicでってことですか？
    NavMeshAgent navMeshAgent;     // NavMeshAgentコンポーネンス

    public float detectionRange = 80f;     // プレイヤーを検知する距離

    bool isAttack; //攻撃中フラグ
    public float attackRange = 30f;         // 攻撃を開始する距離
    public float stopRange = 5f; //接近限界距離
    public GameObject bulletPrefab;     // 発射する弾のPrefab
    public GameObject gate;            // 弾を発射する位置
    public float bulletSpeed = 100f;    // 発射する弾の速度 
    public float fireInterval = 2.0f; //弾を発射するインターバル
    bool lockOn = true; //ターゲット

    float timer; //時間経過



    GameObject gameMgr; //ゲームマネージャーusing UnityEngine;
    GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameMgr = GameObject.FindGameObjectWithTag("GM");
        gameManager = gameMgr.GetComponent<GameManager>();
        navMeshAgent = gameMgr.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {


        //playingなら何もしないまたはplayerがいないなら
        if (GameManager.gameState != GameState.playing && player == null)
        {
            return;
        }

        //常にこのオブジェクトとプレイヤーオブジェクトの距離を測る
        float playerDistance = Vector3.Distance(player.transform.position, transform.position);

        //プレイヤーの距離がプレイヤーを検知する距離より大きいなら
        if (playerDistance >= detectionRange)
        {
            navMeshAgent.isStopped = true;//止める
            lockOn = false; //プレイヤーの方を向くフラグをOFF
        }
        //プレイヤーの距離がプレイヤーを検知する距離内だったら
        else
        {
            navMeshAgent.SetDestination(player.transform.position);//playerを追う
            navMeshAgent.isStopped = false;//止めるフラグOFF
            lockOn = true;//プレイヤーの方を向くフラグをON

            //攻撃距離内だったら
            if (playerDistance <= attackRange)
            {
                //アタックコルーチン発動
                StartCoroutine(Ataack());

                //もしプレイヤーの距離が接近限界距離より小さいなら
                if (playerDistance <= stopRange)
                {
                    navMeshAgent.isStopped = true;//止める
                }
                else navMeshAgent.isStopped = true;//止める


            }
        }

        //lookOnがtrueなら
        if(lockOn)
        {
            transform.LookAt(player.transform);//playerの方を向く
        }

        if (enemyHP == 0)
        {

            //死んだらEnemyListを削除   
            gameManager.enemyList.RemoveAt(0);
            //自分も破壊
            Destroy(gameObject);

        }
        if (isDamage)
        {
            Blinking();//点滅処理
        }



    }

    IEnumerator Ataack()
    {
        isAttack = true;
        lockOn = false ;
        bulletPrefab = Instantiate(bulletPrefab,gate.transform.position,Quaternion.Euler(90,0,0));
        //bulletPrefab.GetComponent<Rigidbody>().AddForce()   　　　　 ☆次ここから☆


        Debug.Log("攻撃中だよ");

        yield break;
    }


    //攻撃を食らったら(一旦triggerEnter)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            enemyHP--;

            isDamage = true;

            Invoke("IsDamage", 1f);


        }
        if (collision.gameObject.CompareTag("PlayerSword"))
        {
            enemyHP = enemyHP - 3;
        }
    }

    void IsDamage()
    {

        body.SetActive(true);//最後は表示する

        isDamage = false;//OFF
    }

    //点滅処理
    void Blinking()
    {
        float val = Mathf.Sin(Time.time * 50);

        if (val >= 0) body.SetActive(true);

        else body.SetActive(false);

    }
}
