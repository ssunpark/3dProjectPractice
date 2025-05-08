using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

// �������̽�
public class UI_TouchBounce : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // �߰� ���
    // - �Ǻ� ���� ���
    // - ��ư�� ���� ��� ��ư�� Ȱ��ȭ ��������
    // - ��¡


    // ���콺�� ��ġ�ϸ� ũ�⸦ ����(Ű��ų� �ø��ų�)
    public float StartScale = 1f;
    public float EndScale = 1.2f;
    public float Duration = 0.1f;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnDisable()
    {
        _rectTransform.localScale = new Vector3(StartScale, StartScale, 1f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _rectTransform.DOScale(new Vector3(EndScale, EndScale, 1f), Duration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _rectTransform.DOScale(new Vector3(StartScale, StartScale, 1f), Duration);
    }
}