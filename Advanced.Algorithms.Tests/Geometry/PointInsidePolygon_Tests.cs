﻿using Advanced.Algorithms.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Advanced.Algorithms.Tests.Geometry
{

    [TestClass]
    public class PointInsidePolygon_Tests
    {
        [TestMethod]
        public void PointInsidePolygon_Smoke_Test()
        {
            var polygon = new List<int[]>() {
                new int[] { 0, 0 },
                new int[] { 10, 10 },
                new int[] { 11, 11 },
                new int[] { 0, 10 }
            };

            var testPoint = new int[] { 20, 20 };

            Assert.IsFalse(PointInsidePolygon.IsInside(polygon, testPoint));

            testPoint = new int[] { 5, 5 };
            Assert.IsTrue(PointInsidePolygon.IsInside(polygon, testPoint));
        }
    }
}
