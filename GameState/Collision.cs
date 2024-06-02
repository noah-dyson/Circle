using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

namespace Circle
{
    public static class Collisions
    {
        // performs a separating axis test between a line and one of the players collision boxes
        public static bool SeparatingAxisCollision(Line line, Player player, Vector2[] partVertices)
        {
            Vector2[] vertices = line.Vertices;
            Vector2[] axis = line.Axis;
            Vector2[] playerVertices = partVertices;
            Vector2[] playerAxis = player.Axis;

            // for each axis of the line, project the vertices of the line and the player onto the axis
            // and check if the projections overlap
            for (int i = 0; i < 2; i++)
            {
                float[] lineProjection = new float[4];
                float[] playerProjection = new float[4];

                for (int j = 0; j < 4; j++)
                {
                    lineProjection[j] = Vector2.Dot(vertices[j], axis[i]);
                    playerProjection[j] = Vector2.Dot(playerVertices[j], axis[i]);
                }

                float lineMin = lineProjection.Min();
                float lineMax = lineProjection.Max();
                float playerMin = playerProjection.Min();
                float playerMax = playerProjection.Max();

                if (lineMax < playerMin || playerMax < lineMin)
                {
                    return false;
                }
            }

            // the same check but for the player's axes
            for (int i = 0; i < 2; i++)
            {
                float[] lineProjection = new float[4];
                float[] playerProjection = new float[4];

                for (int j = 0; j < 4; j++)
                {
                    lineProjection[j] = Vector2.Dot(vertices[j], playerAxis[i]);
                    playerProjection[j] = Vector2.Dot(playerVertices[j], playerAxis[i]);
                }

                float lineMin = lineProjection.Min();
                float lineMax = lineProjection.Max();
                float playerMin = playerProjection.Min();
                float playerMax = playerProjection.Max();

                if (lineMax < playerMin || playerMax < lineMin)
                {
                    return false;
                }
            }

            return true;
        }

        // checks if the player is near enough to a line to be considered a possible collision
        public static List<Line> SortAndSweep(Player player, LinkedList<Line> lines)
        {
            Vector2 axis = new Vector2(1, 0);
            
            // project the player vertices onto the x axis
            float[] playerProj = { Vector2.Dot(player.VerticesTop[0], axis), Vector2.Dot(player.VerticesTop[1], axis) };
            float playerMin = playerProj.Min();
            float playerMax = playerProj.Max();

            List<Line> possibleCollisions = new List<Line>();

            // for each line, project the line vertices onto the x axis
            // and check if the projections overlap with the player's
            foreach (Line line in lines)
            {
                float[] lineProj = { Vector2.Dot(line.Vertices[0], axis), Vector2.Dot(line.Vertices[1], axis), Vector2.Dot(line.Vertices[2], axis), Vector2.Dot(line.Vertices[3], axis) };
                float lineMin = lineProj.Min();
                float lineMax = lineProj.Max();

                if (lineMax > playerMin && playerMax > lineMin)
                {
                    possibleCollisions.Add(line);
                }
            }

            return possibleCollisions;
        }
    }
}