using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class AlarmClockBomb : MonoBehaviour
{
    [Header("Bomb Settings")]
    [SerializeField] private float theTimeItTakesToDetonate;
    [SerializeField] private float timeTillDetonation;
    [SerializeField] private float explosionDamage;
    [SerializeField] private bool playerInRange;
    
    [Space(10)] [Header("Visual Settings")]
    [SerializeField] private GameObject childWithSpriteRenderer;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private float flashInterval;
    [SerializeField] private Sprite bombSpriteAfterAThirdOfTimePassed;
    [SerializeField] private Sprite bombSpriteAfterTwoThirdOfTimePassed;
    [SerializeField] private Sprite bombSpriteAfterThreeThirdOfTimePassed;
    [SerializeField] private Sprite bombSpriteAfterDefused;
    [SerializeField] private Sprite explosionSprite;
    
    [Space(10)] [Header("Defuse Settings")]
    [SerializeField] private bool isDefused;
    [SerializeField] private bool isHoveringOverUndefusedBomb;
    
    private Mouse mouse;
    
    
    void Start()
    {
        spriteRenderer = childWithSpriteRenderer.GetComponent<SpriteRenderer>();
        //originalColor = spriteRenderer.color;
        mouse = Mouse.current;
        timeTillDetonation = theTimeItTakesToDetonate;
        StartCoroutine(ChangeBombSpriteDependingOnTimeLeft());
    }
    
    private void Update()
    {
        if (isDefused) return;
        
        if (timeTillDetonation > 0)
        {
            timeTillDetonation -= Time.deltaTime;
        }
        else
        {
            DetonateBomb();
        }
        
        HandleDefusingInput();
    }
    
    private void HandleDefusingInput()
    {
        if (mouse == null) return;
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouse.position.ReadValue());
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        Collider2D hit = Physics2D.OverlapPoint(mousePos2D);
        
        if (hit != null && hit.gameObject == gameObject && mouse.leftButton.wasPressedThisFrame)
        {
            DefuseBomb();
        }
    }
    
    private void DefuseBomb()
    {
        if (isDefused) return;
        isDefused = true;
        
        StopCoroutine(ChangeBombSpriteDependingOnTimeLeft());
        
        spriteRenderer.sprite = bombSpriteAfterDefused;
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        
        Destroy(gameObject, 1f);
    }

    private void DetonateBomb()
    {
        if (isDefused) return;
        
        CheckIfPlayerInRange();
        spriteRenderer.sprite = explosionSprite;
        Destroy(gameObject, 1f);
    }

    private void CheckIfPlayerInRange()
    {
        if (playerInRange)
        {
            PlayerStats.Instance.TakeDamage(explosionDamage);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    
    IEnumerator ChangeBombSpriteDependingOnTimeLeft()
    {
        float oneThirdTime = theTimeItTakesToDetonate / 3f;
        float twoThirdsTime = theTimeItTakesToDetonate * 2f / 3f;

        while (timeTillDetonation > 0 && !isDefused)
        {
            if (timeTillDetonation > twoThirdsTime)
            {
                spriteRenderer.sprite = bombSpriteAfterAThirdOfTimePassed;
            }
            else if (timeTillDetonation > oneThirdTime)
            {
                spriteRenderer.sprite = bombSpriteAfterTwoThirdOfTimePassed;
            }
            else
            {
                spriteRenderer.sprite = bombSpriteAfterThreeThirdOfTimePassed;
            }
            
            yield return new WaitForSeconds(flashInterval);
        }
    }
}