using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    /* Ej de uso del PoolManager:
     * - Crear un gameObject de nombre "PoolManager" y agregarle como componente este script     
     * - Reemplazar : EnemyController enemy = Instantiate(enemyPrefab, position, rotation);
     *   por        : EnemyController enemy = PoolManager.Instance.Get(enemyPrefab, position, rotation);
     * - En este caso la var enemyPrefab est� declarada de tipo EnemyController. Para usar este PoolManager 
     *   solamente se necesita que el prefab sea de un tipo que herede de Component, 
         como cualquier script creado por nosotros que hereda de MonoBehaviour -> Behaviour -> Component
     * - Y en vez de hacer un Destroy del objeto, se reemplaza: Destroy(gameObject);
     *   por: PoolManager.Instance.Release(gameObject);
     * - En el caso de querer usar este PoolManager con prefabs de tipo GameObject, tambi�n se puede usar la funci�n:
     *     public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
     * - NO es necesario realizar ninguna configuraci�n adicional para que el Pool Manager empiece a funcionar.
     *   Si se desea tener un control m�s detallado de cu�ntos objetos tendr� cada pool de cada prefab,
     *   y si estos objetos se van a crear todos en el Start o se van a crear de a uno, se puede configurar 
     *   un nuevo elemento en el array poolData.
    */

    // Ser� singleton
    // Tambi�n se configura que su orden de ejecuci�n sea primero que el resto de los scripts
    private static PoolManager instance = null;
    public static PoolManager Instance => instance;

    [SerializeField] private int defaultCapacity = 10;
    [SerializeField] private int maxSize = 100;
    [SerializeField] private bool defaultCreateObjects = false;
    [SerializeField] private bool collectionCheck = true; // Recordar que esto solo es para el Editor, para ver errores en caso de usar mal el Release
    [SerializeField] private bool forceDestroy = true; //Indica si al ejecutar Release(obj) no existe pool configurado para ese obj, que hago: Destroy(obj) o nada.
    [SerializeField] private Transform defaultParent;
    [SerializeField] private int creationOnStartWaitFrameAfter = 10; // Cada x elementos creados en el pool se descansar� 1 frame.
    [SerializeField] private PoolManagerData data;

    // Cada elemento de este dictionary es un pool para un prefab en particular.
    // Asocia: <ID del prefab == prefab.gameObject.GetInstanceID(), ObjectPool>            
    Dictionary<int, ObjectPool<Component>> pools = new();

    // En la funcion Release: necesito saber cual es el ObjectPool al que pertenece el gameObject instanciado de un prefab
    // Asocia: <ID del gameObject, ObjectPool>    
    Dictionary<int, ObjectPool<Component>> objectPoolLookup = new();

    // Y adem�s en el m�todo Release necesitamos la Component que hay que liberar.    
    // Asocia: <ID del gameObject, Component creada con el Instantiate>    
    Dictionary<int, Component> componentLookup = new();

    // Para asociar cu�l es el �tem de PoolData para crear un pool
    // Asocia: <ID del prefab, PoolData configurada en el Inspector>    
    Dictionary<int, PoolData> poolDataLookup = new();

    // En caso que se active el flag createParent, el Transform del objeto padre creado se podr� accesar con este dictionary
    // Asocia: <ID del prefab, Transform del padre de los objetos que se crean para ese prefab>
    Dictionary<int, Transform> parentLookup = new();

    // Como la funci�n CreateFunc no recibe argumentos, cuando se quiera hacer el Instantiate(prefab, parent)
    // se usar�n estar vars
    Component prefabTemp;
    Transform parentTemp;
    void SetTempVars(Component prefab, Transform parent)
    {
        prefabTemp = prefab;
        parentTemp = parent;
    }

    private void Awake()
    {
        if (!SingletonAwakeValidation())
            return;
        FillPoolDataLookup();
    }

    private void Start()
    {
        StartCoroutine(CreatePoolsModeStartRoutine());
    }

    public T Get<T>(T prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        var parent = GetParentOrCreate(GetPrefabID(prefab));
        var obj = GetFromPool(prefab, parent);
        obj.transform.SetPositionAndRotation(position, rotation);
        return (T)obj;
    }

    public T Get<T>(T prefab, Transform parent) where T : Component
    {
        var obj = GetFromPool(prefab, parent);
        return (T)obj;
    }

    public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        Transform prefabTransform = prefab.transform;
        var comp = Get(prefabTransform, position, rotation);
        return comp.gameObject;
    }

    public bool Release(GameObject obj)
    {
        if (!obj)
            return false; // Un objeto ya fue destruido del pool.

        // Para validar si se est� tratando de hacer Release 2 veces de un objeto ya devuelto al pool:
        //  una validaci�n simple es si el obj ya est� desactivado:
        if (collectionCheck && !obj.activeInHierarchy)
            return false; // Release de un objeto ya desactivado

        var gameObjectID = obj.GetInstanceID();
        if (objectPoolLookup.TryGetValue(gameObjectID, out var pool))
        {
            var component = componentLookup[gameObjectID];
            pool.Release(component);
            return true;
        }
        else
        {
            if (forceDestroy)
                Destroy(obj);
            return false; // Se quiere liberar un objeto que no fue creado por el Pool Manager
        }
    }

    bool SingletonAwakeValidation()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return false;
        }
        return true;
    }

    void FillPoolDataLookup()
    {
        if (!data) return;
        for (int i = 0; i < data.poolData.Length; i++)
        {
            var poolDataItem = data.poolData[i];
            poolDataLookup[GetPrefabID(poolDataItem.prefab)] = poolDataItem;
        }
    }

    int GetPrefabID(Component prefab) => prefab.gameObject.GetInstanceID();

    IEnumerator CreatePoolsModeStartRoutine()
    {
        if (!data) yield break;
        
        for (int i = 0; i < data.poolData.Length; i++)
        {
            var poolDataItem = data.poolData[i];
            if (poolDataItem.createPoolMode == CreatePoolMode.Start)
            {
                Transform parent = GetParentOrCreate(GetPrefabID(poolDataItem.prefab));
                yield return StartCoroutine(CreatePoolRoutine(poolDataItem.prefab, poolDataItem.defaultCapacity, 
                    poolDataItem.maxSize, true, parent));
            }
        }
    }

    Transform GetParentOrCreate(int prefabID)
    {
        if (parentLookup.TryGetValue(prefabID, out var parent))
            return parent;
        else if (!poolDataLookup.TryGetValue(prefabID, out var poolDataItem))
            return defaultParent;
        else if (poolDataItem.createParent)
        {
            GameObject parentObject = new GameObject("Object Pool - " + poolDataItem.prefab.name);
            parent = parentObject.transform;
            parentLookup[prefabID] = parent;
            return parent;
        }
        else
            return defaultParent;
    }

  
    Component GetFromPool(Component prefab, Transform parent)
    {
        var pool = GetPoolOrCreate(prefab);
        SetTempVars(prefab, parent);
        var obj = pool.Get(); 
        obj.gameObject.SetActive(true);
        obj.transform.parent = parent; // Si no se hizo el Instantiate, nos aseguramos de setear correctamente el parent
        return obj;
    }
    
    ObjectPool<Component> GetPoolOrCreate(Component prefab)
    {
        int prefabID = GetPrefabID(prefab);

        if (!pools.TryGetValue(prefabID, out var pool))
        {
            var parent = GetParentOrCreate(prefabID);

            if (poolDataLookup.TryGetValue(prefabID, out var poolDataItem)) // Se valida si tiene configuraci�n en PoolData
            {
                bool createObjects = poolDataItem.createPoolMode == CreatePoolMode.FirstGet
                    // Cuando se dispara una bala que tiene configurado su pool para ser creado en el Start
                    // y la corutina del Start todav�a se est� ejecutando y no se ha creado el pool de las balas
                    || poolDataItem.createPoolMode == CreatePoolMode.Start 
                    || defaultCreateObjects;
                pool = CreatePool(prefab, poolDataItem.defaultCapacity, poolDataItem.maxSize, createObjects, parent);
            }
            else
                pool = CreatePool(prefab, defaultCapacity, maxSize, defaultCreateObjects, parent);
        }

        return pool;
    }

    ObjectPool<Component> CreatePool(Component prefab, int defaultCapacity, int maxSize, bool createObjects, Transform parent)
    {
        int prefabID = GetPrefabID(prefab);
        if (pools.TryGetValue(prefabID, out var pool))
            return pool;
        
        pool = new ObjectPool<Component>(CreateFunc, null, OnReturnedToPool, OnDestroyPoolObject, collectionCheck, 
            defaultCapacity, maxSize); // Se pasa null porque el SetActive(true) se hace en el m�todo GetFromPool
        pools[prefabID] = pool;

        prefab.gameObject.SetActive(false); //Todos los clones quedaran en estado desactivado

        if (createObjects)
            CreateObjectsInPool(prefab, parent, pool, defaultCapacity);

        return pool;
    }

    IEnumerator CreatePoolRoutine(Component prefab, int defaultCapacity, int maxSize, bool createObjects, Transform parent)
    {
        var pool = CreatePool(prefab, defaultCapacity, maxSize, false, parent); // Solo crear el pool, sin ning�n clon

        if (createObjects)
            yield return StartCoroutine(CreateObjectsInPoolRoutine(prefab, parent, pool, defaultCapacity));
    }

    void CreateObjectsInPool(Component prefab, Transform parent, ObjectPool<Component> pool, int defaultCapacity)
    {
        SetTempVars(prefab, parent);
        var objectsCreated = new Component[defaultCapacity];

        for (int i = 0; i < objectsCreated.Length; i++) // Si son muchos objetos se puede notar una peque�a baja en los FPS.
            objectsCreated[i] = pool.Get();

        for (int i = 0; i < objectsCreated.Length; i++)
            pool.Release(objectsCreated[i]);
    }

    IEnumerator CreateObjectsInPoolRoutine(Component prefab, Transform parent, ObjectPool<Component> pool, int defaultCapacity)
    {        
        var objectsCreated = new List<Component>(defaultCapacity);

        for (int i = 0; pool.CountAll < defaultCapacity; i++)
        {            
            SetTempVars(prefab, parent); // Esto tiene que hacerse dentro del for en caso que en el mismo frame otro pool inicie su creaci�n
            objectsCreated.Add(pool.Get()); // Este Get llamar� a un Instantiate

            if (i % creationOnStartWaitFrameAfter == 0)
                yield return null;
        }

        for (int i = 0; i < objectsCreated.Count; i++)
            pool.Release(objectsCreated[i]);
    }
   
    Component CreateFunc()
    {
        var component = Instantiate(prefabTemp, parentTemp);
        int gameObjectID = component.gameObject.GetInstanceID();
        int prefabID = GetPrefabID(prefabTemp);

        objectPoolLookup[gameObjectID] = pools[prefabID]; // Se asocia el pool al que pertenece el objeto reci�n creado
        componentLookup[gameObjectID] = component; // Se asocia la componente reci�n creada al object reci�n creado

        return component;
    }

    void OnReturnedToPool(Component obj) => obj.gameObject.SetActive(false);

    void OnDestroyPoolObject(Component obj) => Destroy(obj.gameObject);
}