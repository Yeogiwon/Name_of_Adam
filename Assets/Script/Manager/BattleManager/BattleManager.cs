using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 전투를 담당하는 매니저
// 필드와 턴의 관리
// 필드에 올라와있는 캐릭터의 제어를 배틀매니저에서 담당

public class BattleManager : MonoBehaviour
{
    private static BattleManager s_instance;
    public static BattleManager Instance { get { Init(); return s_instance; } }

    private SoundManager _sound;
    public static SoundManager Sound => Instance._sound;

    [SerializeField] BattleCutSceneController _battlecutScene;
    public static BattleCutSceneController BattleCutScene => Instance._battlecutScene;

    [SerializeField] UnitSpawner _spawner;
    public static UnitSpawner Spawner => Instance._spawner;

    private BattleDataManager _battleData;
    public static BattleDataManager Data => Instance._battleData;

    private BattleUIManager _battleUI;
    public static BattleUIManager BattleUI => Instance._battleUI;

    private PlayerSkillController _playerSkillController;
    public static PlayerSkillController PlayerSkillController => Instance._playerSkillController;

    private Field _field;
    public static Field Field => Instance._field;

    private Mana _mana;
    public static Mana Mana => Instance._mana;

    private PhaseController _phase;
    public static PhaseController Phase => Instance._phase;

    [SerializeField] List<GameObject> Background;

    private void Awake()
    {
        _battleData = Util.GetOrAddComponent<BattleDataManager>(gameObject);
        _battleUI = Util.GetOrAddComponent<BattleUIManager>(gameObject);
        _mana = Util.GetOrAddComponent<Mana>(gameObject);
        _phase = new PhaseController();
        _playerSkillController = Util.GetOrAddComponent<PlayerSkillController>(gameObject);

        SetBackground();
    }

    private void Update()
    {
        _phase.OnUpdate();
    }

    private static void Init()
    {
        if (s_instance == null)
        {
            GameObject go = GameObject.Find("@BattleManager");

            if (go == null)
            {
                //go = new GameObject("@BattleManager");
                //go.AddComponent<BattleManager>();
                return;
            }

            s_instance = go.GetComponent<BattleManager>();
        }
    }

    public void TurnStart()
    {
        foreach (BattleUnit unit in _battleData.BattleUnitList)
        {
            unit.TurnStart();
        }
    }

    public void TurnEnd()
    {
        foreach (BattleUnit unit in _battleData.BattleUnitList)
        {
            unit.TurnEnd();
        }
    }

    public void SetupField()
    {
        GameObject fieldObject = GameObject.Find("Field");

        if (fieldObject == null)
            fieldObject = GameManager.Resource.Instantiate("Field");

        _field = fieldObject.GetComponent<Field>();
    }

    public void SpawnInitialUnit()
    {
        if (SceneChanger.GetSceneName() == "BattleTestScene")
            return;

        PlayAfterCoroutine(() => {
            _spawner.SpawnInitialUnit();
        }, 0.5f);

        PlayAfterCoroutine(() => {
            if (GameManager.Data.Map.GetCurrentStage().StageLevel == 11)
            {
                string scriptKey = "엘리트전_입장_최초";

                EventConversation(scriptKey);
            }
            else
            {
                _phase.ChangePhase(_phase.Prepare);
            }
        }, 1f);
    }

    private void EventConversation(string scriptKey)
    {
        List<Script> scripts = GameManager.Data.ScriptData[scriptKey];
        GameManager.UI.ShowPopup<UI_Conversation>().Init(scripts, true, true);
    }

    private void SetBackground()
    {
        // string str = GameManager.Data.CurrentStageData.FactionName;

        for (int i = 0; i < 3; i++)
        {
            Background[i].SetActive(false);
            
            // if (((Faction)i + 1).ToString() == str)
            if (i == 0)
                Background[i].SetActive(true);
        }
    }

    #region Click 관련
    public void OnClickTile(Tile tile)
    {
        Vector2 coord = _field.FindCoordByTile(tile);
        _phase.OnClickEvent(coord);
    }

    public void PreparePhaseClick(Vector2 coord)
    {
        if (!_field.ColoredTile.Contains(coord))
        {
            _battleUI.CancelAllSelect();
            return;
        }

        if (_field.FieldType == FieldColorType.UnitSpawn)
        {
            SpawnUnitOnField(coord);
        }
        else if (_field.FieldType == FieldColorType.PlayerSkill)
        {
            _battleUI.GetSelectedPlayerSkill().Use(coord);
            _playerSkillController.PlayerSkillUse();
        }
        else if (_field.FieldType == FieldColorType.UltimatePlayerSkill)
        {
            if (GameManager.Data.PlayerSkillCountChage(-1))
            {
                _battleUI.GetSelectedPlayerSkill().Use(coord);
                _playerSkillController.PlayerSkillUse();
            }
        }
    }

