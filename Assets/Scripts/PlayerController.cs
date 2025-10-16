using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;

    public float moveSpeed = 5.0f; //�ړ��X�s�[�h
    public float jumpForce = 8.0f; //�W�����v�p���[
    public float gravity = 20.0f; //�d��
    float recoverTime = 0.0f;

    Vector3 moveDirection = Vector3.zero; //�ړ�����

    public GameObject body; //�_�őΏ�
    bool isDamage; //�_���[�W�t���O

    public int life = 10;
    
    //���ɂ܂��R���|�[�l���g��SE�����
    AudioSource audio;
    public AudioClip se_shot;
    public AudioClip se_damage;
    public AudioClip se_jump;
    public AudioClip se_walk;

    
    void Start()
    {
        //�I�[�f�B�I�R���|�[�l���g���擾
        audio = GetComponent<AudioSource>();
        //�e�R���|�[�l���g���擾
        controller = GetComponent<CharacterController>();
        //GameManager.
        

    }

    void Update()
    {
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

        //�����n�ʂɐڒn���Ă�����
        if (controller.isGrounded)
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // ���[�J�����W�n�ł̈ړ�����
            Vector3 moveDirection = new Vector3(horizontal, 0, vertical);

            // ���K�����đ��x�����ɕۂ�
            if (moveDirection.magnitude > 1)
            {
                moveDirection.Normalize();
            }

            // TransformDirection�Ń��[�J�����W���烏�[���h���W�ɕϊ�
            // ����ɂ��A�v���C���[�̉�]���l�������
            Vector3 worldMoveDirection = transform.TransformDirection(moveDirection);

            // CharacterController.Move�Ƀ��[���h���W�ł̈ړ��ʂ�n��
            controller.Move(worldMoveDirection * moveSpeed * Time.deltaTime);

        }

        //�����X�^�����Ȃ�
        if (IsStun())
        {
            //�����܂ł̎��Ԃ��J�E���g
            recoverTime -= Time.deltaTime;

            //�_�ŏ���
            Blinking();

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
        SEPlay(SEType.Walk);
    }

    public void MoveToRight()
    {
        if (IsStun()) return;
        SEPlay(SEType.Walk);
    }

    public void MoveToUp()
    {
        if (IsStun()) return;
        SEPlay(SEType.Walk);

    }

    public void MoveToDown()
    {
        if (IsStun()) return;
        SEPlay(SEType.Walk);

    }

    void Jump()
    {
        if (IsStun()) return;
        if (controller.isGrounded)
        {
            SEPlay(SEType.Jump);
            moveDirection.y = jumpForce;

        }
    }

    public int Life()
    {
        return life;
    }

    bool IsStun()
    {
        //recoverTime���쓮����Life��0�ɂȂ����ꍇ��Stun�t���O��ON
        bool stun = recoverTime > 0.0f || life <= 0;
        //Stun�t���O��OFF�̏ꍇ�̓{�f�B���m���ɕ\��
        if (!stun) body.SetActive(true);
        //Stun�t���O�����^�[��
        return stun;
    }

    //CharaController�ɏՓ˔��肪�������Ƃ��̏���
    private void OnTriggerEnter(Collider hit)
    {
        if (IsStun()) return;

        //�Ԃ��������肪Enemy��EnemyBullet�Ȃ�
        if (hit.gameObject.CompareTag("Enemy") || hit.gameObject.CompareTag("EnemyBullet"))
        {
            Debug.Log("atata");
            SEPlay(SEType.Damage);

            //�̗͂��}�C�i�X
            life--;
            recoverTime = 0.5f;

            if (life <= 0)
            {
                GameManager.gameState = GameState.gameover;
                Destroy(gameObject, 0.5f); //�������ԍ��Ŏ���������
            }
            
        }
    }

    void Blinking()
    {
        //���̎��̃Q�[���i�s���ԂŐ��������̒l���Z�o
        float val = Mathf.Sin(Time.time * 50);
        //���̎����Ȃ�\��
        if (val >= 0) body.SetActive(true);
        //���̎����Ȃ��\��
        else body.SetActive(false);
    }

    //SE�Đ�
    public void SEPlay(SEType type)
    {
        switch (type)
        {
            case SEType.Shot:
                audio.PlayOneShot(se_shot);
                break;
            case SEType.Damage:
                audio.PlayOneShot(se_damage);
                break;
            case SEType.Jump:
                audio.PlayOneShot(se_jump);
                break;
            case SEType.Walk:
                audio.PlayOneShot(se_walk);
                break;
        }
    }


}
