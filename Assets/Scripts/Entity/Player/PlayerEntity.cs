﻿using UnityEngine;
using System.Collections;
using System;

public class PlayerEntity : Entity 
{
    public float power, defense, attackSpeed, movementSpeed, minDamage, maxDamage;

    private int nextLevelExperience;
    public int NextLevelExperience
    {
        get
        {
            return nextLevelExperience;
        }

        set
        {
            nextLevelExperience = value;
        }
    }

    private int attributePoints;
    public int AttributePoints
    {
        get { return attributePoints; }
        set { attributePoints = value; }
    }

    public Mesh mesh { get { return GetComponent<MeshFilter>().mesh; } }

    GameManager gamemanager;

	// Use this for initialization
    public void Awake()
    {
        base.Awake();
        gamemanager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        Experience = 0;
        Level = 1;
        nextLevelExperience = 100;
    }


	public void Start() 
    {
        base.Start();

        

        if (gamemanager.loadsavetest == true)
        {
            for (int i = 2; i < 6; i++)
            {
                if(PlayerPrefs.HasKey("ability"+i) == true)
                {
                    abilityManager.AddAbility(GameManager.Abilities[PlayerPrefs.GetString("ability"+i)], i);
                    abilityIndexDict[PlayerPrefs.GetString("ability"+i)] = i;
                }
            }
            for( int i=0;i<6;i++)
            {
                
                if(PlayerPrefs.HasKey(i+"name") == true)
                {
                    equipment tempequip = gamemanager.EquipmentFactory.loadequipment(i+"");
                    if (addEquipment(tempequip))
                    {
                        Debug.Log("EQUIPPING " + tempequip.equipmentName + " FROM LOAD");
                    }
                    else
                    {
                        Debug.Log("CAN'T EQUIP THE "+tempequip.equipmentName+" FOR SOME REASON");
                    }
                }
            }

            // Load saved items, if any.
            Inventory.LoadItems();
        }

        else
        {
            abilityManager.AddAbility(GameManager.Abilities["shadowbolt"], 2);
            abilityManager.AddAbility(GameManager.Abilities["poisonbolt"], 3);
            abilityManager.AddAbility(GameManager.Abilities["chaosbolt"], 4);
            abilityManager.AddAbility(GameManager.Abilities["bladewaltz"], 5);

            abilityIndexDict["shadowbolt"] = 2;
            abilityIndexDict["poisonbolt"] = 3;
            abilityIndexDict["chaosbolt"] = 4;
            abilityIndexDict["bladewaltz"] = 5;
        }

	}
	
	// Update is called once per frame
	public void Update () 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit target;
        Physics.Raycast(ray, out target, Mathf.Infinity);
        Vector3 vectorToMouse = target.point - gameObject.transform.position;
        Vector3 normalizedVectorToMouse = new Vector3(vectorToMouse.x, gameObject.transform.forward.y, vectorToMouse.z);
        Debug.DrawRay(gameObject.transform.position, normalizedVectorToMouse.normalized * 5.0f, Color.yellow);
        //Debug.Log(ray.direction);


        //Debug.DrawRay(ray.origin, ray.direction, Color.yellow);
        //abilities[4].AttackHandler(GameObject.FindGameObjectWithTag("Player"), true);
        //abilities[5].AttackHandler(GameObject.FindGameObjectWithTag("Player"), true);

        // update these on-demand instead of every update
        power = currentAtt.Power;
        defense = currentAtt.Defense;
        attackSpeed = currentAtt.AttackSpeed;
        movementSpeed = currentAtt.MovementSpeed;
        minDamage = currentAtt.MinDamage;
        maxDamage = currentAtt.MaxDamage;

        //Debug.Log(abilities.Count);

        
	}

    public void OnApplicationQuit()
    {
       // Inventory.SaveItems();
    }

    public Attributes GetAttributes()
    {
        return currentAtt;
    }

    /// <summary>
    /// Give the specified amount of experience to the player.
    /// </summary>
    /// <param name="expToAdd">Experience to add to the player.</param>
    public void GiveExperience(int expToAdd)
    {
        Experience += expToAdd;
    }

    public void GiveAttributePoints(int attrPointsToAdd)
    {
        attributePoints += attrPointsToAdd;
    }
}
