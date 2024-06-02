using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

namespace Circle
{
    public static class Collisions
    {
        public static bool SeparatingAxisCollision(Line line, Player player, Vector2[] partVertices)
        {
            Vector2[] vertices = line.vertices;
            Vector2[] axis = line.axis;
            Vector2[] playerVertices = partVertices;
            Vector2[] playerAxis = player.axis;

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

        public static List<Line> SortAndSweep(Player player, LinkedList<Line> lines)
        {
            Vector2 axis = new Vector2(1, 0);
            float[] playerProj = { Vector2.Dot(player.verticesTop[0], axis), Vector2.Dot(player.verticesTop[1], axis) };
            float playerMin = playerProj.Min();
            float playerMax = playerProj.Max();

            List<Line> possibleCollisions = new List<Line>();

            foreach (Line line in lines)
            {
                float[] lineProj = { Vector2.Dot(line.vertices[0], axis), Vector2.Dot(line.vertices[1], axis), Vector2.Dot(line.vertices[2], axis), Vector2.Dot(line.vertices[3], axis) };
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