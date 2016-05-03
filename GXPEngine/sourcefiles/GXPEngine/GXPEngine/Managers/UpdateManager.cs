using System;
using System.Collections.Generic;
using System.Reflection;

namespace GXPEngine.Managers
{
    public class UpdateManager
    {
        private readonly Dictionary<GameObject, UpdateDelegate> _updateReferences =
            new Dictionary<GameObject, UpdateDelegate>();

        private UpdateDelegate _updateDelegates;

        //------------------------------------------------------------------------------------------------------------------------
        //														UpdateManager()
        //------------------------------------------------------------------------------------------------------------------------

        //------------------------------------------------------------------------------------------------------------------------
        //														Step()
        //------------------------------------------------------------------------------------------------------------------------
        public void Step()
        {
            if (_updateDelegates != null)
                _updateDelegates();
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Add()
        //------------------------------------------------------------------------------------------------------------------------
        public void Add(GameObject gameObject)
        {
            MethodInfo info = gameObject.GetType()
                .GetMethod("Update", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (info != null)
            {
                var onUpdate =
                    (UpdateDelegate) Delegate.CreateDelegate(typeof (UpdateDelegate), gameObject, info, false);
                if (onUpdate != null)
                {
                    _updateReferences[gameObject] = onUpdate;
                    _updateDelegates += onUpdate;
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
                .GetMethod("Update",
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (info != null)
            {
                throw new Exception("'Update' function was not binded for '" + gameObject +
                                    "'. Please check it's case. (capital U?)");
            }
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														Remove()
        //------------------------------------------------------------------------------------------------------------------------
        public void Remove(GameObject gameObject)
        {
            if (_updateReferences.ContainsKey(gameObject))
            {
                UpdateDelegate onUpdate = _updateReferences[gameObject];
                if (onUpdate != null) _updateDelegates -= onUpdate;
                _updateReferences.Remove(gameObject);
            }
        }

        private delegate void UpdateDelegate();
    }
}