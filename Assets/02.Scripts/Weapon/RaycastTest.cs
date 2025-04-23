using UnityEngine;

public class RaycastTest : MonoBehaviour
{
    [SerializeField]
    private LineRendererAtoB visualizerLine;
    [SerializeField]
    private Transform owner;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(owner.position, ray.direction, out RaycastHit hit, Mathf.Infinity))
        {
            visualizerLine.Play(owner.position, hit.point);
        }
        else
        {
            visualizerLine.Stop();
        }
    }
}
