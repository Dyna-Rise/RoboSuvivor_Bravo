using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Animation()
    {
        if (axisH != 0 || axisV != 0)
        {

            //�ЂƂ܂�Run�A�j���𑖂点��
            anime.SetBool("run", true);

            //angleZ�𗘗p���ĕ��p�����߂�@�p�����[�^direction int�^
            //int�^��direction ���F0�@��F1�@�E�F2�@���F����ȊO

            if (angleZ > -135f && angleZ < -45f) //������
            {
                anime.SetInteger("direction", 0);
            }
            else if (angleZ >= -45f && angleZ <= 45f) //�E����
            {
                anime.SetInteger("direction", 2);
                transform.localScale = new Vector2(1, 1);
            }
            else if (angleZ > 45f && angleZ < 135f) //�����
            {
                anime.SetInteger("direction", 1);
            }
            else //������
            {
                anime.SetInteger("direction", 3);
                transform.localScale = new Vector2(-1, 1);
            }
        }
        else //�������͂��Ȃ��ꍇ
        {
            anime.SetBool("run", false); //����t���O��OFF
        }
    }

}
