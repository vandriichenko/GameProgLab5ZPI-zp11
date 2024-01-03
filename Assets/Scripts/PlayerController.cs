using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float Speed = 5;
    public float JumpSpeed = 7;

    public GameObject WinnerText;

    private Rigidbody2D body;
    private Animator animator;

    private bool isLeftMovement;

    private bool isRunning;
    private bool isJumping;

    private bool isGameovered;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        var horizontalMovement = Input.GetAxisRaw("Horizontal");
        if (horizontalMovement != 0 )
        {
            isRunning = true;
            body.position += new Vector2(horizontalMovement * Speed * Time.deltaTime, 0);
            if (horizontalMovement > 0 && isLeftMovement ||
                horizontalMovement < 0 && !isLeftMovement)
            {
                var scale = transform.localScale;
                    scale.x *= -1;
                transform.localScale = scale;
                isLeftMovement = !isLeftMovement;
            }
        }
        else
        {
            isRunning = false;
        }

        if(Input.GetKeyDown(KeyCode.UpArrow) && IsGrounded())
        {
            isJumping = true;
            body.AddForce(new Vector2(0, JumpSpeed), ForceMode2D.Impulse);
        }

        if(transform.position.y < -4)
        {
            GameOver();
        }

        SwitchAnimation();
    }

    private void SwitchAnimation()
    {
        if (isJumping)
        {
            if(body.velocity.y > 0) 
            {
                animator.SetTrigger("ToUp");
            }
            else 
            {
                animator.SetTrigger("ToDown");
            }
        }
        else if (isRunning) 
        {
            animator.SetTrigger("ToRun");
        }
        else
        {
            animator.SetTrigger("ToIdle");
        }
    }

    private bool IsGrounded()
    {
        var raycast = Physics2D.Raycast(transform.position - transform.localScale / 2, Vector2.down, 0.1f);

        return raycast.collider != null;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        isJumping = false;

        if(collision.gameObject.tag == "Enemy")
        {
            GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameWin();
    }

    private void GameOver()
    {
        if (isGameovered)
        { 
            return; 
        }

        isGameovered = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GameWin()
    {
        WinnerText.SetActive(true);
        Destroy(gameObject, 3);
    }

    private void OnDestroy()
    {
        GameOver();
    }
}
