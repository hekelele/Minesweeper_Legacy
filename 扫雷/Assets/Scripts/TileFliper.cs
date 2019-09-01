using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileFliper : MonoBehaviour
{
    private Tilemap _FrontMap;
    public Tile[] _MarkType;

    private void Awake()
    {
        _FrontMap = GetComponent<Tilemap>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int vp = _FrontMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            TileBase t = _FrontMap.GetTile(vp);
            if (t != null)
            {
                _FrontMap.SetTile(vp, null);
                Vector3Int[] vps = GameFlowManager._Main.getFlipArea(vp);
                for (int i = 0; i < vps.Length; i++)
                {
                    _FrontMap.SetTile(vps[i], null);
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Vector3Int vp = _FrontMap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            TileBase t = _FrontMap.GetTile(vp);
            if (t != null)
            {
                int type = -1;
                for (int i = 0; i < _MarkType.Length; i++)
                {
                    if (_MarkType[i] == t)
                    {
                        type = i;
                        break;
                    }
                }
                if (type >= 0)
                {
                    type = (type + 1) % _MarkType.Length;
                    _FrontMap.SetTile(vp, _MarkType[type]);
                }
                else
                {
                    Debug.LogError("type unknow");
                }
            }

            
        }
    }
    
}
