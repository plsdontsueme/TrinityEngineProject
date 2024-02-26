
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

                if (loaded) _transform.OnUnload();
                _transform.gameObject = null;
                if (value.gameObject == null)
                {
                    value.gameObject = this;
                    _transform = value;
                }
                else
                {
                    _transform = value.Copy();
                    _transform.gameObject = this;
                }
                if (loaded) _transform.OnLoad();
            }
        }

        #region component
        public Component[] Components => components.ToArray();
        List<Component> components;
        public void AddComponent(Component component)
        {
            if (component.gameObject != null) component.gameObject.RemoveComponent(component);
            component.gameObject = this;
            components.Add(component);
            if (loaded) component.OnLoad();
        }
        public void RemoveComponent(Component component)
        {
            if (loaded) component.OnUnload();
            components.Remove(component);
        }
        public T? GetComponent<T>() where T : Component
        {
            T? result = components.OfType<T>().FirstOrDefault();
            if (result == null) TgMessage.ThrowWarning($"GameObject does not contain a Component of Type '{typeof(T)}' )0o0(");
            return result;
        }
        public T[] GetComponents<T>() where T : Component
        {
            T[] result = components.OfType<T>().ToArray();
            if (result.Length == 0) TgMessage.ThrowWarning($"GameObject does not contain Components of Type '{typeof(T)}' )0o0(");
            return result;
        }
        #endregion

        public static GameObject Instantiate(GameObject blueprint)
        {
            Component[] blueprintComponents = blueprint.Components;
            Component[] copiedComponents = new Component[blueprintComponents.Length];
            for (int i = 0; i < blueprintComponents.Length; i++)
            {
                copiedComponents[i] = blueprintComponents[i].ShallowCopy();
            }
            GameObject gameObject = new GameObject(blueprint.transform.Copy(), copiedComponents);
            gameObject.active = blueprint.active;
            gameObject.Load();
            return gameObject;
        }
        public static GameObject Instantiate(params Component[] components)
        {
            GameObject gameObject = new GameObject(components);
            gameObject.Load();
            return gameObject;
        }
        public static GameObject Instantiate(Transform transform, params Component[] components)
        {
            GameObject gameObject = new GameObject(transform, components);
            gameObject.Load();
            return gameObject;
        }
        public GameObject(params Component[] components) : this(new Transform(), components) { }
        public GameObject(Transform transform, params Component[] components)
        {
            _transform = transform;
            this.components = new List<Component>();
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
        bool isPartOfScene;

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
            sceneGameObjects.Add(this);
            isPartOfScene = true;
            transform.OnLoad();
            foreach (var component in components)
            {
                component.OnLoad();
            }
        }
        void Unload()
        {
            if (!loaded) return;
            loaded = false;
            transform.OnUnload();
            foreach (var component in components)
            {
                component.OnUnload();
            }
        }
        public void Destroy()
        {
            isPartOfScene = false;
            sceneGameObjects.Remove(this);
            Unload();
        }

    }
}
