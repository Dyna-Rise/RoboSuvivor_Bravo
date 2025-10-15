using System.Collections;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{

    public int enemyHP = 5; //�G��HP
    public float enemySpeed = 5.0f; //�G�̃X�s�[�h
    bool isDamage; //�_���[�W���t���O

    public GameObject body; //�_�ł����body

    // [SerializeField] Transform self;
    GameObject player;
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

    float recoverTime = 2f;
    



    GameObject gameMgr; //�Q�[���}�l�[�W���[using UnityEngine;
    GameManager gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameMgr = GameObject.FindGameObjectWithTag("GM");
        gameManager = gameMgr.GetComponent<GameManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player"); //��������邽�т�player��transform���擾

    }

    // Update is called once per frame
    void Update()
    {


        //playing�ȊO�Ȃ牽�����Ȃ��܂���player�����Ȃ��Ȃ�
        if (GameManager.gameState != GameState.playing || player == null)
        {
            //�r����player�������Ă��~�܂�Ȃ��G�G
            return;
        }
        
        timer += Time.deltaTime;

        //���Player�̈ʒu���擾
        Vector3 playerPos = player.transform.position;
        //��ɂ��̃I�u�W�F�N�g�ƃv���C���[�I�u�W�F�N�g�̋����𑪂�
        float playerDistance = Vector3.Distance(playerPos, transform.position);
       
        //�v���C���[�̋������v���C���[�����m���鋗�����傫���Ȃ�
        if (playerDistance >= detectionRange)
        {
            navMeshAgent.isStopped = true;//�~�߂�
            lockOn = false; //�v���C���[�̕��������t���O��OFF
        }
        //�v���C���[�̋������v���C���[�����m���鋗������������
        else if (playerDistance <= detectionRange)
        {
            navMeshAgent.SetDestination(playerPos);//player��ǂ�
            navMeshAgent.isStopped = false;//�~�߂�t���OOFF
            lockOn = true;//�v���C���[�̕��������t���O��ON
            

            //�U����������������
            if (playerDistance <= attackRange)
            {
                //�������������
                navMeshAgent.speed = 2.5f;

                if(timer >= fireInterval)
                {
                    if (!isAttack)
                    {

                        //�A�^�b�N�R���[�`������
                        StartCoroutine(Ataack());
                    }
                }

               

                //�����v���C���[�̋������ڋߌ��E������菬�����Ȃ�
                if (playerDistance <= stopRange)
                {
                    navMeshAgent.isStopped = true;//�~�߂�
                }
                


            }
            else
            {
                navMeshAgent.speed = 5;//speed��ʏ�X�s�[�h��(5�j
            }

        }

        //lookOn��true�Ȃ�
        if (lockOn)
        {
            transform.LookAt(playerPos);//player�̕�������
        }

        if (enemyHP <= 0)
        {

            //���񂾂�EnemyList���폜   
            //gameManager.enemyList.RemoveAt(0);
            //�������j��
            Destroy(gameObject);

        }

        if (IsDamage())//�o���Ă����BIsDamage���\�b�h�������Ă��炻��̒l��ture�Ȃ�@�o���Ă����B
        {
            recoverTime -= Time.deltaTime;//recoverTime��0�ɂȂ�܂Ōo�ߎ��Ԃ�����
            Blinking();//�_�ŏ���  
        }

        

    }

    IEnumerator Ataack()
    {
        isAttack = true;
        lockOn = false;
        GameObject obj = Instantiate(bulletPrefab, gate.transform.position,gate.transform.rotation * Quaternion.Euler(90, 0, 0));

        

        obj.GetComponent<Rigidbody>().AddForce(gate.transform.forward * bulletSpeed, ForceMode.Impulse); �@�@�@�@
        timer = 0;

        //Debug.Log("�U��������");
       //yield return new WaitForSeconds(1);
        isAttack = false;
        lockOn = true; 
        yield break;
    }


    //�U����H�������(��UtriggerEnter)
    private void OnTriggerEnter (Collider collision)
    {
        if (collision.gameObject.CompareTag("PlayerBullet"))
        {
            enemyHP--;

            recoverTime = 0.5f;//����������

            

            Debug.Log(enemyHP);


        }
        if (collision.gameObject.CompareTag("PlayerSword"))
        {
            enemyHP = enemyHP - 3;
        }
        
    }

    //recover��0���傫���Ȃ����u�Ԃ�true�ɂȂ��āAUpdata�Ōo�ߎ��Ԉ�����0�ɂȂ����u�Ԃ�false�ɂ��郁�\�b�h
    //���̎��ԍ��œ_�ŏ�����������
    bool IsDamage()�@
    {
        bool isDamage = recoverTime > 0; //recoverTime��0�ȏゾ������

        body.SetActive(true);//�Ō�͕\������

        

        return isDamage;
    }

    //�_�ŏ���
    void Blinking()
    {
        float val = Mathf.Sin(Time.time * 50);

        if (val >= 0) body.SetActive(true);

        else body.SetActive(false);

    }
}
