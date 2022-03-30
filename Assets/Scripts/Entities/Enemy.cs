using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TDG.Entity;

public class Enemy : Entity
{
    public NavMeshAgent enemy;
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {

        SphereCollider sphereCollider = this.GetComponent<SphereCollider>();

        sphereCollider.radius = 2;

        player = GameObject.Find("Player(Clone)");
    }

    // Update is called once per frame
    void Update()
    {
        enemy.SetDestination(player.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        Attack();
    }

    public override void Attack()
    {
        Debug.Log(entityName + " tried to attack!");
        player.GetComponent<Player>().TakeDamage(weapon.damage);
    }
}
