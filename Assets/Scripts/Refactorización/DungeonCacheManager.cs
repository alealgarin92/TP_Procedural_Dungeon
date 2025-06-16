using UnityEngine;

public class DungeonCache : MonoBehaviour
{
    public static DungeonCache Instance;

    [Header("Datos cacheados")]
    public BoardManager BoardManagerCache;
    public bool DungeonAlreadyGenerated = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No se destruye al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Evita duplicados
        }
    }

    public void Clear()
    {
        DungeonAlreadyGenerated = false;
        BoardManagerCache = null;
    }
}

