using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator anime;

    float axisH;
    float axisV;

    void Start()
    {
        
    }

    void Update()
    {
        if (axisH != 0 || axisV != 0)
        {
            anime.SetBool("walk", true);

            if (Input.GetAxisRaw("Horizontal") > 0)
            {
                anime.SetInteger("direction", 3);
            }
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                anime.SetInteger("direction", 1);
            }
            if (Input.GetAxisRaw("Vertical") > 0)
            {
                anime.SetInteger("direction", 0);
            }
            if (Input.GetAxisRaw("Vertical") < 0)
            {
                anime.SetInteger("direction", 2);
            }
            
        }
        else
        {
            anime.SetBool("walk", false);
        }

        //スペースキーが押されたら
        if (Input.GetButtonDown("Jump"))
        {
            Debug.Log("janp");
            anime.SetTrigger("Jump");
        }
        //左クリックされたらセットトリガーショット
        //ゲームステータスがゲームオーバーになったらdie

    }
}
