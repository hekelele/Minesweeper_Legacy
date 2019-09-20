using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public enum GameState
{
    Wait,
    Run,
    Pause,
    End
}


public class GameFlowManager : MonoBehaviour
{
    private GameState _State;
    public FrontFieldTilemap _FrontMap;
    public MineFieldTilemap _BackMap;
    public SEPlayer _SE;


    public static GameFlowManager _Main = null;

    public int _Xcount = 9, _YCount = 9;

    private void Awake()
    {
        if (_Main != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _Main = this;
        }
        _State = GameState.Wait;
    }

    private void Start()
    {
        ResetGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (_State == GameState.End)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Vector3Int vp = getClickPos();
            if (_FrontMap.isPosValid(vp))
            {
                if (_State==GameState.Wait)
                {
                    _BackMap.makeMineField(_MineNumber, Camera.main.ScreenToWorldPoint(Input.mousePosition));
                    _State = GameState.Run;
                }
                if (_FrontMap.flipTile(vp, true))
                {
                    // 普通点了一下
                    _SE.playSE("click");
                    checkWinState();
                }
                else
                {
                    // 点死了
                    _State = GameState.End;
                    _SE.playSE("explode");
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Vector3Int vp = getClickPos();
            if (_FrontMap.isPosValid(vp))
            {
                TileBase tb = _FrontMap.GetTile(vp);
                if (tb != null)
                {
                    _SE.playSE("mark");
                    if (tb == _FrontMap._MarkTypes[0])
                    {
                        _FrontMap.SetTile(vp, _FrontMap._MarkTypes[1]);
                    }
                    else if (tb == _FrontMap._MarkTypes[1])
                    {
                        _FrontMap.SetTile(vp, _FrontMap._MarkTypes[2]);
                    }
                    else
                    {
                        _FrontMap.SetTile(vp, _FrontMap._MarkTypes[0]);
                    }
                    
                }
            }
        }
        
    }

    private Vector3Int getClickPos()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D[] co2ds = Physics2D.OverlapPointAll(mousePos);
        for (int i = 0; i < co2ds.Length; i++)
        {
            if (co2ds[i].tag == "Front")
            {
                Vector3Int vp = _FrontMap.WorldToCell(mousePos);
                return vp;
            }
        }
        return new Vector3Int(-1, -1, 0);
    }


    private void checkWinState()
    {
        if (_FrontMap.getTileNum() == _MineNumber)
        {
            _State = GameState.End;
            _SE.playSE("win");
        }
    }



    public void ResetGame()
    {
        _BackMap.clearMineFiled();
        _FrontMap.clearMineFiled();
        _State = GameState.Wait;
        _SE.playSE("reset");
    }
}
