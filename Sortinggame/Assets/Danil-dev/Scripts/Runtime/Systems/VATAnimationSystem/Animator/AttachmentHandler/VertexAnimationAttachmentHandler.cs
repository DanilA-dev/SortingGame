namespace D_Dev.VATAnimationSystem
{
    public class VertexAnimationAttachmentHandler : BaseVertexAnimationAttachmentHandler
    {
        #region Monobehaviour

        private void LateUpdate()
        {
            UpdateAttachments();
        }

        #endregion
    }
}