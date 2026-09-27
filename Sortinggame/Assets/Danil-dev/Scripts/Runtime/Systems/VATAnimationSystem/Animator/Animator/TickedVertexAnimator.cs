using D_Dev.UpdateManagerSystem;

namespace D_Dev.VATAnimationSystem
{
    public class TickedVertexAnimator : BaseVertexAnimator, ITickable
    {
        #region Monobehaviour

        private void OnEnable()
        {
            UpdateManager.Add(this);
        }

        private void OnDisable()
        {
            UpdateManager.Remove(this);
        }

        #endregion

        #region ITickable

        public void Tick() => UpdateAnimation();

        #endregion
    }
}