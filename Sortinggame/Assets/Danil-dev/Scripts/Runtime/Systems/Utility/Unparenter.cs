using UnityEngine;

namespace D_Dev.Utility
{
    public class Unparenter : MonoBehaviour
    {
        #region Fields

        [SerializeField] private bool _onAwake;
        [SerializeField] private bool _onStart;
        [Space]
        [SerializeField] private bool _destroyComponentOnUnparented = true;

        #endregion

        #region Properties

        public bool OnAwake
        {
            get => _onAwake;
            set => _onAwake = value;
        }

        public bool OnStart
        {
            get => _onStart;
            set => _onStart = value;
        }

        public bool DestroyComponentOnUnparented
        {
            get => _destroyComponentOnUnparented;
            set => _destroyComponentOnUnparented = value;
        }

        #endregion

        #region Monobehaviour

        private void Awake()
        {
            if(_onAwake)
                Unparent();
        }

        private void Start()
        {
            if(_onStart)
                Unparent();
        }

        #endregion
        
        #region Public

        public void Unparent()
        {
            transform.SetParent(null);
            if(_destroyComponentOnUnparented)
                Destroy(this);
        }

        #endregion
    }
}
