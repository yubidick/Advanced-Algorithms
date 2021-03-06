﻿using System;

namespace Advanced.Algorithms.DataStructures
{
    internal class AVLTreeNode<T> : IBSTNode<T> where T : IComparable
    {
        internal T Value { get; set; }

        internal AVLTreeNode<T> Parent { get; set; }

        internal AVLTreeNode<T> Left { get; set; }
        internal AVLTreeNode<T> Right { get; set; }

        internal bool IsLeaf => Left == null && Right == null;

        internal AVLTreeNode(AVLTreeNode<T> parent, T value)
        {
            Parent = parent;
            Value = value;
            Height = 0;
        }

        internal int Height { get; set; }

        //exposed to do common tests for Binary Trees
        IBSTNode<T> IBSTNode<T>.Left => Left;
        IBSTNode<T> IBSTNode<T>.Right => Right;
        T IBSTNode<T>.Value => Value;
    }

    //TODO support bulk initial loading
    //TODO implement IEnumerable & make sure duplicates are handled correctly if its not already
    public class AVLTree<T> where T : IComparable
    {
        internal AVLTreeNode<T> Root { get; private set; }
        public int Count { get; private set; }

        //O(log(n)) always
        public bool HasItem(T value)
        {
            if (Root == null)
            {
                return false;
            }

            return find(Root, value) != null;
        }

        //O(1)
        public int GetHeight()
        {
            if (Root == null)
                return -1;

            return Root.Height;
        }

        //O(log(n)) always
        public void Insert(T value)
        {
            if (Root == null)
            {
                Root = new AVLTreeNode<T>(null, value);
                Count++;
                return;
            }

            insert(Root, value);
            Count++;
        }

        //O(log(n)) always
        private void insert(AVLTreeNode<T> node, T value)
        {

            var compareResult = node.Value.CompareTo(value);

            //node is less than the value so move right for insertion
            if (compareResult < 0)
            {
                if (node.Right == null)
                {
                    node.Right = new AVLTreeNode<T>(node, value);
                }
                else
                {
                    insert(node.Right, value);
                }
            }
            //node is greater than the value so move left for insertion
            else if (compareResult > 0)
            {
                if (node.Left == null)
                {
                    node.Left = new AVLTreeNode<T>(node, value);
                }
                else
                {
                    insert(node.Left, value);
                }

            }
            else
            {
                throw new Exception("Item exists");
            }

            UpdateHeight(node);
            balance(node);

        }


        //remove the node with the given identifier from the descendants 
        //O(log(n)) always
        public void Delete(T value)
        {
            if (Root == null)
            {
                throw new Exception("Empty AVLTree");
            }

            delete(Root, value);
            Count--;
        }

        //O(log(n)) always
        private void delete(AVLTreeNode<T> node, T value)
        {
            var baseCase = false;

            var compareResult = node.Value.CompareTo(value);

            //node is less than the search value so move right to find the deletion node
            if (compareResult < 0)
            {
                if (node.Right == null)
                {
                    throw new Exception("Item do not exist");
                }
                delete(node.Right, value);
            }
            //node is less than the search value so move left to find the deletion node
            else if (compareResult > 0)
            {
                if (node.Left == null)
                {
                    throw new Exception("Item do not exist");
                }

                delete(node.Left, value);
            }
            else
            {
                //node is a leaf node
                if (node.IsLeaf)
                {
                    //if node is root
                    if (node.Parent == null)
                    {
                        Root = null;
                    }
                    //assign nodes parent.left/right to null
                    else if (node.Parent.Left == node)
                    {
                        node.Parent.Left = null;
                    }
                    else
                    {
                        node.Parent.Right = null;
                    }
                    baseCase = true;

                }
                else
                {
                    //case one - right tree is null (move sub tree up)
                    if (node.Left != null && node.Right == null)
                    {
                        //root
                        if (node.Parent == null)
                        {
                            Root.Left.Parent = null;
                            Root = Root.Left;
                        }
                        else
                        {
                            //node is left child of parent
                            if (node.Parent.Left == node)
                            {
                                node.Parent.Left = node.Left;
                            }
                            //node is right child of parent
                            else
                            {
                                node.Parent.Right = node.Left;
                            }

                            node.Left.Parent = node.Parent;
                        }
                        baseCase = true;

                    }
                    //case two - left tree is null  (move sub tree up)
                    else if (node.Right != null && node.Left == null)
                    {
                        //root
                        if (node.Parent == null)
                        {
                            Root.Right.Parent = null;
                            Root = Root.Right;
                        }
                        else
                        {
                            //node is left child of parent
                            if (node.Parent.Left == node)
                            {
                                node.Parent.Left = node.Right;
                            }
                            //node is right child of parent
                            else
                            {
                                node.Parent.Right = node.Right;
                            }
                            node.Right.Parent = node.Parent;

                        }
                        baseCase = true;

                    }
                    //case three - two child trees 
                    //replace the node value with maximum element of left subtree (left max node)
                    //and then delete the left max node
                    else
                    {
                        var maxLeftNode = findMax(node.Left);

                        node.Value = maxLeftNode.Value;

                        //delete left max node
                        delete(node.Left, maxLeftNode.Value);
                    }
                }
            }

            if (baseCase)
            {
                UpdateHeight(node.Parent);
                balance(node.Parent);
            }
            else
            {
                UpdateHeight(node);
                balance(node);
            }


        }

