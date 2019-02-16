using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
[CustomGridBrush(false, true, false, "Prefab Brush / Spikes Brush")]
public class SpikesBrush : GridBrushBase
{
    public GameObject upArrow;
    public GameObject downArrow;
    public int zPos = 0;
    public float yUpOffset = 0f;
    public float yDownOffset = 0f;

    public bool paintUpArrow = true;

    public override void Paint(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        brushTarget = GameObject.Find("SpikesLayer");

        Vector3Int cellPosition = new Vector3Int(position.x, position.y, zPos);
        Vector3 centerTile;
        if (paintUpArrow)
            centerTile = new Vector3(.5f, .5f + yUpOffset, 0f);
        else
            centerTile = new Vector3(.5f, .5f + yDownOffset, 0f);

        if (GetObjectInCell(gridLayout, brushTarget.transform, new Vector3Int(position.x, position.y, zPos)) != null)
            return;

        GameObject go;
        if (paintUpArrow)
            go = Instantiate(upArrow);
        else
            go = Instantiate(downArrow);

        go.transform.SetParent(brushTarget.transform);
        go.transform.position = gridLayout.LocalToWorld(gridLayout.CellToLocalInterpolated(cellPosition + centerTile));
    }

    public override void Erase(GridLayout gridLayout, GameObject brushTarget, Vector3Int position)
    {
        brushTarget = GameObject.Find("SpikesLayer");

        Transform erased = GetObjectInCell(gridLayout, brushTarget.transform, new Vector3Int(position.x, position.y, zPos));
        if (erased != null)
            DestroyImmediate(erased.gameObject);
    }

    private static Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3Int position)
    {
        int childCount = parent.childCount;

        Vector3 min = grid.LocalToWorld(grid.CellToLocalInterpolated(position));

        Vector3 max = grid.LocalToWorld(grid.CellToLocalInterpolated(position + Vector3.one));
        Bounds bounds = new Bounds((max + min) * .5f, max - min);

        for(int i=0; i<childCount; i++)
        {
            Transform child = parent.GetChild(i);
            string tag = child.gameObject.tag;

            if(bounds.Contains(child.position) && (tag == "Spikes"))
            {
                return child;
            }

        }
        return null;
    }
}
