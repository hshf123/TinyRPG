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
            Tilemap collision = Util.FindChild<Tilemap>(go, "Collision", true);
            Tilemap portal = Util.FindChild<Tilemap>(go, "Portal", true);

            string collisionTxt = $"{SceneManager.GetActiveScene().name}_Collision";
            string portalTxt = $"{SceneManager.GetActiveScene().name}_Portal";

            using (var writer = File.CreateText($"Assets/Resources/Map/{collisionTxt}.txt"))
            {
                writer.WriteLine(collision.cellBounds.xMin);
                writer.WriteLine(collision.cellBounds.xMax);
                writer.WriteLine(collision.cellBounds.yMin);
                writer.WriteLine(collision.cellBounds.yMax);

                for (int y = collision.cellBounds.yMax; y >= collision.cellBounds.yMin; y--)
                {
                    for (int x = collision.cellBounds.xMin; x <= collision.cellBounds.xMax; x++)
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
                writer.WriteLine(portal.cellBounds.xMin);
                writer.WriteLine(portal.cellBounds.xMax);
                writer.WriteLine(portal.cellBounds.yMin);
                writer.WriteLine(portal.cellBounds.yMax);

                for (int y = portal.cellBounds.yMax; y >= portal.cellBounds.yMin; y--)
                {
                    for (int x = portal.cellBounds.xMin; x <= portal.cellBounds.xMax; x++)
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
