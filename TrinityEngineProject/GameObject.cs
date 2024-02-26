
using OpenTK.Mathematics;

namespace TrinityEngineProject
{
    /*
     * Access for:
     *  Component, transform - has variable of type
     *  User - create, assign/get transfrom and components
     */

    internal class GameObject
    {
        Transform _transform;
        public Transform transform
        {
            get
            {
                return _transform;
            }
            set
            {
                if (value == null) return;
                transform.CopyValues(value);
            }
        }

        #region component
        public Component[] Components => _components.ToArray();
        readonly List<Component> _components = new List<Component>();
        public void AddComponent(Component component)
        {
            if (component.gameObject != null) component.gameObject.RemoveComponent(component);
            component.gameObject = this;
            _components.Add(component);
            if (loaded && !component.loaded) component.OnLoad();
        }
        public void RemoveComponent(Component component)
        {
            if(component.loaded)component.OnUnload();
            _components.Remove(component);
        }
        public T? GetComponent<T>() where T : Component
        {
            T? result = _components.OfType<T>().FirstOrDefault();
            if (result == null) TgMessage.ThrowWarning($"GameObject does not contain a Component of Type '{typeof(T)}' )0o0(");
            return result;
        }
        public T[] GetComponents<T>() where T : Component
        {
            T[] result = _components.OfType<T>().ToArray();
            if (result.Length == 0) TgMessage.ThrowWarning($"GameObject does not contain Components of Type '{typeof(T)}' )0o0(");
            return result;
        }
        #endregion

        public GameObject Instantiate(Transform? parent = null)
        {
            Component[] copiedComponents = new Component[_components.Count];
            for (int i = 0; i < copiedComponents.Length; i++)
            {
                copiedComponents[i] = _components[i].ShallowCopy();
            }
            GameObject gameObject = new GameObject(copiedComponents);
            gameObject.transform.parent = parent;
            gameObject.active = active;
            if (transform.HasChildren)
            {
                foreach (var child in transform.Children.ToArray())
                    child.gameObject.Instantiate(gameObject.transform);
            }
            if (parent == null) gameObject.Load();
            return gameObject;
        }
        public static GameObject Instantiate(params Component[] components)
        {
            GameObject gameObject = new GameObject(components);
            gameObject.Load();
            return gameObject;
        }
        public static GameObject Instantiate(Vector3? position = null, Vector3? scale = null, Quaternion? rotation = null, params Component[] components)
        {
            GameObject gameObject = new GameObject(components);
            if (position.HasValue) gameObject.transform.position = position.Value;
            if (scale.HasValue) gameObject.transform.scale = scale.Value;
            if (rotation.HasValue) gameObject.transform.rotation = rotation.Value;
            gameObject.Load();
            return gameObject;
        }
        public GameObject(params Component[] components)
        {
            _transform = new Transform();
            _transform.gameObject = this;
            foreach (var component in components)
                AddComponent(component);
        }

        static List<GameObject> sceneGameObjects = new List<GameObject>();
        public static void ClearSceneGameObjects()
        {
            foreach (var gameObject in sceneGameObjects)
            {
                gameObject.Destroy();
            }
            sceneGameObjects.Clear();
        }
        public bool isPartOfScene { get; private set; }

        public bool active { get; private set; } = true;
        public void setAcive(bool value)
        {
            if (value == active) return;
            active = value;

            if (value)
            {
                if (isPartOfScene) Load();
            }
            else
            {
                Unload();
            }
        }

        public bool loaded { get; private set; }
        public void Load()
        {
            if (loaded || !active) return;
            loaded = true;
            if (!isPartOfScene)
            {
                sceneGameObjects.Add(this);
                isPartOfScene = true;
            }
            foreach (var component in _components)
            {
                component.OnLoad();
            }
            foreach (var child in transform.Children)
            {
                child.gameObject.Load();
            }
        }
        void Unload()
        {
            if (!loaded) return;
            loaded = false;
            foreach (var component in _components)
            {
                component.OnUnload();
            }
            foreach (var child in transform.Children)
            {
                child.gameObject.Unload();
            }
        }
        public void Destroy()
        {
            RemoveFromScene();
            Unload();
        }
        void RemoveFromScene()
        {
            isPartOfScene = false;
            sceneGameObjects.Remove(this);
            foreach (var child in transform.Children)
            {
                child.gameObject.RemoveFromScene();
            }
        }


        
    }
}
