using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    public UI_Hands UI_hands;
    public UI_PlayerSkill UI_playerSkill;
    public UI_DarkEssence UI_darkEssence;
    public UI_ManaGauge UI_manaGauge;

    public UI_WaitingLine UI_waitingLine;
    public UI_TurnCount UI_turnCount;
    public UI_TurnNotify UI_turnNotify;

    private UI_TurnChangeButton _turnChangeButton;

    private const int _maxHandCount = 3;

    private void Start()
    {
        //버튼 생성
        GameManager.UI.ShowScene<UI_DeckButton>().Set(false/*true*/);

        //정보들
        UI_waitingLine = GameManager.UI.ShowScene<UI_WaitingLine>();
        //UI_turnCount = GameManager.UI.ShowScene<UI_TurnCount>();
        UI_turnNotify = GameManager.UI.ShowScene<UI_TurnNotify>();


        //컨트롤바
        _turnChangeButton = GameManager.UI.ShowScene<UI_TurnChangeButton>();
        UI_ControlBar control = GameManager.UI.ShowScene<UI_ControlBar>();

        UI_playerSkill = control.UI_PlayerSkill;
        UI_playerSkill.SetSkill(GameManager.Data.GetPlayerSkillList());
        UI_hands = control.UI_Hands;
        UI_darkEssence = control.UI_DarkEssence;
        UI_manaGauge = control.UI_ManaGauge;

        FillHand();
    }

    public void RefreshWaitingLine(List<BattleUnit> orderList)
    {
        UI_waitingLine.SetWaitingLine(orderList);
    }

    public void AddHandUnit(DeckUnit unit)
    {
        BattleManager.Data.PlayerHands.Add(unit);
        UI_hands.AddUnit(unit);
    }

    public void RemoveHandUnit(DeckUnit unit)
    {
        BattleManager.Data.PlayerHands.Remove(unit);
        UI_hands.RemoveUnit(unit);
        FillHand();
    }

    public void FillHand()
    {
        int curID = GameManager.Data.Map.CurrentTileID;

        while (BattleManager.Data.PlayerHands.Count < _maxHandCount)
        {
            DeckUnit unit;
            if (curID == 1 && GameManager.Data.StageAct == 0)
            {
                unit = BattleManager.Data.GetUnitFromDeck();
            }
            else if (curID == 2 && GameManager.Data.StageAct == 0)
            {
                unit = BattleManager.Data.GetTUnitFromDeck();
            }
            else
            {
                unit = BattleManager.Data.GetRandomUnitFromDeck();
            }

            if (unit == null)
                return;
            AddHandUnit(unit);
        }
    }

    public void RefreshHand()
    {
        UI_hands.RefreshCard();
    }

    public PlayerSkill GetSelectedPlayerSkill() => UI_playerSkill.GetSelectedCard().GetSkill();

    public void CancelAllSelect()
    {
        BattleManager.Field.ClearAllColor();
        UI_hands.CancelSelect();
        UI_playerSkill.CancelSelect();
    }

    public void ShowTutorial()
    {
        PhaseController phaseController = BattleManager.Phase;
        int curID = GameManager.Data.Map.CurrentTileID;

        UI_Tutorial UItutorial = GameObject.Find("UI_Tutorial").GetComponent<UI_Tutorial>();

        if (curID == 1 && GameManager.Data.StageAct == 0)
        {
            if (GameManager.Instance.Tutorial_Trigger_First == true)
            {
                if (phaseController.Current == phaseController.Prepare)
                {
                    UItutorial.TutorialActive(0);
                }
                else if (phaseController.Current == phaseController.Engage)
                {
                    UItutorial.TutorialActive(4);
                    GameManager.Instance.Tutorial_Trigger_First = false;
                }
            }
        }
        else if (curID == 2 && GameManager.Data.StageAct == 0)
        {
            if (GameManager.Instance.Tutorial_Trigger_Second == true)
            {
                if (phaseController.Current == phaseController.Prepare)
                {
                    UItutorial.TutorialActive(8);
                }
                else if (phaseController.Current == phaseController.Engage)
                {
                    UItutorial.TutorialActive(12);
                    GameManager.Instance.Tutorial_Trigger_Second = false;
                }
            }
        }
    }

    public void tutorialHandUnit()
    {
        
    }
    

    private List<UI_Info> infoList = new();

    public UI_Info ShowInfo()
    { 
        UI_Info ui_Info = GameManager.UI.ShowScene<UI_Info>();
        ui_Info.GetComponent<Canvas>().sortingOrder = infoList.Count + 1;
        infoList.Add(ui_Info);
        return ui_Info;
    }

    public void CloseInfo(UI_Info close)
    {
        foreach (UI_Info info in infoList)
        {
            if (info == close)
            {
                info.InfoDestroy();
                infoList.Remove(info);

                break;
            }
        }

        InfoOrdering();
    }

    private void InfoOrdering()
    {
        for (int i = 0; i < infoList.Count; i++)
        {
            infoList[i].GetComponent<Canvas>().sortingOrder = i + 1;
        }
    }
}