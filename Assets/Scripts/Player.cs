using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Joystick movementJoystick;
    public Button dashButton;
    private bool useJoystick = true;
    public float minX, maxX, minY, maxY;

    public Rigidbody2D rb;
    public SpriteRenderer characterSR;
    public Animator animator;
 
    public GameObject ghostEffect;
    public float ghostDelaySeconds;
    private Coroutine dashEffectCoroutine;

    public float dashBoost;
    private float _dashTime;
    public float dashTime;
    private bool isDashing=false;

    public Vector3 moveInput;

    public GameObject damPopUp;
    public LosePanel losePanel;

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        dashButton.onClick.AddListener(OnDashButtonClick);
    }
    

    // Update is called once per frame
    private void Update()
    {
        if (useJoystick)
        {
            this.MovePlayerMobile();
        }
        //else
        //{
        //    this.MovePlayer();
        //}

    }
    protected void OnDashButtonClick()
    {
        if (_dashTime <= 0 && isDashing == false)
        {
            moveSpeed += dashBoost;
            _dashTime = dashTime;
            isDashing = true;
            StartDashEffect();
        }
    }
    protected void MovePlayerMobile()
    {

        Vector2 direction = movementJoystick.Direction;
  
        if (direction.magnitude > 0)
        {
            rb.velocity = new Vector2(direction.x * moveSpeed, direction.y * moveSpeed);
            animator.SetFloat("Speed", direction.sqrMagnitude);
            if (direction.x != 0)
                if (direction.x < 0)
                    characterSR.transform.localScale = new Vector3(-1, 1, 0);
                else
                    characterSR.transform.localScale = new Vector3(1, 1, 0);
        }
        else
        {
            rb.velocity =Vector2.zero;
            animator.SetFloat("Speed", 0);
        }

        if (_dashTime <= 0 && isDashing == true)
        {
            moveSpeed -= dashBoost;
            isDashing = false;
            StopDashEffect();
        }
        else if (_dashTime > 0)
        {
            _dashTime -= Time.deltaTime;
        }
        Vector2 position = transform.position; // Lấy vị trí 2D của nhân vật (x, y)

       
        position.x = Mathf.Clamp(position.x, minX, maxX); // Giới hạn theo trục X

       
        position.y = Mathf.Clamp(position.y, minY, maxY); // Giới hạn theo trục Y

        
        transform.position = position; // Gán lại vị trí đã điều chỉnh
    }
    //protected void MovePlayer()
    //{
    //    /// Part 2
    //    // Movement
    //    moveInput.x = Input.GetAxisRaw("Horizontal");
    //    moveInput.y = Input.GetAxisRaw("Vertical");
    //    transform.position += moveSpeed * Time.deltaTime * moveInput;
    //    //

    //    animator.SetFloat("Speed", moveInput.sqrMagnitude);


    //    if (Input.GetKeyDown(KeyCode.Space) && _dashTime <= 0 && isDashing==false)
    //    {
    //        //animator.SetBool("Roll", true);
    //        moveSpeed += dashBoost;
    //        _dashTime = dashTime;
    //        isDashing = true;
    //        StartDashEffect();
    //    }

    //    if (_dashTime <= 0 && isDashing==true)
    //    {
    //        //animator.SetBool("Roll", false);
    //        moveSpeed -= dashBoost;
    //        isDashing = false;
    //        StopDashEffect();
    //    }
    //    else
    //    {
    //        _dashTime -= Time.deltaTime;
    //    }

    //    // Rotate Face
    //    if (moveInput.x != 0)
    //        if (moveInput.x < 0)
    //            characterSR.transform.localScale = new Vector3(-1, 1, 0);
    //        else
    //            characterSR.transform.localScale = new Vector3(1, 1, 0);
    //}

    public void TakeDamageEffect(int damage)
    {
        if (damPopUp != null)
        {
            GameObject instance = Instantiate(damPopUp, transform.position
                    + new Vector3(UnityEngine.Random.Range(-0.3f, 0.3f), 0.5f, 0), Quaternion.identity);
            instance.GetComponentInChildren<TextMeshProUGUI>().text = damage.ToString();
            Animator animator = instance.GetComponentInChildren<Animator>();
            animator.Play("red");
        }
        if (GetComponent<Health>().isDead)
        {
            losePanel.Show();
        }
    }

    protected void StopDashEffect()
    {
        if (dashEffectCoroutine != null) StopCoroutine(dashEffectCoroutine);
    }

    protected void StartDashEffect()
    {
        if (dashEffectCoroutine != null) StopCoroutine(dashEffectCoroutine);
        dashEffectCoroutine = StartCoroutine(DashEffectCoroutine());
    }

    IEnumerator DashEffectCoroutine()
    {
        while (true)
        {
            GameObject ghost = Instantiate(ghostEffect, transform.position, transform.rotation);
            Sprite currentSprite = characterSR.sprite;
            ghost.GetComponentInChildren<SpriteRenderer>().sprite = currentSprite;
            Destroy(ghost, 0.5f);
            yield return new WaitForSeconds(ghostDelaySeconds);
        }
    }
}
