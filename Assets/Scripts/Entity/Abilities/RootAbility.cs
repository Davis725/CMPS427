﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RootAbility : Ability
{
    public RootAbility(AttackType attackType, DamageType damageType, float range, float angle, float cooldown, float damageMod, float resourceCost, string id, string readable, GameObject particles)
        : base(attackType, damageType, range, angle, cooldown, damageMod, resourceCost, id, readable, particles)
    {

    }

    public override void AttackHandler(GameObject source, Entity attacker, bool isPlayer)
    {
        List<GameObject> attacked = OnAttack(source, isPlayer);

        if (isPlayer == true)
        {
            foreach (GameObject enemy in attacked)
            {
                if (enemy.GetComponent<AIController>().IsResetting() == false
                    && enemy.GetComponent<AIController>().IsDead() == false)
                {
                    Entity defender = enemy.GetComponent<Entity>();
                    DoBuff(enemy, attacker);

                    if (enemy.GetComponent<AIController>().IsInCombat() == false)
                    {
                        enemy.GetComponent<AIController>().BeenAttacked(source);
                    }
                }
            }
        }

        else
        {
            foreach (GameObject enemy in attacked)
            {
                Entity defender = enemy.GetComponent<Entity>();
                DoBuff(enemy, attacker);

            }
        }
    }

    public override List<GameObject> OnAttack(GameObject source, bool isPlayer)
    {
        List<GameObject> enemiesToAttack = new List<GameObject>();

        Vector3 forward = new Vector3();

        // this is a player attack, forward attack vector will be based on cursor position
        if (isPlayer == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit target;
            Physics.Raycast(ray, out target, Mathf.Infinity);
            Vector3 vectorToMouse = target.point - source.transform.position;
            forward = new Vector3(vectorToMouse.x, source.transform.forward.y, vectorToMouse.z).normalized;
        }

        int enemyMask = LayerMask.NameToLayer("Enemy");
        int playerMask = LayerMask.NameToLayer("Player");

        Collider[] colliders;

        if (isPlayer == true)
        {
            colliders = Physics.OverlapSphere(source.transform.position, range, 1 << enemyMask);
        }

        else
        {
            colliders = Physics.OverlapSphere(source.transform.position, range, 1 << playerMask);
        }

        foreach (Collider collider in colliders)
        {

            // create a vector from the possible enemy to the attacker
            Vector3 normalizedAttackPosition = new Vector3(source.transform.position.x, 1, source.transform.position.z);
            Vector3 normalizedDefenderPosition = new Vector3(collider.transform.position.x, 1, collider.transform.position.z);

            Vector3 enemyVector = normalizedDefenderPosition - normalizedAttackPosition;
            Vector3 enemyVector2 = normalizedAttackPosition - normalizedDefenderPosition;

            // this is an enemy attack, forward attack vector will be based on target position
            if (isPlayer == false)
            {
                forward = enemyVector;
            }

            // if the angle between the forward vector of the attacker and the enemy vector is less than the angle of attack, the enemy is within the attack angle
            if (Vector3.Angle(forward, enemyVector) < angle)
            {
                RaycastHit hit = new RaycastHit();

                if (isPlayer == true)
                {
                    // try to cast a ray from the enemy to the player

                    bool rayCastHit = Physics.Raycast(new Ray(normalizedDefenderPosition, enemyVector2), out hit, range, ~(1 << enemyMask));


                    if (!rayCastHit)
                    {

                    }
                    // if the ray hits, the enemy is in line of sight of the player, this is a successful attack hit
                    else
                    {
                        if (hit.collider.gameObject.tag == "Player")
                        {

                            Debug.DrawRay(normalizedDefenderPosition, enemyVector, Color.green, 0.5f);
                            Debug.DrawRay(normalizedDefenderPosition, enemyVector2, Color.red, 0.5f);

                            enemiesToAttack.Add(collider.gameObject);
                        }
                    }
                }

                else
                {
                    // try to cast a ray from the player to the enemy

                    Debug.Log("rootin");

                    bool rayCastHit = Physics.Raycast(new Ray(normalizedDefenderPosition, enemyVector2), out hit, range, ~(1 << playerMask));

                    if (!rayCastHit)
                    {
                        Debug.Log("fail");
                    }
                    // if the ray hits, the player is in line of sight of the enemy, this is a successful attack hit
                    else
                    {
                        if (hit.collider.gameObject.tag == "Enemy")
                        {
                            Debug.DrawRay(normalizedDefenderPosition, enemyVector, Color.green, 0.5f);
                            Debug.DrawRay(normalizedDefenderPosition, enemyVector2, Color.red, 0.5f);

                            enemiesToAttack.Add(collider.gameObject);
                        }

                        else
                        {
                            Debug.Log("not something that we didn't hit");
                            Debug.Log(hit.collider.name);
                        }
                    }
                }
            }
        }

        return enemiesToAttack;
    }

    /// <summary>
    /// Adds the buff to the enemy. Pretty straightforward
    /// 
    /// </summary>
    /// <param name="target">the gameobject of the target</param>
    /// <param name="source">the entity that is applying the buff/debuff</param>
    public void DoBuff(GameObject target, Entity source)
    {
        target.GetComponent<EntityAuraManager>().Add("root", source);

    }
    
}