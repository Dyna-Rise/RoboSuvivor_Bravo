using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{

    public int enemyHP = 5; //�G��HP
    public float enemySpeed = 5.0f; //�G�̃X�s�[�h
    bool isDamage; //�_���[�W���t���O

    public GameObject body; //�_�ł����body

    GameObject player;      // �v���C���[��Transform��Inspector����ݒ�   [SerializeField] �Ƃ�public�ł��Ă��Ƃł����H
    NavMeshAgent navMeshAgent;     // NavMeshAgent�R���|�[�l���X

    public float detectionRange = 80f;     // �v���C���[�����m���鋗��

    bool isAttack; //�U�����t���O
    public float attackRange = 30f;         // �U�����J�n���鋗��
    public float stopRange = 5f; //�ڋߌ��E����
    public GameObject bulletPrefab;     // ���˂���e��Prefab
    public GameObject gate;            // �e�𔭎˂���ʒu
    public float bulletSpeed = 100f;    // ���˂���e�̑��x 
    public float fireInterval = 2.0f; //�e�𔭎˂���C���^�[�o��
    bool lockOn = true; //�^�[�Q�b�g

    float timer; //���Ԍo��



    GameObject gameMgr; //�Q�[���}�l�[�W���[using UnityEngine;
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


        //playing�Ȃ牽�����Ȃ��܂���player�����Ȃ��Ȃ�
        if (GameManager.gameState != GameState.playing && player == null)
        {
            return;
        }

        //��ɂ��̃I�u�W�F�N�g�ƃv���C���[�I�u�W�F�N�g�̋����𑪂�
        float playerDistance = Vector3.Distance(player.transform.position, transform.position);

        //�v���C���[�̋������v���C���[�����m���鋗�����傫���Ȃ�
        if (playerDistance >= detectionRange)
        {
            navMeshAgent.isStopped = true;//�~�߂�
            lockOn = false; //�v���C���[�̕��������t���O��OFF
        }
        //�v���C���[�̋������v���C���[�����m���鋗������������
        else
        {
            navMeshAgent.SetDestination(player.transform.position);//player��ǂ�
            navMeshAgent.isStopped = false;//�~�߂�t���OOFF
            lockOn = true;//�v���C���[�̕��������t���O��ON

            //�U����������������
            if (playerDistance <= attackRange)
            {
                //�A�^�b�N�R���[�`������
                StartCoroutine(Ataack());

                //�����v���C���[�̋������ڋߌ��E������菬�����Ȃ�
                if (playerDistance <= stopRange)
                {
                    navMeshAgent.isStopped = true;//�~�߂�
                }
                else navMeshAgent.isStopped = true;//�~�߂�


            }
        }

        //lookOn��true�Ȃ�
        if(lockOn)
        {
            transform.LookAt(player.transform);//player�̕�������
        }

        if (enemyHP == 0)
        {

            //���񂾂�EnemyList���폜   
            gameManager.enemyList.RemoveAt(0);
            //�������j��
            Destroy(gameObject);

        }
        if (isDamage)
        {
            Blinking();//�_�ŏ���
        }



    }

    IEnumerator Ataack()
    {
        isAttack = true;
        lockOn = false ;
        bulletPrefab = Instantiate(bulletPrefab,gate.transform.position,Quaternion.Euler(90,0,0));
        //bulletPrefab.GetComponent<Rigidbody>().AddForce()   �@�@�@�@ �����������灙


        Debug.Log("�U��������");

        yield break;
    }


    //�U����H�������(��UtriggerEnter)
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

        body.SetActive(true);//�Ō�͕\������

        isDamage = false;//OFF
    }

    //�_�ŏ���
    void Blinking()
    {
        float val = Mathf.Sin(Time.time * 50);

        if (val >= 0) body.SetActive(true);

        else body.SetActive(false);

    }
}
