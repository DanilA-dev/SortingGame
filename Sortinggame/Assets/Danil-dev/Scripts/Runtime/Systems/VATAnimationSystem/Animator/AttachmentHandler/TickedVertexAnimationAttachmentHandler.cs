using D_Dev.UpdateManagerSystem;

namespace D_Dev.VATAnimationSystem
{
    public class TickedVertexAnimationAttachmentHandler : BaseVertexAnimationAttachmentHandler, ILateTickable
    {
        #region Monobehaviour

        private void OnEnable()
        {
            LateUpdateManager.Add(this);
        }

        private void OnDisable()
        {
            LateUpdateManager.Remove(this);
        }

        #endregion

        #region ITickable

        public void LateTick()
        {
            UpdateAttachments();
        }

        #endregion
    }
}