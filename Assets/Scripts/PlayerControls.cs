using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    private bool isMoving;
    private Vector2 input;
    private bool right = true;
    private Animator animator;

    public LayerMask collisions;
    public LayerMask interact;
    public LayerMask deathzone;
    public bool canMove = true;

    public static event Action OnDeath;
    public static event Action OnDanger;
    public GameObject danger;

    public float comboResetTime = 0.6f;
    private int comboStep = 0;
    private float lastAttackTime = 0f;

    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioSource dangerSource;
    public AudioClip walking;
    public AudioClip dangerSound;
    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void HandleUpdate()
    {
        if (!isMoving && canMove)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input != Vector2.zero)
            {
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;
                soundSource.clip = walking;

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
            soundSource.Play();

            if (input.x > 0 && right)
            {
                Flip();
            }
            if (input.x < 0 && !right)
            {
                Flip();
            }
        }
        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.F))
        {
            Interact();
        }

        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
        }
        if (Input.GetMouseButtonDown(0))
        {
            handleStrike();
        }
    }
    void Interact()
    {
        var interactPos = transform.position;
        var collider = Physics2D.OverlapCircle(interactPos, 0.9f, interact);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }
    void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        right = !right;
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;
        animator.SetFloat("xVelocity", input.x);
        animator.SetFloat("yVelocity", input.y);
        isMoving = false;
    }

    void handleStrike()
    {
        lastAttackTime = Time.time;
        comboStep++;

        if (comboStep == 1)
        {
            animator.SetTrigger("strike1");
        }
        else if (comboStep == 2)
        {
            animator.SetTrigger("strike2");
        }
        else
        {
            comboStep = 1;
            animator.SetTrigger("strike1");
        }
    }
    public bool isCountingDown = false;
    public int duration = 4;
    public int timeRemaining;

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, collisions | interact) != null)
        {
            return false;
        }
        if (Physics2D.OverlapCircle(targetPos, 0.2f, deathzone) != null)
        {
            Deathcount();
        }
        else
        {
            isCountingDown = false;
            duration = 4;
            danger.SetActive(false);
            dangerSource.mute = true;
        }
        return true;
    }

    void Deathcount()
    {
        if (!isCountingDown)
        {
            isCountingDown = true;
            timeRemaining = duration;
            Invoke("_tick", 1f);
            OnDanger?.Invoke();
            dangerSource.Play();
            dangerSource.mute = false;
        }
    }
    private void _tick()
    {
        timeRemaining--;
        if (timeRemaining > 0 && isCountingDown == true)
        {
            Invoke("_tick", 1f);
            Debug.Log(timeRemaining);
        }
        else if (isCountingDown == true)
        {
            OnDeath?.Invoke();
            isCountingDown = false;
            dangerSource.mute = true;
        }
    }

}