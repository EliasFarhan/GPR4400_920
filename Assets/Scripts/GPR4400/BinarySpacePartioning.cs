using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GPR4400
{
    public class BinaryNode
    {
        private Rect aabb_;
        public Rect Aabb => aabb_;

        private BinaryNode[] children_ = new BinaryNode[2];
        public BinaryNode[] Children => children_;

        public BinaryNode(Rect aabb)
        {
            aabb_ = aabb;
        }

        public void Split(bool horizontal, float ratio)
        {
            var rect = new Rect
            {
                min = aabb_.min, max = aabb_.max - (horizontal ? Vector2.right*aabb_.size.x : Vector2.up*aabb_.size.y) * (1.0f - ratio)
            };
            children_[0] = new BinaryNode(rect);
            
            rect.min = aabb_.min + (horizontal ? Vector2.right*aabb_.size.x : Vector2.up*aabb_.size.y) * ratio;
            rect.max = aabb_.max;
            children_[1] = new BinaryNode(rect);
        }
        
    }
    [ExecuteInEditMode]
    public class BinarySpacePartioning : MonoBehaviour
    {
        private BinaryNode rootNode;
        private const float margin = 1.0f;
        [SerializeField] private float minRatio = 0.2f;
        [SerializeField] private float maxRatio = 0.8f;
        [SerializeField] private int iteration = 2;

        private void Start()
        {
            if (Application.isPlaying)
            {
                GenerateBinaryTree();
            }
        }

        void GenerateBinaryTree()
        {
            var mainCamera = Camera.main;
            var cameraSize = 2.0f * mainCamera.orthographicSize * new Vector2(mainCamera.aspect, 1.0f);
            var cameraRect = new Rect(){min=-cameraSize/2.0f, max = cameraSize/2.0f};
            rootNode = new BinaryNode(cameraRect);
            SplitBinaryNode(rootNode, iteration);
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                GenerateBinaryTree();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            DrawBinaryNode(rootNode);
        }

        void DrawBinaryNode(BinaryNode node)
        {
            if(node == null)
                return;
            if (node.Children[0] != null)
            {
                foreach (var child in node.Children)
                {
                    DrawBinaryNode(child);
                }
            }
            else
            {
                Gizmos.DrawWireCube(node.Aabb.center, node.Aabb.size);
            }
        }

        private void SplitBinaryNode(BinaryNode node, int remainingIteration)
        {
            if(remainingIteration == 0)
                return;
            remainingIteration--;
            if (node.Children[0] == null)
                node.Split(node.Aabb.size.x>node.Aabb.size.y,Random.Range(minRatio, maxRatio));
            foreach (var child in node.Children)
            {
                SplitBinaryNode(child, remainingIteration);
            }
        }
    }
}