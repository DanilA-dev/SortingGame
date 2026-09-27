using UnityEngine;

namespace D_Dev.Base
{
    public interface IInteractable
    {
        public GameObject GameObject { get; }
        public bool CanBeStopped { get; }
        public bool IsDistanceBased { get; }
        public void StartInteract(GameObject interactor);
        public void StopInteract(GameObject interactor);
        public bool CanInteract(GameObject interactor);
        public void Focus(GameObject interactor);
        public void Unfocus(GameObject interactor);
    }
}
