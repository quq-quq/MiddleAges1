using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    private float timeBtwAttack;
    public float strartTimeBtwAttack;

    public Transform attackPos;
    public float radiusAttack;
    public int damage;
    public int health;
    public LayerMask layerOfObj;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if (layerOfObj != 4)
            anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timeBtwAttack <= 0)
        {
            if(gameObject.name =="Player"  && Input.GetMouseButton(0))
            {
                anim.SetTrigger("IsAttack");
            }
            else if(gameObject.name != "Player" && layerOfObj != 0)
            {
                Collider[] enemies = Physics.OverlapSphere(attackPos.position, radiusAttack, layerOfObj);
                if(enemies.Length> 0)
                {
                    anim.SetTrigger("IsAttack");
                }
            }
            timeBtwAttack = strartTimeBtwAttack;
        }
        else
        {
            timeBtwAttack -= Time.deltaTime;
        }

        if (health <= 0)
            Destroy(gameObject);
    }

    public void TakeDamage(int damage1)
    {
        health -= damage1;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, radiusAttack);
    }

    public void PutDamage()
    {
        Collider[] enemies = Physics.OverlapSphere(attackPos.position, radiusAttack, layerOfObj);
        if (enemies.Length > 0)
            enemies[0].GetComponent<AttackScript>().TakeDamage(damage);
    }
}
