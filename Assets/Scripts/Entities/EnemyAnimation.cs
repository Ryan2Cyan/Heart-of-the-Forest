using UnityEngine;

namespace Entities
{
    public class EnemyAnimation : MonoBehaviour
    {
        private Enemy parentScript;
        private Animator animator;
        private Vector3 currentPos;
        private Vector3 lastPos;
        private bool isRunning;
        
        private static readonly int Running = Animator.StringToHash("running");
        private static readonly int Attack = Animator.StringToHash("attack");
        private static readonly int Death = Animator.StringToHash("death");

        public AudioClip Walk;
        private AudioSource src;

        private void Start()
        {
            animator = transform.GetComponent<Animator>();
            parentScript = transform.parent.gameObject.GetComponent<Enemy>();
            src = gameObject.GetComponentInParent<AudioSource>();
        }

        
        private void Update()
        {
            // Check if the enemy is moving:
            currentPos = transform.position;
            isRunning = lastPos != currentPos;

            lastPos = transform.position;
            
            
            // Set animator variables:
            if (parentScript.isDead)
            {
                parentScript.isAttacking = false;
                animator.SetBool(Running, false);
                animator.SetBool(Attack, false);
                animator.SetBool(Death, true);
            }
            if (parentScript.isAttacking)
            {
                animator.SetBool(Attack, true);
            }
            if (!parentScript.isAttacking)
            {
                animator.SetBool(Attack, false);
            }
            animator.SetBool(Running, isRunning);
        }

        public void WalkSound()
		{
            src.PlayOneShot(Walk);
		}
        
    }
}
