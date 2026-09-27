using D_Dev.UpdateManagerSystem;

namespace D_Dev.TransformFollower.Extensions
{
    public class TickedTransformFollower : BaseTransformFollower, ITickable
    {
        #region Monobehaviour

        private void OnEnable() => UpdateManager.Add(this);
        private void OnDisable() => UpdateManager.Remove(this);

        #endregion

        #region ITickable

        public void Tick()
        {
            OnUpdate();
        }

        #endregion
    }
}
