using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class UI_TowerInteract : MonoBehaviour
{
    public event Action OnDestroyClicked;

    [Header("UI 구성요소")]
    [SerializeField] private RectTransform menuRect;
    [SerializeField] private Button destroyButton;  // 해체 버튼

    [Header("설정")]
    [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

    private Camera mainCam;
    private Vector3 targetWorldPos;
    private bool isShowing = false;

    private void Awake()
    {
        mainCam = Camera.main;

        destroyButton.onClick.AddListener(() => { OnDestroyClicked?.Invoke(); });

        Hide();
    }

    private void Update()
    {
        // 타워의 3D 월드 좌표를 2D 화면(Screen) 좌표로 변환해서 따라다님
        if (isShowing)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(targetWorldPos + offset);
            menuRect.position = screenPos;
        }
    }

    public void Show(Vector3 worldPos)
    {
        targetWorldPos = worldPos;
        isShowing = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        isShowing = false;
        gameObject.SetActive(false);
    }
}
