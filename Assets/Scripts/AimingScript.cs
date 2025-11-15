using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class AimingScript : MonoBehaviour
{
    private SpriteRenderer cannon;
    private Animator animator;
    private AudioSource audioSource;
    private Player player => GameManager.Instance.player;
    void Awake()
    {
        cannon = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        aimInMouseDirection();
    }

    private void aimInMouseDirection()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + 90f);
    }

    public void EndFiringAnimation()
    {
        player.SetFiringState(false);
        animator.SetBool("isFiring", false);
    }
    public void PlayCannonSound()
    {
        audioSource.Play();
    }
}
