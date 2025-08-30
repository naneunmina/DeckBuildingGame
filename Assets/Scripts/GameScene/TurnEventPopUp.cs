using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnEventPopup : MonoBehaviour
{
    [Header("Data")]
    public EventSO[] pool;
    public int[] triggerTurns = {3, 7, 11};
    [SerializeField] TurnManager turnManager;
    [SerializeField] ResourceManager resourceManager;
    [SerializeField] ShopManager shopManager;
    [SerializeField] MacaronManager macaronManager;
    [SerializeField] FacilityManager facilityManager;

    [Header("Result")]
    public EventResultPopup resultPopup;

    [Header("UI")]
    public CanvasGroup root;
    public Image art;
    public TMP_Text txtTitle, txtDesc;
    public TMP_Text txtC1Desc, txtC2Desc;
    public Button btnC1, btnC2;

    public bool pauseOnOpen = true;

    float prevScale;
    EventSO current;

    void Awake()
    {
        HideSelf();
        turnManager.OnTurnChanged.AddListener(MaybeShow);
        btnC1.onClick.AddListener(() => Choose(1));
        btnC2.onClick.AddListener(() => Choose(2));
    }

    public void MaybeShow(int turnNumber)
    {
        if (!triggerTurns.Contains(turnNumber) || pool == null || pool.Length == 0) return;
        current = pool[Random.Range(0, pool.Length)];
        Open(current);
    }

    void Open(EventSO e)
    {
        if (pauseOnOpen) { prevScale = Time.timeScale; Time.timeScale = 0f; }

        if (art)      art.sprite = e.art;
        if (txtTitle) txtTitle.text = e.title;
        if (txtDesc)  txtDesc.text  = e.description;
        if (txtC1Desc)  txtC1Desc.text  = e.choice1Desc;
        if (txtC2Desc)  txtC2Desc.text  = e.choice2Desc;

        root.gameObject.SetActive(true);
        root.alpha = 1f; root.interactable = true; root.blocksRaycasts = true;
    }

    void Choose(int idx)
    {
        EventResult res = default;

        if (current != null)
        {
            if (idx == 1) res = current.OnChoose1(turnManager, resourceManager,shopManager, macaronManager, facilityManager);
            else          res = current.OnChoose2(turnManager, resourceManager,shopManager, macaronManager, facilityManager);
        }

        // 선택 팝업은 숨기되, 시간은 결과 팝업이 닫힐 때까지 계속 멈춤
        HideSelf(keepTimePaused:true);

        // 결과 팝업 표시, 닫히면 시간 재개
        resultPopup.Show(res.art, res.text, closed: ResumeAfterResult);
    }

    void ResumeAfterResult()
    {
        if (pauseOnOpen) Time.timeScale = (prevScale > 0f ? prevScale : 1f);
        current = null;
    }

    void HideSelf(bool keepTimePaused = false)
    {
        root.alpha = 0f; root.interactable = false; root.blocksRaycasts = false;
        root.gameObject.SetActive(false);
        if (!keepTimePaused && pauseOnOpen) Time.timeScale = (prevScale > 0f ? prevScale : 1f);
    }
}
