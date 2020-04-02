using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace SpatialPartitionPattern
{
    public class GameController : MonoBehaviour
    {
        public bool isSpatialPartition;

        public GameObject friendlyObj;
        public GameObject enemyObj;

        public Text timeElapsed;

        // Change materials to detect which enemy is the closest
        public Material enemyMaterial;
        public Material closestEnemyMaterial;

        // To get a cleaner workspace, parent all soldiers to these empty gameobjects
        public Transform enemyParent;
        public Transform friendlyParent;

        // Store all soldiers in these lists
        List<Soldier> enemySoldiers = new List<Soldier>();
        List<Soldier> friendlySoldiers = new List<Soldier>();

        // Save the closest enemies to easier change back its material
        List<Soldier> closestEnemies = new List<Soldier>();

        // Grid data
        float mapWidth = 50f;
        int cellSize = 10;

        // Number of soldiers on each team
        public int numberOfSoldiers;

        // The Spatial Partition grid
        Grid grid;


        void Start()
        {
            // Create a new grid
            grid = new Grid((int)mapWidth, cellSize);

            // Add random enemies and friendly and store them in a list
            for (int i = 0; i < numberOfSoldiers; i++)
            {
                // Give the enemy a random position
                Vector3 randomPos = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth));

                // Create a new enemy
                GameObject newEnemy = Instantiate(enemyObj, randomPos, Quaternion.identity) as GameObject;

                // Add the enemy to a list
                enemySoldiers.Add(new Enemy(newEnemy, mapWidth, grid));

                // Parent it
                newEnemy.transform.parent = enemyParent;


                // Give the friendly a random position
                randomPos = new Vector3(Random.Range(0f, mapWidth), 0.5f, Random.Range(0f, mapWidth));

                // Create a new friendly
                GameObject newFriendly = Instantiate(friendlyObj, randomPos, Quaternion.identity) as GameObject;

                // Add the friendly to a list
                friendlySoldiers.Add(new Friendly(newFriendly, mapWidth));

                // Parent it
                newFriendly.transform.parent = friendlyParent;
            }
        }


        void Update()
        {
            // Track start time
            float startTime = Time.realtimeSinceStartup;

            // Move the enemies
            for (int i = 0; i < enemySoldiers.Count; i++)
            {
                enemySoldiers[i].Move();
            }

            // Reset material of the closest enemies
            for (int i = 0; i < closestEnemies.Count; i++)
            {
                closestEnemies[i].soldierMeshRenderer.material = enemyMaterial;
            }

            // Reset the list with closest enemies
            closestEnemies.Clear();

            // For each friendly, find the closest enemy and change its color and chase it
            for (int i = 0; i < friendlySoldiers.Count; i++)
            {
                Soldier closestEnemy;

                if (!isSpatialPartition)
                {
                    closestEnemy = FindClosestEnemySlow(friendlySoldiers[i]);
                } else
                {
                    // The fast version with spatial partition
                    closestEnemy = grid.FindClosestEnemy(friendlySoldiers[i]);
                }


                // If we found an enemy
                if (closestEnemy != null)
                {
                    // Change material
                    closestEnemy.soldierMeshRenderer.material = closestEnemyMaterial;

                    closestEnemies.Add(closestEnemy);

                    // Move the friendly in the direction of the enemy
                    friendlySoldiers[i].Move(closestEnemy);
                }

            }

            // Note how much time has passed
            float elapsedTime = (Time.realtimeSinceStartup - startTime) * 1000f;

           timeElapsed.text = elapsedTime + " ms";
        }

        //Find the closest enemy - slow version
        Soldier FindClosestEnemySlow(Soldier soldier)
        {
            Soldier closestEnemy = null;

            float bestDistSqr = Mathf.Infinity;

            //Loop thorugh all enemies
            for (int i = 0; i < enemySoldiers.Count; i++)
            {
                //The distance sqr between the soldier and this enemy
                float distSqr = (soldier.soldierTransform.position - enemySoldiers[i].soldierTransform.position).sqrMagnitude;

                //If this distance is better than the previous best distance, then we have found an enemy that's closer
                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;

                    closestEnemy = enemySoldiers[i];
                }
            }

            return closestEnemy;
        }

        public void NumSoldiers100()
        {
            // Set number of soldiers
            numberOfSoldiers = 100;
            // Reset the number of soldiers on the grid
            ResetSoldierCount();
        }

        public void NumSoldiers500()
        {
            // Set number of soldiers
            numberOfSoldiers = 500;
            // Reset the number of soldiers on the grid
            ResetSoldierCount();
        }

        public void NumSoldiers1000()
        {
            // Set number of soldiers
            numberOfSoldiers = 1000;
            // Reset the number of soldiers on the grid
            ResetSoldierCount();
        }

        public void IsSpatialPartition()
        {
            isSpatialPartition = !isSpatialPartition;
            ResetSoldierCount();
        }

        public void ResetSoldierCount()
        {
            // Fill an array with every soldier
            GameObject[] soldiers = GameObject.FindGameObjectsWithTag("soldierTag");

            // Iterate through and clear out all the soldiers from the grid
            foreach (GameObject obj in soldiers)
            {
                Destroy(obj);
            }

            // Clear the lists of both enemy and friendly soldiers
            enemySoldiers.Clear();
            friendlySoldiers.Clear();

            // Go back to start
            Start();
        }
    }    
}