    private void SpawnUnitOnField(Vector2 coord)
    {
        DeckUnit unit = _battleUI.UI_hands.GetSelectedUnit();
        if (!_field.ColoredTile.Contains(coord))
            return;

        _mana.ChangeMana(-unit.DeckUnitTotalStat.ManaCost); //마나 사용가능 체크

        unit.FirstTurnDiscountUndo();

        _spawner.DeckSpawn(unit, coord);
        GameManager.VisualEffect.StartVisualEffect(
            Resources.Load<AnimationClip>("Arts/EffectAnimation/VisualEffect/UnitSpawnBackEffect"),
            _field.GetTilePosition(coord) + new Vector3(0f, 3.5f, 0f));
        GameManager.VisualEffect.StartVisualEffect(
            Resources.Load<AnimationClip>("Arts/EffectAnimation/VisualEffect/UnitSpawnFrontEffect"),
            _field.GetTilePosition(coord) + new Vector3(0f, 3.5f, 0f));

        _battleUI.RemoveHandUnit(unit); //유닛 리필
        GameManager.UI.ClosePopup();
        _field.ClearAllColor();
    }

    public void MovePhaseClick(Vector2 coord)
    {
        BattleUnit unit = _battleData.GetNowUnit();
        BattleUnit destunit = _field.GetUnit(coord);

        if (coord == unit.Location)
            return;

        if (destunit != null && unit.Team != destunit.Team)
        {
            return;
        }

        if (unit.Team == Team.Player)
        {
            if (!_field.ColoredTile.Contains(coord))
                return;

            Vector2 dest = coord - unit.Location;
            MoveLocate(unit, dest);
        }

        _phase.ChangePhase(_phase.Action);
    }

    public void ActionPhaseClick(Vector2 coord)
    {
        if (!_field.ColoredTile.Contains(coord))
            return;

        BattleUnit unit = _battleData.GetNowUnit();

        if (coord != unit.Location)
        {
            List<Vector2> splashRange = unit.GetSplashRange(coord, unit.Location);
            List<BattleUnit> unitList = new();

            foreach (Vector2 splash in splashRange)
            {
                BattleUnit targetUnit = _field.GetUnit(coord + splash);

                if (targetUnit == null)
                    continue;

                // 힐러의 예외처리 필요
                if (targetUnit.Team != unit.Team)
                    unitList.Add(targetUnit);
            }

            if (unitList.Count == 0)
                return;

            unit.Action.ActionStart(unit, unitList);
        }
    }

    #endregion

    public void AttackStart(BattleUnit caster, BattleUnit hit)
    {
        List<BattleUnit> hits = new ();
        hits.Add(hit);

        AttackStart(caster, hits);
    }

    public void AttackStart(BattleUnit caster, List<BattleUnit> hits)
    {
        BattleCutSceneData CSData = new(caster, hits);
        _battlecutScene.InitBattleCutScene(CSData);

        StartCoroutine(_battlecutScene.AttackCutScene(CSData));
    }

    public void EndUnitAction()
    {
        _field.ClearAllColor();
        _battleData.BattleOrderRemove(Data.GetNowUnit());
        _battleUI.UI_darkEssence.Refresh();
        _phase.ChangePhase(_phase.Engage);
    }

    public void StigmaSelectEvent(Corruption cor)
    {
        BattleUnit targetUnit = cor.GetTargetUnit();

        if (!targetUnit.Fall.IsEdified)
        {
            GameManager.UI.ShowPopup<UI_StigmaSelectButtonPopup>().Init(targetUnit.DeckUnit, null, 2, cor.LoopExit);
        }
        else
        { 
            cor.LoopExit();
        }
    }

    public void DirectAttack()
    {
        //핸드에 있는 유닛을 하나 무작위로 제거하고 배틀 종료 체크
        Debug.Log("Direct Attack");

        if (_battleData.PlayerHands.Count == 0)
        {
            BattleOverCheck();
            return;
        }

        int randNum = UnityEngine.Random.Range(0, Data.PlayerHands.Count);
        _battleUI.RemoveHandUnit(Data.PlayerHands[randNum]);

        BattleOverCheck();
    }

