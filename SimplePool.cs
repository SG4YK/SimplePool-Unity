using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace SimplePool
{
    public delegate void Method(Transform obj, [CanBeNull] string[] args);

    public class SimplePool : MonoBehaviour
    {
        public int m_defaultAmount = 10;
        public int m_maxAmount = 10;
        public string m_poolName = "DefaultPool";
        private Queue<Transform> m_queue;
        private HashSet<int> m_spawned;
        public Transform prefab;

        private void Start()
        {
            m_queue = new Queue<Transform>(m_maxAmount <= 0 ? 100 : m_maxAmount);
            m_spawned = new HashSet<int>();
        }

        public Transform Spawn(Vector3 position, [CanBeNull] Method onSpawned = null, [CanBeNull] string[] args = null)
        {
            Transform obj;
            if (m_queue.Count > 0)
            {
                obj = m_queue.Dequeue();
            }
            else
            {
                obj = Instantiate(prefab);
                m_spawned.Add(obj.GetHashCode());
            }

            obj.position = position;
            if (onSpawned != null) onSpawned(obj, args);
            obj.gameObject.SetActive(true);
            return obj;
        }

        public bool isSpawned(Transform obj)
        {
            return m_spawned.Contains(obj.GetHashCode());
        }

        public bool Despawn(Transform obj, [CanBeNull] Method onDespawned = null, [CanBeNull] string[] args = null)
        {
            if (!isSpawned(obj))
            {
                Debug.Log("Game object " + obj.gameObject.name + " does not belong to pool [" + m_poolName + "].");
                return false;
            }

            if (m_maxAmount > 0 && m_queue.Count >= m_maxAmount)
            {
                Debug.Log("Pool " + m_poolName + " is full.");
                Destroy(obj.gameObject);
                return true;
            }

            obj.gameObject.SetActive(false);
            if (onDespawned != null) onDespawned(obj, args);

            m_queue.Enqueue(obj);
            return true;
        }

        /**
         * 
         */
        public void Clear()
        {
            m_queue.Clear();
            m_spawned.Clear();
        }

        public void Reset()
        {
            Clear();
            Init();
        }

        protected void Init()
        {
            for (var i = 0; i < m_defaultAmount; i++)
            {
                var obj = Instantiate(prefab);
                obj.gameObject.SetActive(false);
                m_spawned.Add(obj.GetHashCode());
                m_queue.Enqueue(obj);
            }
        }
    }
}