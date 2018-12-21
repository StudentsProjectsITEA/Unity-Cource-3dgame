using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    public Transform target;
    private NavMeshAgent AIAgent;
    public  bool isDead;
    

    void Start () {
        AIAgent = GetComponent<NavMeshAgent>();
	}
	
	void Update () {
        AIAgent.SetDestination(target.position);
	}

    public void PlayDeathAnim()
    {
        GetComponent<Animator>().SetBool("isDead", true);
    }
    void Death()
    {
        FindObjectOfType<EnemySpawn>().SpawnOnDeath();
        Destroy(this.gameObject);
    }
}
