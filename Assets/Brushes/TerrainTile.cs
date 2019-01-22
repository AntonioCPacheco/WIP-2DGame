using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TerrainTile : Tile
{
    [SerializeField] private Sprite[] terrainSprites;
    [SerializeField] private Sprite preview;

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector3Int pos = new Vector3Int(position.x + j, position.y + i, position.z);
                if (CheckTerrain(tilemap, pos))
                {
                    tilemap.RefreshTile(pos);
                    
                }
            }
        }
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        //Creating an int mask for which tileoption to choose from
        int mask = 0;
        if (CheckTerrain(tilemap, position + new Vector3Int(0, 1, 0)))
            mask += 1;
        if (CheckTerrain(tilemap, position + new Vector3Int(1, 0, 0)))
            mask += 2;
        if (CheckTerrain(tilemap, position + new Vector3Int(0, -1, 0)))
            mask += 4;
        if (CheckTerrain(tilemap, position + new Vector3Int(-1, 0, 0)))
            mask += 8;

        int index = GetIndex((byte)mask);
        if (index >= 0 && index <= terrainSprites.Length)
        {
            tileData.sprite = terrainSprites[index];
        }
        else
            Debug.Log("ERROR: Sprite index beyond sprites array length");
    }

    private bool CheckTerrain(ITilemap t, Vector3Int pos)
    {
        return (t.GetTile(pos) == this || (t.GetTile(pos)!=null && t.GetTile(pos).GetType()==typeof(TerrainTile)));
    }

    private int GetIndex(byte mask)
    {
        switch (mask)
        {
            case 0: return 15;
            case 1: return 14;
            case 2: return 9;
            case 3: return 6;
            case 4: return 12;
            case 5: return 13;
            case 6: return 0;
            case 7: return 3;
            case 8: return 11;
            case 9: return 8;
            case 10: return 10;
            case 11: return 7;
            case 12: return 2;
            case 13: return 5;
            case 14: return 1;
            case 15: return 4;
        }
        return -1;
    }

#if UNITY_EDITOR    
    [MenuItem("Assets/Create/TerrainTiles")]
    public static void CreateTerrainTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save TerrainTiles", "New TerrainTiles", "asset", "");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TerrainTile>(), path);
    }
#endif
}