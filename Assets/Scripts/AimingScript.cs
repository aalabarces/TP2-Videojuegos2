using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class AimingScript : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private bool isPlayer => gameObject.transform.parent.CompareTag("Player");
    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isPlayer) {
            aimInMouseDirection();
        }
        else {
            aimInPlayerDirection();
        }
    }

    private void aimInMouseDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
    }

    private void aimInPlayerDirection()
    {
        GameObject player = GameManager.Instance.player.gameObject;
        if (player == null) return;

        Vector3 playerPosition = player.transform.position;
        Vector3 direction = playerPosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
    }

    public void EndFiringAnimation()
    {
        if (isPlayer)
        {
            GameManager.Instance.player.SetFiringState(false);
        }
        animator.SetBool("isFiring", false);
    }
    public void PlayCannonSound()
    {
        audioSource.Play();
        if (!isPlayer) audioSource.volume = 0.75f;
    }
}
