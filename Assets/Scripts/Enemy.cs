using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TDG.Entity;

public class Enemy : Entity
{
    public NavMeshAgent enemy;
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        SphereCollider sphereCollider = this.GetComponent<SphereCollider>();

        sphereCollider.radius = 2;

        player = GameObject.Find("Player(Clone)").transform;
    }

    // Update is called once per frame
    void Update()
    {
        enemy.SetDestination(player.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("In range of player, launch attack script here and change to idle animation");
    }
}
