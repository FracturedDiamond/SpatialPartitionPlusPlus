using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialPartitionPattern
{
    // The friendly sphere which is chasing the enemy cubes
    public class Friendly : Soldier
    {
        // Init friendly
        public Friendly(GameObject soldierObj, float mapWidth)
        {
            this.soldierTransform = soldierObj.transform;

            this.walkSpeed = 2f;
        }


        // Move towards the closest enemy -- will always move within its grid
        public override void Move(Soldier closestEnemy)
        {
            // Rotate towards closest enemy
            soldierTransform.rotation = Quaternion.LookRotation(closestEnemy.soldierTransform.position - soldierTransform.position);
            // Move towards closest enemy
            soldierTransform.Translate(Vector3.forward * Time.deltaTime * walkSpeed);
        }
    }
}
