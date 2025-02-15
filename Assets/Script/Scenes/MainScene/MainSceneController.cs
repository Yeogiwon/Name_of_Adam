using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    [SerializeField] GameObject Canvas;
    [SerializeField] GameObject ContinueBox;

    private void Start()
    {
        if (GameManager.SaveManager.SaveFileCheck())
            ContinueBox.SetActive(true);
        else
            ContinueBox.SetActive(false);
    }

    public void StartButton()
    {
        // 게임오브젝트를 생성해서 보내주기 & 생성한 오브젝트가 맵 선택 씬에 도달했을 때 활성화되서 튜토 이미지 띄우고 자신 삭제하기
        GameManager.Data.DeckClear();
        Destroy(GameManager.Instance.gameObject);

        GameManager.SaveManager.DeleteSaveData();
        SceneChanger.SceneChange("CutScene");
    }

    public void ContinueBotton()
    {
        if (GameManager.SaveManager.SaveFileCheck())
            SceneChanger.SceneChange("StageSelectScene");
    }

    public void OptionButton()
    {
        UI_Option go = GameManager.UI.ShowPopup<UI_Option>();
        //GameObject go = Resources.Load<GameObject>("Prefabs/UI/Popup/UI_Option");
        //GameObject.Instantiate(go, Canvas.transform);
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}