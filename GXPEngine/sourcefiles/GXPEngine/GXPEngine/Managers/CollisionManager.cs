using System;
using System.Collections.Generic;
using System.Reflection;

namespace GXPEngine
{
    //------------------------------------------------------------------------------------------------------------------------
    //														CollisionManager
    //------------------------------------------------------------------------------------------------------------------------
    public class CollisionManager
    {
        private readonly Dictionary<GameObject, ColliderInfo> _collisionReferences =
            new Dictionary<GameObject, ColliderInfo>();

        private readonly List<ColliderInfo> activeColliderList = new List<ColliderInfo>();
        private readonly List<GameObject> colliderList = new List<GameObject>();

        //------------------------------------------------------------------------------------------------------------------------
        //														CollisionManager()
        //------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------------------
        //														Step()
        //------------------------------------------------------------------------------------------------------------------------
        public void Step()
        {
            for (int i = activeColliderList.Count - 1; i >= 0; i--)
            {
                ColliderInfo info = activeColliderList[i];
                for (int j = colliderList.Count - 1; j >= 0; j--)
                {
                    if (j >= colliderList.Count) continue; //fix for removal in loop
                    GameObject other = colliderList[j];
                    if (info.gameObject != other)
                    {
                        if (info.gameObject.HitTest(other))
                        {
                            if (info.onCollision != null)
                            {
                                info.onCollision(other);
                            }
                        }
                    }
                }
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Add()
        //------------------------------------------------------------------------------------------------------------------------
        public void Add(GameObject gameObject)
        {
            if (gameObject.collider != null)
            {
                colliderList.Add(gameObject);
            }
            MethodInfo info = gameObject.GetType()
                .GetMethod("OnCollision", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (info != null)
            {
                var onCollision =
                    (CollisionDelegate) Delegate.CreateDelegate(typeof (CollisionDelegate), gameObject, info, false);
                if (onCollision != null)
                {
                    var colliderInfo = new ColliderInfo(gameObject, onCollision);
                    _collisionReferences[gameObject] = colliderInfo;
                    activeColliderList.Add(colliderInfo);
                }
            }
            else
            {
                validateCase(gameObject);
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														validateCase()
        //------------------------------------------------------------------------------------------------------------------------
        private void validateCase(GameObject gameObject)
        {
            MethodInfo info = gameObject.GetType()
                .GetMethod("OnCollision",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (info != null)
            {
                throw new Exception("'OnCollision' function was not binded. Please check it's correct case (capital O?)");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Remove()
        //------------------------------------------------------------------------------------------------------------------------
        public void Remove(GameObject gameObject)
        {
            colliderList.Remove(gameObject);
            if (_collisionReferences.ContainsKey(gameObject))
            {
                ColliderInfo colliderInfo = _collisionReferences[gameObject];
                activeColliderList.Remove(colliderInfo);
                _collisionReferences.Remove(gameObject);
            }
        }

        private struct ColliderInfo
        {
            public readonly GameObject gameObject;
            public readonly CollisionDelegate onCollision;

            //------------------------------------------------------------------------------------------------------------------------
            //														ColliderInfo()
            //------------------------------------------------------------------------------------------------------------------------
            public ColliderInfo(GameObject gameObject, CollisionDelegate onCollision)
            {
                this.gameObject = gameObject;
                this.onCollision = onCollision;
            }
        }

        private delegate void CollisionDelegate(GameObject gameObject);
    }
}