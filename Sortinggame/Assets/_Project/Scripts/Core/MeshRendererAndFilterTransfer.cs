using Sirenix.OdinInspector;
using UnityEngine;

namespace _Project.Scripts.Editor
{
    public class MeshRendererAndFilterTransfer : MonoBehaviour
    {
        public MeshRenderer inputRenderer;
        public MeshFilter inputMeshFilter;
        [Space]
        public MeshRenderer outputRenderer;
        public MeshFilter outputMeshFilter;

        [Button]
        public void Transfer()
        {
            outputRenderer.material = inputRenderer.sharedMaterial;
            outputMeshFilter.mesh = inputMeshFilter.sharedMesh;
        }
    }
}