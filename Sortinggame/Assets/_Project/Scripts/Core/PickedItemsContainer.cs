using System.Collections.Generic;
using System.Linq;
using D_Dev.Base;
using D_Dev.CustomEventManager;
using D_Dev.PolymorphicValueSystem;
using D_Dev.ScriptableVariables;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace _Project.Scripts
{
    public class PickedItemsContainer : MonoBehaviour
    {
        #region Fields

        [Title("Base Settings")]
        [SerializeField] private Transform _rootPoint;
        [SerializeReference] private Vector3 _itemStep;
        [SerializeReference] private PolymorphicValue<int> _maxCapacity = new IntConstantValue();
        [SerializeReference] private PolymorphicValue<int> _currentCapacity = new IntConstantValue();
        [Title("Events Variables")] 
        [SerializeField] private StringScriptableVariable _onItemInteractStartEventName;

        [FoldoutGroup("Item Animation Tween")] 
        [SerializeReference] private PolymorphicValue<float> _duration = new FloatConstantValue();
        [FoldoutGroup("Item Animation Tween")] 
        [SerializeReference] private PolymorphicValue<float> _jumpPower = new FloatConstantValue();
        [FoldoutGroup("Item Animation Tween")] 
        [SerializeReference] private PolymorphicValue<int> _jumpsNum = new IntConstantValue();
        [FoldoutGroup("Item Animation Tween")] 
        [SerializeField] private Ease _jumpEase;
        [FoldoutGroup("Events")]
        public UnityEvent OnFullCapacity;
        [FoldoutGroup("Events")]
        public UnityEvent OnItemPicked;
        [FoldoutGroup("Events")]
        public UnityEvent OnItemDropped;

        private List<IInteractable> _currentItems = new();
        private List<Transform> _createdPoints = new();
        private Vector3 _lastStep;
        private int _posIndex;

        private Sequence _seq;
        
        #endregion

        #region Properties

        public List<IInteractable> CurrentItems => _currentItems;

        #endregion

        #region Monobehaviour

        private void OnEnable()
        {
            EventManager.AddListener<IInteractable>(_onItemInteractStartEventName.ToString(), OnItemStartInteraction);
            InitPositions();
        }

        private void OnDisable()
        {
            EventManager.RemoveListener<IInteractable>(_onItemInteractStartEventName.ToString(), OnItemStartInteraction);
        }

        #endregion

        #region Public

        public void TryRemoveLastItem()
        {
            var last = GetLastItem();
            if(last == null)
                return;
            
            _currentItems.Remove(last);
            if(_posIndex > 0)
                _posIndex--;
            
            last.StopInteract(gameObject);
            OnItemDropped?.Invoke();
        }

        public IInteractable GetLastItem()
        {
            if (_currentItems.Count <= 0)
                return null;

            return _currentItems.Last();
        }
        
        #endregion
        
        #region Listeners

        private void OnItemStartInteraction(IInteractable interactable)
        {
            TryPickItem(interactable);
        }

        #endregion

        #region Private
        private void InitPositions()
        {
            _lastStep = _rootPoint.position;
            for (int i = 0; i < _maxCapacity.Value; i++)
            {
                GameObject newItemPoint = new GameObject("ItemStep");
                newItemPoint.transform.SetParent(transform);
                newItemPoint.transform.localPosition = i == 0? Vector3.zero : _lastStep;
                _lastStep = new Vector3(0, _itemStep.y + newItemPoint.transform.localPosition.y, 0);
                _createdPoints.Add(newItemPoint.transform);
            }
        }
        
        private void TryPickItem(IInteractable interactable)
        {
            if (_currentItems.Count >= _currentCapacity.Value)
            {
                OnFullCapacity?.Invoke();
                return;
            }
            
            if(!_currentItems.Contains(interactable))
                _currentItems.Add(interactable);
            
            AnimatePick(interactable);
            OnItemPicked?.Invoke();
        }

        private void AnimatePick(IInteractable interactable)
        {
            var pos = _createdPoints[_posIndex];
            interactable.GameObject.transform.SetParent(pos);
            interactable.CanBeStopped = false;
            _seq = DOTween.Sequence();
            _seq.Kill();
            _seq.Append(interactable.GameObject.transform.DOLocalJump(Vector3.zero, _jumpPower.Value, _jumpsNum.Value, _duration.Value)
                    .SetEase(_jumpEase))
                .Join(interactable.GameObject.transform.DOLocalRotate(Vector3.zero, _duration.Value))
                .OnComplete(() =>
                {
                    if(_posIndex < _createdPoints.Count)
                        _posIndex++;
                    
                    interactable.CanBeStopped = true;
                })
                .SetAutoKill(true);
        }

        #endregion
    }
}