        public T FindMax()
        {
            return findMax(Root).Value;
        }


        private AVLTreeNode<T> findMax(AVLTreeNode<T> node)
        {
            while (true)
            {
                if (node.Right == null)
                {
                    return node;
                }

                node = node.Right;
            }
        }

        public T FindMin()
        {
            return findMin(Root).Value;
        }

        private AVLTreeNode<T> findMin(AVLTreeNode<T> node)
        {
            while (true)
            {
                if (node.Left == null)
                {
                    return node;
                }

                node = node.Left;
            }
        }

        //O(log(n)) worst O(n) for unbalanced tree
        public bool Contains(T value)
        {
            if (Root == null)
            {
                return false;
            }

            return find(Root, value)!=null;
        }


        //find the node with the given identifier among descendants of parent and parent
        //uses pre-order traversal
        //O(log(n)) worst O(n) for unbalanced tree
        private AVLTreeNode<T> find(AVLTreeNode<T> parent, T value)
        {
            if (parent == null)
            {
                return null;
            }

            if (parent.Value.CompareTo(value) == 0)
            {
                return parent;
            }

            var left = find(parent.Left, value);

            if (left != null)
            {
                return left;
            }

            var right = find(parent.Right, value);

            return right;
        }

        private void balance(AVLTreeNode<T> node)
        {
            if (node == null)
                return;

            if (node.Left == null && node.Right == null)
                return;

            var leftHeight = node.Left?.Height + 1 ?? 0;
            var rightHeight = node.Right?.Height + 1 ?? 0;

            var balanceFactor = leftHeight - rightHeight;
            //tree is left heavy
            //differance >=2 then do rotations
            if (balanceFactor >= 2)
            {
                leftHeight = node.Left?.Left?.Height + 1 ?? 0;
                rightHeight = node.Left?.Right?.Height + 1 ?? 0;

                //left child is left heavy
                if (leftHeight > rightHeight)
                {
                    rightRotate(node);
                }
                //left child is right heavy
                else
                {
                    leftRotate(node.Left);
                    rightRotate(node);
                }
            }
            //tree is right heavy
            //differance <=-2 then do rotations
            else if (balanceFactor <= -2)
            {
                leftHeight = node.Right?.Left?.Height + 1 ?? 0;
                rightHeight = node.Right?.Right?.Height + 1 ?? 0;

                //right child is right heavy
                if (rightHeight > leftHeight)
                {
                    leftRotate(node);
                }
                //right child is left heavy
                else
                {
                    rightRotate(node.Right);
                    leftRotate(node);
                }

            }
        }

        private void rightRotate(AVLTreeNode<T> node)
        {
            var prevRoot = node;
            var leftRightChild = prevRoot.Left.Right;

            var newRoot = node.Left;

            //make left child as root
            prevRoot.Left.Parent = prevRoot.Parent;

            if (prevRoot.Parent != null)
            {
                if (prevRoot.Parent.Left == prevRoot)
                {
                    prevRoot.Parent.Left = prevRoot.Left;
                }
                else
                {
                    prevRoot.Parent.Right = prevRoot.Left;
                }
            }

            //move prev root as right child of current root
            newRoot.Right = prevRoot;
            prevRoot.Parent = newRoot;

            //move right child of left child of prev root to left child of right child of new root
            newRoot.Right.Left = leftRightChild;
            if (newRoot.Right.Left != null)
            {
                newRoot.Right.Left.Parent = newRoot.Right;
            }

            UpdateHeight(newRoot);

            if (prevRoot == Root)
            {
                Root = newRoot;
            }

        }

        private void leftRotate(AVLTreeNode<T> node)
        {
            var prevRoot = node;
            var rightLeftChild = prevRoot.Right.Left;

            var newRoot = node.Right;

            //make right child as root
            prevRoot.Right.Parent = prevRoot.Parent;

            if (prevRoot.Parent != null)
            {
                if (prevRoot.Parent.Left == prevRoot)
                {
                    prevRoot.Parent.Left = prevRoot.Right;
                }
                else
                {
                    prevRoot.Parent.Right = prevRoot.Right;
                }
            }


            //move prev root as left child of current root
            newRoot.Left = prevRoot;
            prevRoot.Parent = newRoot;

            //move left child of right child of prev root to right child of left child of new root
            newRoot.Left.Right = rightLeftChild;
            if (newRoot.Left.Right != null)
            {
                newRoot.Left.Right.Parent = newRoot.Left;
            }

            UpdateHeight(newRoot);

            if (prevRoot == Root)
            {
                Root = newRoot;
            }
        }

        private void UpdateHeight(AVLTreeNode<T> node)
        {
            if (node == null)
            {
                return;
            }

            if (node.Left != null)
            {
                node.Left.Height = Math.Max(node.Left.Left?.Height + 1 ?? 0,
                                                 node.Left.Right?.Height + 1 ?? 0);
            }

            if (node.Right != null)
            {
                node.Right.Height = Math.Max(node.Right.Left?.Height + 1 ?? 0,
                                  node.Right.Right?.Height + 1 ?? 0);
            }

            node.Height = Math.Max(node.Left?.Height + 1 ?? 0,
                                      node.Right?.Height + 1 ?? 0);
        }
    }
}
