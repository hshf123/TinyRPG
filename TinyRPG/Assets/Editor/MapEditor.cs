using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MapEditor
{
#if UNITY_EDITOR
    [MenuItem("Tools/GenerateMap")]
    private static void GenerateMap()
    {
        GameObject[] gameObjects = Resources.LoadAll<GameObject>("Prefabs/Map");
        foreach (GameObject go in gameObjects)
        {
            Tilemap env = Util.FindChild<Tilemap>(go, "Env", true);
            Tilemap collision = Util.FindChild<Tilemap>(go, "Collision", true);
            Tilemap portal = Util.FindChild<Tilemap>(go, "Portal", true);

            string collisionTxt = $"{SceneManager.GetActiveScene().name}_Collision";
            string portalTxt = $"{SceneManager.GetActiveScene().name}_Portal";

            using (var writer = File.CreateText($"Assets/Resources/Map/{collisionTxt}.txt"))
            {
                writer.WriteLine(env.cellBounds.xMin);
                writer.WriteLine(env.cellBounds.xMax);
                writer.WriteLine(env.cellBounds.yMin);
                writer.WriteLine(env.cellBounds.yMax);

                for (int y = env.cellBounds.yMax; y >= env.cellBounds.yMin; y--)
                {
                    for (int x = env.cellBounds.xMin; x <= env.cellBounds.xMax; x++)
                    {
                        TileBase tb = collision.GetTile(new Vector3Int(x, y, 0));
                        if (tb != null)
                            writer.Write("1");
                        else
                            writer.Write("0");
                    }
                    writer.WriteLine();
                }
            }

            using (var writer = File.CreateText($"Assets/Resources/Map/{portalTxt}.txt"))
            {
                writer.WriteLine(env.cellBounds.xMin);
                writer.WriteLine(env.cellBounds.xMax);
                writer.WriteLine(env.cellBounds.yMin);
                writer.WriteLine(env.cellBounds.yMax);

                for (int y = env.cellBounds.yMax; y >= env.cellBounds.yMin; y--)
                {
                    for (int x = env.cellBounds.xMin; x <= env.cellBounds.xMax; x++)
                    {
                        TileBase tb = portal.GetTile(new Vector3Int(x, y, 0));
                        if (tb != null)
                            writer.Write("1");
                        else
                            writer.Write("0");
                    }
                    writer.WriteLine();
                }
            }
        }

    }

#endif
}
