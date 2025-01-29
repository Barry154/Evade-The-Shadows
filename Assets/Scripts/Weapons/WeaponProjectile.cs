using UnityEngine;

namespace Scripts.Weapons
{
    public class WeaponProjectile : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] protected SpriteRenderer visual;
        [SerializeField] protected Animator animator;
        [SerializeField] protected ParticleSystem hitParticles;
        [SerializeField] protected Collider2D collider;

        [Header("Settings")] 
        [SerializeField] protected float speed;

        protected Vector3 dir;
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (hitParticles)
            {
                hitParticles.transform.SetParent(transform.parent);
                hitParticles.Play();
            }
            Destroy(gameObject);
        }
    }
}