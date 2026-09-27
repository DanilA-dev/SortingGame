using D_Dev.EntityVariable.Types;
using D_Dev.MenuHandler;
using D_Dev.PolymorphicValueSystem;
using D_Dev.RuntimeEntityVariables;
using D_Dev.ScriptableVariables;
using D_Dev.TagSystem;
using D_Dev.TagSystem.Extensions;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.UI
{
    public class ItemInteractableMenu : BaseMenu
    {
        #region Fields

        [Title("Components")]
        [SerializeReference] private PolymorphicValue<GameObject> _lastInteractableItem = new GameObjectConstantValue();
        [SerializeField] private Tag[] _itemTags;

        [Title("Variable Id's")] 
        [SerializeField] private StringScriptableVariable _itemNameVariableId;
        [SerializeField] private StringScriptableVariable _itemDescVariableId;

        [Title("UI")] 
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _descText;
        
        private RuntimeEntityVariablesContainer _itemVariablesContainer;

        #endregion

        #region Monobehaviour

                
        private void OnEnable()
        {
            if(_lastInteractableItem == null)
                return;

            _lastInteractableItem.OnValueChanged += OnLastInteractableChanged;
            UpdateItemInfo(_lastInteractableItem.Value);
        }

        private void OnDisable()
        {
            _lastInteractableItem.OnValueChanged -= OnLastInteractableChanged;
            _itemVariablesContainer = null;
        }

        #endregion

        #region Listeners

        private void OnLastInteractableChanged(GameObject interactable)
        {
            if(interactable == null) 
                Close();
            else
                UpdateItemInfo(interactable);
        }

        #endregion
        
        #region Private

        private void UpdateItemInfo(GameObject interactable)
        {
            if(interactable == null)
                return;
            
            if(!interactable.HasTags(_itemTags))
                return;

            if (!interactable.TryGetComponent(out _itemVariablesContainer))
                return;

            if (_itemVariablesContainer.TryGetVariable<StringEntityVariable>(_itemNameVariableId, out var itemName))
                _nameText?.SetText(itemName.Value.Value);
           
            if (_itemVariablesContainer.TryGetVariable<StringEntityVariable>(_itemDescVariableId, out var itemDesc))
                _descText?.SetText(itemDesc.Value.Value);
        }

        #endregion
    }
}