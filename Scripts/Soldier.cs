using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialPartitionPattern
{
    // The soldier base class for enemies and friendly
    public class Soldier
    {
        // To change material
        public MeshRenderer soldierMeshRenderer;
        // To move the soldier
        public Transform soldierTransform;
        // The speed at which the soldier is walking with
        protected float walkSpeed;
        // Previous and next soldiers in the linked list of soldiers
        public Soldier previousSoldier;
        public Soldier nextSoldier;

        // The enemy doesn't need any outside information
        public virtual void Move()
        { }

        // The friendly has to move which soldier is the closest
        public virtual void Move(Soldier soldier)
        { }
    }
}
