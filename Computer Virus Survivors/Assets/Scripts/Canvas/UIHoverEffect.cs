using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float hoverDistance = 10f; // 위로 이동할 거리
    public float moveSpeed = 5f;      // 이동 속도

    private Vector2 originalPosition; // 초기 위치 저장 (Vector2)
    private bool isHovered = false;   // 마우스가 위에 있는지 여부
    private RectTransform rectTransform; // 자식 UI의 RectTransform

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        // 현재 위치 저장
        originalPosition = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // 목표 위치 계산
        Vector2 targetPosition = isHovered
            ? originalPosition + Vector2.up * hoverDistance
            : originalPosition;

        // 부드럽게 위치 이동
        rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}