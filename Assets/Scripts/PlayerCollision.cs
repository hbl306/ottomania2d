using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameManager gameManager;

    private GameFlowManager gameFlowManager;

    private PlayerController playerController;


    private void Awake()
    {
        gameManager =
            FindAnyObjectByType<GameManager>();

        gameFlowManager =
            FindAnyObjectByType<GameFlowManager>();

        playerController =
            GetComponent<PlayerController>();
    }


    private void OnTriggerEnter2D(
        Collider2D collision
    )
    {
        // =================================
        // COIN
        // =================================

        if (collision.CompareTag("Coin"))
        {
            Destroy(
                collision.gameObject
            );


            if (gameManager != null)
            {
                gameManager.AddScore(1);
                AudioManager.Instance?.PlayCoin();
            }
        }
        else if (collision.CompareTag("Item"))
        {
            Destroy(
                collision.gameObject
            );


            if (gameManager != null)
            {
                gameManager.AddScoreItem(1);
                AudioManager.Instance?
        .PlayItemPickup();
            }
        }


        // =================================
        // TRAP
        // =================================

        else if (
            collision.CompareTag("Trap")
        )
        {
            if (playerController != null)
            {
                playerController.Die();
                AudioManager.Instance?
    .PlayPlayerHurt();
            }
        }


        // =================================
        // ENEMY
        // =================================

        else if (
            collision.CompareTag("Enemy")
        )
        {
            if (playerController != null)
            {
                playerController.Die();
                AudioManager.Instance?
    .PlayPlayerHurt();
            }
        }


        // =================================
        // NEXT MAP
        // =================================

        else if (
            collision.CompareTag("MapExit")
        )
        {
            if (gameFlowManager != null)
            {
                gameFlowManager.GoToNextMap();
            }
        }
        else if (
            collision.CompareTag("Victory")
        )
        {
            if (gameManager != null)
            {
                gameManager.Victory();
            }
        }
        
    }
}