using UnityEngine;

namespace D_Dev.Base
{
    public interface IInteractable
    {
        public bool CanBeStopped { get; }
        public bool IsDistanceBased { get; }
        public void StartInteract(GameObject interactor);
        public void StopInteract(GameObject interactor);
        public bool CanInteract(GameObject interactor);
    }
}
