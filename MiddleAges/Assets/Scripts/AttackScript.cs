using System.Collections;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    [SerializeField] private float timeBtwAttack;

    public float radiusAttack;
    public int damageDefault;
    [System.NonSerialized] public int damage;

    [System.NonSerialized] public Transform AttackTransform;
    private AudioSource hitAudio;

    private Animator anim;
    private bool isAttacking;

    void Start()
    {
        damage = damageDefault;
        anim = gameObject.GetComponentInParent<Animator>();
        hitAudio = gameObject.GetComponent<AudioSource>();
    }
    public void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger("IsAttack");
            StartCoroutine(AttackReload());
        }
    }
    private IEnumerator AttackReload()
    {
        yield return new WaitForSeconds(timeBtwAttack);
        isAttacking = false;
    }
    public void PutDamage()
    {
        hitAudio.Play();
        if (AttackTransform != null)
        {
            AttackTransform.GetComponent<EntityBehaviour>().SetDamage(damage);
        }
    }

}
