using OpenTK.Mathematics;

namespace TrinityEngineProject
{
    /*
        * Access for:
        *  GameObject - set gameObject, load, unload
        *  Renderer - GetMatrix for model
        *  User - create and assign to GameObject, read gameObject
        */

    internal class Transform
    {
        public GameObject gameObject { get; internal set; }

        Vector3 _position;
        public Vector3 position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                UpdateGlobalPosition(true);
            }
        }
        /*
        public float position_X
        {
            get { return _position.X; }
            set
            {
                _position.X = value;
                UpdateGlobalPosition();
            }
        }
        public float position_Y
        {
            get { return _position.Y; }
            set
            {
                _position.X = value;
                UpdateGlobalPosition();
            }
        }
        public float position_Z
        {
            get { return _position.Z; }
            set
            {
                _position.X = value;
                UpdateGlobalPosition();
            }
        }
        */

        Vector3 _scale;
        public Vector3 scale
        {
            get => _scale;
            set
            {
                _scale = value;
                UpdateGlobalScale();
            }
        }

        Quaternion _rotation;
        public Quaternion rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                UpdateGlobalRotation();
            }
        }

        public Quaternion globalRotation { get; private set; }
        public Vector3 globalScale { get; private set; }
        public Vector3 globalPosition { get; private set; }

        void UpdateGlobalPosition(bool chain)
        {
            if (_parent == null)
            {
                globalPosition = _position;
            }
            else
            {
                globalPosition = _parent.globalPosition + _parent.globalRotation * _parent.globalScale * _position;
            }
            if (chain && _children.Count > 0)
            {
                foreach (Transform child in _children)
                {
                    child.UpdateGlobalPosition(true);
                }
            }
        }
        void UpdateGlobalScale()
        {
            if (_parent == null)
            {
                globalScale = _scale;
            }
            else
            {
                globalScale = _parent.globalScale * _scale;
                //globalScale = Vector3.TransformNormal(_scale, Matrix4.CreateFromQuaternion(_parent.globalRotation));
                //globalScale *= _parent.globalScale;
            }
            if (_children.Count > 0)
            {
                foreach (Transform child in _children)
                {
                    child.UpdateGlobalPosition(false);
                    child.UpdateGlobalScale();
                }
            }
        }
        void UpdateGlobalRotation()
        {
            if (_parent == null)
            {
                globalRotation = _rotation;
            }
            else
            {
                globalRotation = _parent.globalRotation * _rotation;
            }
            if (_children.Count > 0)
            {
                foreach (Transform child in _children)
                {
                    child.UpdateGlobalPosition(false);
                    child.UpdateGlobalRotation();
                }
            }
        }
        void UpdateAllGlobal()
        {
            if (_parent == null)
            {
                globalPosition = _position;
                globalScale = _scale;
                globalRotation = _rotation;
            }
            else
            {
                globalPosition = _parent.globalPosition + _parent.globalRotation * _parent.globalScale * _position;
                globalScale = _parent.globalScale * _scale;
                globalRotation = _parent.globalRotation * _rotation;
            }
            if (_children.Count > 0)
            {
                foreach (Transform child in _children)
                {
                    child.UpdateAllGlobal();
                }
            }
        }

        public Transform? parent
        {
            get
            {
                return _parent;
            }
            set
            {
                if (value == null)
                {
                    _parent = value;
                }
                else
                {
                    if (value == this || value.IsParent(this)) return;
                    _parent?._children.Remove(this);
                    _parent = value;
                    _parent._children.Add(this);
                    if (gameObject.loaded)
                    {
                        _parent.gameObject.Load(); // returns if already loaded
                    }
                }

                UpdateAllGlobal();
            }
        }
        Transform? _parent;
        bool IsParent(Transform? transform)
        {
            if (_parent == null) return false;
            if (_parent == transform) return true;

            return IsParent(_parent);
        }
        internal readonly List<Transform> _children = new List<Transform>();
        public bool HasChildren => _children.Count > 0;
        public IReadOnlyList<Transform> Children => _children.AsReadOnly();


        public void CopyValues(Transform reference)
        {
            _position = reference.position;
            _scale = reference.scale;
            _rotation = reference.rotation;
            UpdateAllGlobal();
        }
        public Transform(Transform reference)
        {
            _position = reference.position;
            _scale = reference.scale;
            _rotation = reference.rotation;
            UpdateAllGlobal();
        }
        public Transform(Vector3? position = null, Vector3? scale = null, Quaternion? rotation = null)
        {
            _position = position ?? Vector3.Zero;
            _scale = scale ?? Vector3.One;
            _rotation = rotation ?? Quaternion.Identity;
            UpdateAllGlobal();
        }

        public Matrix4 GetMatrix()
        {
            return Matrix4.CreateScale(globalScale) * Matrix4.CreateFromQuaternion(globalRotation) * Matrix4.CreateTranslation(globalPosition);
        }
    }
}
