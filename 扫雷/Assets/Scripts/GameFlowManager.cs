using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameFlowManager : MonoBehaviour
{
    public TileFliper _TF = null;
    public MineMaker _MM = null;

    public SEPlayer _SE;

    public Transform _GridTransform;
    public Transform _BorderTransform;
    public Transform _BackgroundTransform;

    public Text _MineNumText;

    public int _XCount = 9, _YCount = 9;
    public int _MineCount = 10;
    private int _MineCountMark = 0;

    public GameObject _DifficultyGO;

    public Transform _SpriteFather;

    public static GameFlowManager _Main = null;

    public GameObject _MineClickPrefab;
    public GameObject _MineMarkedPrefab;
    public GameObject _MineCorrectPrefab;

    public Vector3 _FinalClickTilePos;

    public enum GameState
    {
        WAIT,
        RUNNING,
        END,
        WIN,
        CHANGE
    }

    private GameState _State = GameState.WAIT;
    private GameState _StateCache;

    private bool _CoolDown = true;

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
    }

    private void Start()
    {
        resetGame();
    }

    private void Update()
    {
        if(_State == GameState.WAIT)
        {
            if (!_CoolDown)
            {
                _CoolDown = true;
                return;
            }
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (_MM.isPosInCells(pos))
                {
                    _SE.playSE("click");
                    _MM.makeMines(pos, _XCount, _YCount, _MineCount);
                    _TF.clickTile(pos);
                    _State = GameState.RUNNING;
                    GameTimer._Main.startTimer();
                    checkWinState();
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                _SE.playSE("mark");
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _TF.changeMarking(pos);
                _MineCountMark = _TF.getMineMarkNum();
                _MineNumText.text = "" + (_MineCount - _MineCountMark);
            }
        }
        else if (_State == GameState.RUNNING)
        {
            if (Input.GetMouseButtonUp(0))
            {
                _SE.playSE("click");
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _TF.clickTile(pos);
                checkWinState();
            }
            else if (Input.GetMouseButtonUp(1))
            {
                _SE.playSE("mark");
                Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _TF.changeMarking(pos);
                _MineCountMark = _TF.getMineMarkNum();
                _MineNumText.text = "" + (_MineCount - _MineCountMark);
            }
        }
        else if (_State == GameState.CHANGE)
        {
            if (Input.GetMouseButtonUp(1))
            {
                _DifficultyGO.SetActive(false);
                _State = _StateCache;
            }
        }
    }

    private void checkWinState()
    {
        if (_State == GameState.RUNNING && _TF.getTileNum() <= _MineCount)
        {
            winGame();
        }
    }

    
    // 获取翻开区域
    public Vector3Int[] getFlipArea(Vector3Int clickPos)
    {
        List<Vector3Int> vps = new List<Vector3Int>();
        List<Vector3Int> waitList = new List<Vector3Int>();
        List<Vector3Int> checkedList = new List<Vector3Int>();
        
        if (!_MM.isMineByPos(clickPos))
        {
            if (_MM.isEmptyByPos(clickPos))
            {
                waitList.Add(clickPos);
                int loopNum = 0;
                while (waitList.Count > 0 || loopNum > 5000)
                {
                    loopNum++;
                    vps.Add(waitList[0]);
                    Vector3Int[] neighbours = new Vector3Int[8] {
                        new Vector3Int(waitList[0].x+1,waitList[0].y,0), new Vector3Int(waitList[0].x-1, waitList[0].y, 0),
                        new Vector3Int(waitList[0].x, waitList[0].y+1, 0), new Vector3Int(waitList[0].x, waitList[0].y-1, 0),
                        new Vector3Int(waitList[0].x-1,waitList[0].y-1,0), new Vector3Int(waitList[0].x-1, waitList[0].y+1, 0),
                        new Vector3Int(waitList[0].x+1, waitList[0].y-1, 0), new Vector3Int(waitList[0].x+1, waitList[0].y+1, 0)};
                    for(int i=0; i<neighbours.Length; i++)
                    {
                        if (!checkedList.Contains(neighbours[i]))
                        {
                            checkedList.Add(neighbours[i]);
                            if (_MM.isEmptyByPos(neighbours[i]))
                            {
                                waitList.Add(neighbours[i]);
                                vps.Add(neighbours[i]);
                            }
                            else if (!_MM.isMineByPos(neighbours[i]))
                            {
                                vps.Add(neighbours[i]);
                            }
                        }
                    }
                    waitList.RemoveAt(0);
                }
            }
        }
        else
        {
            endGame();
        }

        return vps.ToArray();
    }
    

    // 游戏胜利
    public void winGame()
    {
        _State = GameState.WIN;
        _TF.clearAllTilesWhenReady();
        _SE.playSE("win");
        GameTimer._Main.stopTimer();
        markAllMine(_MineCorrectPrefab);
    }


    // 结束游戏
    public void endGame()
    {
        _State = GameState.END;
        _TF.clearAllTilesWhenReady();
        _SE.playSE("explode");
        GameTimer._Main.stopTimer();

        respawnSpriteAt(_FinalClickTilePos, _MineClickPrefab);
        markAllMarkedMinePos(_MineMarkedPrefab);
    }

    public void changeDifficluty()
    {
        _StateCache = _State;
        _DifficultyGO.SetActive(true);
        _State = GameState.CHANGE;
        _SE.playSE("click");
    }

    public void newGame(int difficulty)
    {
        switch (difficulty)
        {
            case 0: makeNewGame(9, 9, 10); break;
            case 1: makeNewGame(16, 16, 40); break;
            case 2: makeNewGame(32, 16, 99); break;
            default: newGameRand(); break;
        }
    }

    public void newGameRand()
    {
        _DifficultyGO.SetActive(false);
        _YCount = Random.Range(9, 16);
        _XCount = Random.Range(_YCount/2,_YCount*2);
        _MineCount = Random.Range(1, Mathf.Min((_XCount * _YCount) / 2, _XCount * _YCount - 9));
        makeNewGame(_XCount,_YCount,_MineCount);
    }

    public void makeNewGame(int x, int y, int m)
    {
        _XCount = x;
        _YCount = y;
        _MineCount = m;
        _DifficultyGO.SetActive(false);
        resetGame();
        _CoolDown = false;
    }


    // 重新开始游戏
    public void resetGame()
    {
        clearAllSprites();
        _SE.playSE("reset");
        _State = GameState.END;
        _TF.makeField(_XCount, _YCount);
        _MM.emptyMineField(_XCount, _YCount);
        float scale;
        scale = 9f / _YCount;
        Vector3 vp = _GridTransform.transform.position;
        vp.x = _XCount * scale / -2f;
        _GridTransform.transform.position = vp;
        Vector3 vp2 = _BorderTransform.localScale;
        Vector3 vp3 = _BackgroundTransform.localScale;
        vp2.x = vp.x * -2f + 0.5f;
        vp3.x = vp.x * -2f - 0.5f * scale;
        _BorderTransform.localScale = vp2;
        _BackgroundTransform.localScale = vp3;

        _MineCountMark = 0;
        _MineNumText.text = "" + (_MineCount - _MineCountMark);

        _GridTransform.localScale = new Vector3(scale, scale, 1);
        _State = GameState.WAIT;
        GameTimer._Main.clear();
    }

    private void markAllMarkedMinePos(GameObject go)
    {
        Vector3Int[] vpis = _TF.getAllMarkedPos();
        for(int i=0; i<vpis.Length; i++)
        {
            respawnSpriteAt(_TF.getWorldPos(vpis[i]), go);
        }
    }

    private void markAllMine(GameObject go)
    {
        Vector3[] vps = _MM.getAllMineWorldPos();
        for(int i=0; i<vps.Length; i++)
        {
            respawnSpriteAt(vps[i], go);
        }
    }


    private void respawnSpriteAt(Vector3 pos, GameObject spritePrefab)
    {
        GameObject go = Instantiate(spritePrefab, pos,Quaternion.identity,_SpriteFather);
        go.transform.localScale = _GridTransform.localScale;
        go.transform.position += _GridTransform.localScale/2f;
    }

    private void clearAllSprites()
    {
        int cn = _SpriteFather.childCount;
        _SpriteFather.gameObject.SetActive(false);
        for (int i=0; i<cn; i++)
        {
            Destroy(_SpriteFather.GetChild(cn-i-1).gameObject);
        }
        _SpriteFather.gameObject.SetActive(true);
    }
}
