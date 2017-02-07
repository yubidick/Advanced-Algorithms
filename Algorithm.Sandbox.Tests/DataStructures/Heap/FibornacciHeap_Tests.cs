﻿using Algorithm.Sandbox.DataStructures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm.Sandbox.Tests.DataStructures.Heap
{
    [TestClass]
    public class FibornacciMinHeap_Tests
    {
        /// <summary>
        /// A tree test
        /// </summary>
        [TestMethod]
        public void FibornacciMinHeap_Test()
        {
            int nodeCount = 1000 * 10;
            //insert test
            var tree = new AsFibornacciMinHeap<int>();

            var nodePointers = new AsArrayList<AsFibornacciTreeNode<int>>();

            for (int i = 0; i <= nodeCount; i++)
            {
                var node = tree.Insert(i);
                nodePointers.AddItem(node);
            }

            Assert.IsTrue(tree.PeekMin() == 0);


            for (int i = 0; i <= nodeCount; i++)
            {
                nodePointers[i].Value--;
                tree.DecrementKey(nodePointers[i]);
                var min = tree.ExtractMin();
                Assert.AreEqual(min, i - 1);
            }

            nodePointers.Clear();

            var rnd = new Random();
            var testSeries = Enumerable.Range(1, nodeCount).OrderBy(x => rnd.Next()).ToList();

            foreach (var item in testSeries)
            {
                nodePointers.AddItem(tree.Insert(item));
            }


            for (int i = 0; i < nodeCount; i++)
            {
                nodePointers[i].Value--;
                tree.DecrementKey(nodePointers[i]);
            }

            for (int i = 1; i <= nodeCount; i++)
            {
                var min = tree.ExtractMin();
                Assert.AreEqual(min, i - 1);
            }

        }
    }
}