    public void UnitDeadEvent(BattleUnit unit)
    {
        _battleData.BattleUnitList.Remove(unit);
        _field.ExitTile(unit.Location);

        if (unit.IsConnectedUnit)
            return;

        _battleData.BattleOrderRemove(unit);

        if (unit.Team == Team.Enemy && !unit.IsConnectedUnit)
        {
            GameManager.Data.DarkEssenseChage(unit.Data.DarkEssenseDrop);
        }

        foreach (BattleUnit fieldUnit in _battleData.BattleUnitList)
        {
            fieldUnit.FieldUnitDdead();
        }
    }

    public void BattleOverCheck()
    {
        if (SceneChanger.GetSceneName() == "BattleTestScene")
            return;

        int MyUnit = 0;
        int EnemyUnit = 0;

        foreach (BattleUnit unit in Data.BattleUnitList)
        {
            if (unit.Team == Team.Player)//아군이면
                MyUnit++;
            else
                EnemyUnit++;
        }

        MyUnit += _battleData.PlayerDeck.Count;
        MyUnit += _battleData.PlayerHands.Count;

        if (MyUnit == 0)
        {
            BattleOverLose();
        }
        else if (EnemyUnit == 0)
        {
            BattleOverWin();
            if (GameManager.Data.StageAct == 0 && GameManager.Data.Map.CurrentTileID == 99)
            {
                if (GameManager.Instance.Tutorial_Stage_Trigger == true)
                {
                    GameObject.Find("UI_Tutorial").GetComponent<UI_Tutorial>().TutorialActive(14);
                    GameManager.Instance.Tutorial_Stage_Trigger = false;
                }
            }
        }
    }

    private void BattleOverWin()
    {
        Debug.Log("YOU WIN");
        _battleData.OnBattleOver();
        _phase.ChangePhase(new BattleOverPhase());
        StageData data = GameManager.Data.Map.GetCurrentStage();
        try
        {
            if (data.StageLevel >= 10)
            {
                GameManager.UI.ShowScene<UI_BattleOver>().SetImage("elite win");
                GameManager.SaveManager.DeleteSaveData();
            }
            else
            {
                GameManager.UI.ShowScene<UI_BattleOver>().SetImage("win");
            }
                
        }
        catch
        {
            GameManager.UI.ShowScene<UI_BattleOver>().SetImage("win");
        }
    }

    private void BattleOverLose()
    {
        Debug.Log("YOU LOSE");
        _phase.ChangePhase(new BattleOverPhase());
        GameManager.UI.ShowScene<UI_BattleOver>().SetImage("lose");
        GameManager.SaveManager.DeleteSaveData();
        GameManager.Data.DeckClear();
    }

    // 이동 경로를 받아와 이동시킨다
    private void MoveLocate(BattleUnit caster, Vector2 coord)
    {
        _field.MoveUnit(caster.Location, caster.Location + coord);
        GameManager.Sound.Play("Move/MoveSFX");

        foreach (ConnectedUnit unit in caster.ConnectedUnits)
        {
            _field.MoveUnit(unit.Location, unit.Location + coord);
        }
    }

    public bool UnitSpawnReady(FieldColorType colorType)
    {
        if (_phase.Current != _phase.Prepare)
            return false;

        if (colorType == FieldColorType.none)
            _field.ClearAllColor();
        else
            _field.SetSpawnTileColor(colorType);

        return true;
    }

    public void BenedictionCheck()
    {
        BattleUnit lastUnit = null;

        foreach (BattleUnit unit in Data.BattleUnitList)
        {
            if (unit.Team == Team.Enemy)
            {
                if (lastUnit == null)
                {
                    lastUnit = unit;
                }
                else
                {
                    lastUnit = null;
                    break;
                }
            }
        }

        if (lastUnit != null)
        {
            if (lastUnit.Buff.CheckBuff(BuffEnum.Benediction))
                return;

            if(GameManager.Data.StageAct == 0 && GameManager.Data.Map.CurrentTileID == 1)
                return;

            if(GameManager.Data.StageAct == 0 && GameManager.Instance.Tutorial_Benediction_Trigger == true)
            {
                GameObject.Find("UI_Tutorial").GetComponent<UI_Tutorial>().TutorialActive(13);
                GameManager.Instance.Tutorial_Benediction_Trigger = false;
            }
            Buff_Benediction benediction = new();
            lastUnit.SetBuff(benediction, lastUnit);
        }
    }

    public void PlayAfterCoroutine(Action action, float time) => StartCoroutine(PlayCoroutine(action, time));

    private IEnumerator PlayCoroutine(Action action, float time)
    {
        yield return new WaitForSeconds(time);

        action();
    }
}