using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpatialPartitionPattern
{
    public class Grid
    {
        // Need this to convert from world coordinate position to cell position
        int cellSize;

        // This is a grid with a soldier in each cell. Soldiers in same cell link together
        Soldier[,] cells;


        // Init the grid
        public Grid(int mapWidth, int cellSize)
        {
            this.cellSize = cellSize;

            int numberOfCells = mapWidth / cellSize;

            cells = new Soldier[numberOfCells, numberOfCells];
        }


        // Add unity to the grid
        public void Add(Soldier soldier)
        {
            // Determine which grid cell the soldier is in
            int cellX = (int)(soldier.soldierTransform.position.x / cellSize);
            int cellZ = (int)(soldier.soldierTransform.position.z / cellSize);

            // Add the soldier to the front of the list for the cell it's in
            soldier.previousSoldier = null;
            soldier.nextSoldier = cells[cellX, cellZ];

            // Associate this cell with this soldier
            cells[cellX, cellZ] = soldier;

            if (soldier.nextSoldier != null)
            {
                // Set this soldier to be the previous soldier of the next soldier of this soldier (this is like explaining where Rogue One fits into the Star Wars universe)
                // In case you're wondering, Rogue One is the sequel to the prequels of the original trilogy
                soldier.nextSoldier.previousSoldier = soldier;
            }
        }


        // Get the closest enemy from the grid
        public Soldier FindClosestEnemy(Soldier friendlySoldier)
        {
            // Determine which grid cell the friendly soldier is in
            int cellX = (int)(friendlySoldier.soldierTransform.position.x / cellSize);
            int cellZ = (int)(friendlySoldier.soldierTransform.position.z / cellSize);

            // Get the first enemy in grid
            Soldier enemy = cells[cellX, cellZ];

            // Find the closest soldier of all in the linked list
            Soldier closestSoldier = null;

            float bestDistSqr = Mathf.Infinity;

            // Loop through the linked list
            while (enemy != null)
            {
                // The distance sqr between the soldier and this enemy
                float distSqr = (enemy.soldierTransform.position - friendlySoldier.soldierTransform.position).sqrMagnitude;

                // If this distance is better than the previous best distance, then we have found an enemy that's closet
                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;

                    closestSoldier = enemy;
                }

                // Get the next enemy in the list
                enemy = enemy.nextSoldier;
            }

            return closestSoldier;
        }


        // A soldier in the grid has moved, so see if we need to update in which grid the soldier is
        public void Move(Soldier soldier, Vector3 oldPos)
        {
            // See which cell it was in
            int oldCellX = (int)(oldPos.x / cellSize);
            int oldCellZ = (int)(oldPos.z / cellSize);

            // See which cell it is in now
            int cellX = (int)(soldier.soldierTransform.position.x / cellSize);
            int cellZ = (int)(soldier.soldierTransform.position.z / cellSize);

            // If it didn't change cell, we are done
            if (cellX == oldCellX && cellZ == oldCellZ)
            {
                return;
            }

            // Unlink it from the list of its old cell
            if (soldier.previousSoldier != null)
            {
                soldier.previousSoldier.nextSoldier = soldier.nextSoldier;
            }

            if (soldier.nextSoldier != null)
            {
                soldier.nextSoldier.previousSoldier = soldier.previousSoldier;
            }

            // If it's the head of a list, remove it
            if (cells[oldCellX, oldCellZ] == soldier)
            {
                cells[oldCellX, oldCellZ] = soldier.nextSoldier;
            }

            // Add it back to the grid at its new cell
            Add(soldier);
        }
    }
}