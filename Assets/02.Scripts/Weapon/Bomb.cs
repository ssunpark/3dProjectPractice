using UnityEngine;

public class Bomb : MonoBehaviour
{
    // ��ǥ: ���콺�� ������ ��ư�� ������ ī�޶� �ٶ󺸴� �������� ����ź�� ������ �ʹ�.
    // 1. ����ź ������Ʈ �����
    // 2. ������ ��ư �Է� �ޱ�
    // 3. �߻� ��ġ�� ����ź �����ϱ�
    // 4. ������ ����ź�� ī�޶� �������� �������� �� ���ϱ�

    public GameObject VfxPrefab;

    // �浹���� ��
    private void OnCollisionEnter(Collision collision)
    {
        // 1) �浹 ��ġ�� VFX ����
        Instantiate(VfxPrefab, transform.position, Quaternion.identity);

        // 2) ����ź ������Ʈ�� ���� (��� Ȥ�� ȿ�� �Ŀ�)
        gameObject.SetActive(false);
    }
}
