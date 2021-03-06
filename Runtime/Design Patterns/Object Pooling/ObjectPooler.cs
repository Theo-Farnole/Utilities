using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Lortedo.Utilities.Pattern
{
    /// <summary>
    /// Manage Object Pooling.
    /// </summary>
    public class ObjectPooler : Singleton<ObjectPooler>
    {
        #region Fields
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;

            public Pool(string tag, GameObject prefab)
            {
                this.tag = tag;
                this.prefab = prefab;
            }
        }

        private const string debugLogHeader = "<color=green>Object Pooler</color> : ";
        [SerializeField] private List<Pool> _prefabsPool; // the pool user inputs'll set as a dictonary

        private Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();
        #endregion

        #region Methods
        #region Mono Callbacks
        void Awake()
        {
            // avoid error if modification of list while browsing it
            var prefabPool = _prefabsPool;

            // create Dictionnary from Pools Array (because Unity doesn't Dictionnary)
            for (int i = 0; i < prefabPool.Count; i++)
            {
                CreatePool(prefabPool[i].tag, null);
            }
        }
        #endregion

        #region Public methods
        public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation, bool createPoolIfDontExist = false)
        {
            string tag = GetTagFromPrefab(prefab);

            if (createPoolIfDontExist && !_pools.ContainsKey(tag))
                CreatePool(tag, prefab);

            return SpawnFromPool(tag, position, rotation);
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!_pools.ContainsKey(tag))
            {
                Debug.LogWarning(debugLogHeader + "Pool with tag " + tag + " doesn't exist.");
                return null;
            }

            if (_pools[tag].Count == 0) InstantiateOneItem(tag);

            GameObject objectToSpawn = _pools[tag].Dequeue();
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            objectToSpawn.SetActive(true);

            foreach (IPooledObject pooledObject in objectToSpawn.GetComponents<IPooledObject>())
            {
                pooledObject.OnObjectSpawn();
                pooledObject.ObjectTag = tag;
            }

            return objectToSpawn;
        }

        public void EnqueueGameObject(GameObject prefab, GameObject toEnqueue, bool createPoolIfDontExist = false)
        {
            string tag = GetTagFromPrefab(prefab);

            if (createPoolIfDontExist && !_pools.ContainsKey(tag))
                CreatePool(tag, prefab);

            EnqueueGameObject(tag, toEnqueue);
        }

        public void EnqueueGameObject(string tag, GameObject toEnqueue)
        {
            Assert.IsTrue(_pools.ContainsKey(tag), string.Format(debugLogHeader + "Pool {0} don't exists.", tag));

            if (_pools[tag].Contains(toEnqueue))
            {
                Debug.LogWarning(debugLogHeader + "Enqueuing an already enqued game object. Aborting");
                return;
            }

            toEnqueue.SetActive(false);
            _pools[tag].Enqueue(toEnqueue);

#if UNITY_EDITOR
            if (Debugging.DynamicsObjects.Instance != null)
                Debugging.DynamicsObjects.Instance.SetToParent(toEnqueue.transform, tag + "_pool");
            else
#endif
                toEnqueue.transform.parent = null;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// If pool empty, instantiate a prefab.
        /// </summary>
        private void InstantiateOneItem(string tag)
        {
            if (!_pools.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exists.");
                return;
            }

            GameObject prefab = Instantiate(_prefabsPool.First(x => x.tag == tag).prefab);

            if (prefab == null) return;

            EnqueueGameObject(tag, prefab);
        }

        private string GetTagFromPrefab(GameObject prefab)
        {
            Pool pool = _prefabsPool.Where(x => x.prefab == prefab).FirstOrDefault();
            return pool != null ? pool.tag : prefab.name;
        }

        private void CreatePool(string tag, GameObject prefab)
        {
            if (_pools.ContainsKey(tag))
            {
                Debug.LogErrorFormat("Cannot create new pool because key '{0}' already exist");
                return;
            }

            if (_prefabsPool.Select(x => x.prefab).Contains(prefab))
            {
                Debug.LogWarningFormat("You are creating a new pool. However, the prefab already exist in pool of tag {0}.", _prefabsPool.Where(x => x.prefab).First().tag);
            }

            _pools.Add(tag, new Queue<GameObject>());

            // should we create a new pool ?
            Pool poolWithTagFromArg = _prefabsPool.Where(x => x.tag == tag).FirstOrDefault();
            if (poolWithTagFromArg == null)
                _prefabsPool.Add(new Pool(tag, prefab));

            Debugging.DynamicsObjects.Instance?.AddParent(tag + "_pool");
        }

        bool DoPoolExist(GameObject prefab)
        {
            return DoPoolExist(GetTagFromPrefab(prefab));
        }

        bool DoPoolExist(string tag)
        {
            return _pools.ContainsKey(tag);
        }
        #endregion
        #endregion
    }
}
