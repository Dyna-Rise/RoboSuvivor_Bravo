using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    Animator animator;

    public float moveSpeed = 5.0f; //�ړ��X�s�[�h
    public float jumpForce = 8.0f; //�W�����v�p���[
    public float gravity = 20.0f; //�d��
    float recoverTime = 0.0f;

    Vector3 moveDirection = Vector3.zero; //�ړ�����

    public GameObject body; //�_�őΏ�
    bool isDamage; //�_���[�W�t���O

    public int life = 10;
    

    void Start()
    {
        //�e�R���|�[�l���g���擾
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (controller.isGrounded)
        {
            if (Input.GetAxis("Vertical") > 0.0f)
            {
                //��������
                moveDirection.z = Input.GetAxis("Vertical") * moveSpeed;

            }
            else
            {
                moveDirection.z = 0;
            }
        }
        
        //��������
        transform.Rotate(0, Input.GetAxis("Horizontal") * 3, 0);

        //�����Q�[���X�e�[�^�X��Playing��gameClear����Ȃ��Ȃ�Ȃɂ����Ȃ�
        if (GameManager.gameState != GameState.playing || GameManager.gameState != GameState.gameclear)
        {
            //return;

            //�����Z�L�[�������ꂽ�灢�ɓ���
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

            //�����܂ł̎��Ԃ��J�E���g
            recoverTime -= Time.deltaTime;

            IsDamage();
        }

        //�d�͕��̗͂𖈃t���[���ǉ�
        moveDirection.y -= gravity * Time.deltaTime;

        //�ړ����s
        Vector3 globalDirection = transform.TransformDirection(moveDirection);
        controller.Move(globalDirection * Time.deltaTime);

        //�ڒn���Ă�����Y�̓��Z�b�g
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

            //�W�����v�g���K�[��ݒ�
            animator.SetTrigger("jump");
        }
    }

    bool IsStun()
    {
        return recoverTime > 0.0f || life <= 0;
    }

    
    //CharaController�ɏՓ˔��肪�������Ƃ��̏���
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (IsStun()) return;

        //�Ԃ��������肪Enemy��EnemyBullet�Ȃ�
        if (hit.gameObject.CompareTag("Enemy")|| hit.gameObject.CompareTag("EnemyBullet"))
        {
            //�̗͂��}�C�i�X
            life--;
            recoverTime = 0.5f;

            if (life <= 0)
            {
                
                GameManager.gameState = GameState.gameover;
                Destroy(gameObject, 0.5f); //�������ԍ��Ŏ���������
            }
            //�ڐG����Enemy���폜
            Destroy(hit.gameObject);
        }
    }

    void IsDamage()
    {
        //���̎��̃Q�[���i�s���ԂŐ��������̒l���Z�o
        float val = Mathf.Sin(Time.time * 50);
        //���̎����Ȃ�\��
        if (val >= 0) body.SetActive(true);
        //���̎����Ȃ��\��
        else body.SetActive(false);
    }
}
