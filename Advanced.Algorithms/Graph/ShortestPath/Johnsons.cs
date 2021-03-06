﻿using Advanced.Algorithms.DataStructures.Graph.AdjacencyList;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Advanced.Algorithms.Graph
{ 
    /// <summary>
    /// generic operators
    /// </summary>
    /// <typeparam name="W"></typeparam>
    public interface IJohnsonsShortestPathOperators<T, W>
        : IShortestPathOperators<W> where W : IComparable
    {

        /// <summary>
        /// Substract a from b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        W Substract(W a, W b);

        /// <summary>
        /// gives a random vertex not in the graph
        /// </summary>
        /// <returns></returns>
        T RandomVertex();
    }

    public class JohnsonsShortestPath<T, W> where W : IComparable
    {
        readonly IJohnsonsShortestPathOperators<T, W> operators;
        public JohnsonsShortestPath(IJohnsonsShortestPathOperators<T, W> operators)
        {
            this.operators = operators;
        }

        public List<AllPairShortestPathResult<T, W>>
            GetAllPairShortestPaths(WeightedDiGraph<T, W> graph)
        {

            var workGraph = graph.Clone();

            //add an extra vertex with zero weight edge to all nodes
            var randomVetex = operators.RandomVertex();

            if (workGraph.Vertices.ContainsKey(randomVetex))
            {
                throw new Exception("Random Vertex is not unique for given graph.");
            }
            workGraph.AddVertex(randomVetex);

            foreach (var vertex in workGraph.Vertices)
            {
                workGraph.AddEdge(randomVetex, vertex.Key, operators.DefaultValue);
            }

            //now compute shortest path from random vertex to all other vertices
            var bellmanFordSp = new BellmanFordShortestPath<T, W>(operators);
            var bellFordResult = new Dictionary<T, W>();
            foreach (var vertex in workGraph.Vertices)
            {
                var result = bellmanFordSp.GetShortestPath(workGraph, randomVetex, vertex.Key);
                bellFordResult.Add(vertex.Key, result.Length);
            }

            //adjust edges so that all edge values are now +ive
            foreach (var vertex in workGraph.Vertices)
            {
                foreach (var edge in vertex.Value.OutEdges.ToList())
                {
                    vertex.Value.OutEdges[edge.Key] = operators.Substract(
                        operators.Sum(bellFordResult[vertex.Key], edge.Value),
                        bellFordResult[edge.Key.Value]);
                }

            }

            workGraph.RemoveVertex(randomVetex);
            //now run dijikstra for all pairs of vertices
            //trace path
            var dijikstras = new DijikstraShortestPath<T, W>(operators);
            var finalResult = new List<AllPairShortestPathResult<T, W>>();
            foreach (var vertexA in workGraph.Vertices)
            {
                foreach (var vertexB in workGraph.Vertices)
                {
                    var source = vertexA.Key;
                    var dest = vertexB.Key;  
                    var sp = dijikstras.GetShortestPath(workGraph, source, dest);

                    //no path exists
                    if(sp.Length.Equals(operators.MaxValue))
                    {
                        continue;
                    }

                    var distance = sp.Length;
                    var path = sp.Path;

                    finalResult.Add(new AllPairShortestPathResult<T, W>(source, dest, distance, path));

                }
            }

            return finalResult;
        }

    }
}
