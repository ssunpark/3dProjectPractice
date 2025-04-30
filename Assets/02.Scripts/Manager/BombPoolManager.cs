using System.Collections.Generic;
using UnityEngine;

public class BombPoolManager : MonoBehaviour
{
    public static BombPoolManager Instance { get; private set; }

    [Header("∆¯≈∫ «Æ")]
    public GameObject bombPrefab;
    public int bombPoolSize = 10;
    private Queue<GameObject> bombPool = new Queue<GameObject>();

    [Header("¿Ã∆Â∆Æ «Æ")]
    public GameObject vfxPrefab;
    public int vfxPoolSize = 10;
    private Queue<GameObject> vfxPool = new Queue<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitPool(bombPrefab, bombPool, bombPoolSize);
        InitPool(vfxPrefab, vfxPool, vfxPoolSize);
    }

    private void InitPool(GameObject prefab, Queue<GameObject> pool, int size)
    {
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetBomb()
    {
        if (bombPool.Count > 0)
        {
            GameObject bomb = bombPool.Dequeue();
            bomb.SetActive(true);
            return bomb;
        }
        return null;
    }

    public void ReturnBomb(GameObject bomb)
    {
        bomb.SetActive(false);
        bombPool.Enqueue(bomb);
    }

    public GameObject GetVFX()
    {
        if (vfxPool.Count > 0)
        {
            GameObject vfx = vfxPool.Dequeue();
            vfx.SetActive(true);
            return vfx;
        }
        return null;
    }

    public void ReturnVFX(GameObject vfx)
    {
        vfx.SetActive(false);
        vfxPool.Enqueue(vfx);
    }
}
