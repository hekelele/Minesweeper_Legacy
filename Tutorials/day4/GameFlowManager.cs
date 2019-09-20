using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


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

    public Text _MineNumberText;
    public Text _ClockNumberText;

    public static GameFlowManager _Main = null;

    public int _Xcount = 9, _YCount = 9;
    public int _MineNumber = 10;

    public float _StartPoint;

    public Transform _GridTransform;
    public Transform _Back1Transform;
    public Transform _Back2Transform;

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
                    _StartPoint = Time.time;
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

        changeMineNum();

        if(_State == GameState.Run)
        {
            changeClockNum();
        }
        
    }

    private void changeMineNum()
    {
        _MineNumberText.text = "" + (_MineNumber - _FrontMap.countMine());
    }

    private void setMineNum(int n)
    {
        _MineNumberText.text = "" + n;
    }

    private void changeClockNum()
    {
        _ClockNumberText.text = "" + (int)(Time.time - _StartPoint);
    }

    private void setClockNum(int n)
    {
        _ClockNumberText.text = "" + n;
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
            setMineNum(0);
        }
    }



    public void ResetGame()
    {
        _BackMap.setSize(_Xcount, _YCount);
        _FrontMap.setSize(_Xcount, _YCount);
        _BackMap.clearMineFiled();
        _FrontMap.clearMineFiled();
        _State = GameState.Wait;
        _SE.playSE("reset");
        changeMineNum();
        setClockNum(0);
        setGridSize();
    }


    private void setGridSize()
    {
        float _Yscale = 9f/_YCount;
        float _Xscale = _Yscale;

        float _Yoffset = -4.5f;
        float _Xoffset = _Xcount * 1f / _YCount * _Yoffset;

        _GridTransform.localPosition = new Vector3(_Xoffset, _Yoffset, 0);
        _GridTransform.localScale = new Vector3(_Xscale, _Yscale, 1);

        _Back1Transform.localScale = new Vector3(_Xoffset * -2 + 0.5f, _Yoffset * -2 + 0.5f, 1);
        _Back2Transform.localScale = new Vector3(_Xoffset * -2 - _Xscale, _Yoffset * -2 - _Xscale, 1);
    }
}
