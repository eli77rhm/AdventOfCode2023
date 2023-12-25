using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using QuickGraph;

namespace Aoc23
{
    public class Day25 : AllDays
    {
        public Day25() : base("Day25")
        {
        }

        public override void ExecutePart1()
        {
            var graph = new ComponentGraph();
            foreach (var line in Lines)
            {
                var split = line.Trim().Split(": ");
                string component = split[0];
                split = split[1].Split(' ');
                HashSet<string> connections = new HashSet<string>(split);

                if (!graph.Vertices.Contains(component))
                {
                    graph.Vertices.Add(component);
                }

                foreach (var connection in connections)
                {
                    if (!graph.Vertices.Contains(connection))
                    {
                        graph.Vertices.Add(connection);
                    }

                    if (!graph.Edges.Contains((component, connection)) &&
                        !graph.Edges.Contains((connection, component)))
                    {
                        graph.Edges.Add((component, connection));
                    }
                }
            }

            //theory from : https://en.wikipedia.org/wiki/Karger%27s_algorithm
            //implementation from : https://www.geeksforgeeks.org/introduction-and-implementation-of-kargers-algorithm-for-minimum-cut/ 
            //we should run the algorithm multiple times so it will give us new subset each time
            // the one with  number of cuts more 3 is the answer

            var subsets = new SubsetClass();
            var count = 0;
            while (count != 3)
            {
                subsets = new SubsetClass();
                KargerAlgorithm(graph, subsets);

                count = subsets.NumberOfCuts(graph.Edges);
            }
            var result = subsets.Subsets[0].Vertices.Count * subsets.Subsets[1].Vertices.Count;

            Console.WriteLine(result);

        }


        public override void ExecutePart2()
        {

        }

        public class SubsetClass
        {
            public List<Subset> Subsets = new List<Subset>();

            public SubsetClass()
            {
                Subsets.Clear();
                Subsets = new List<Subset>();
            }

            public void AddItem(Subset subset)
            {
                Subsets.Add(subset);
            }

            public void RemoveItem(Subset subset)
            {
                Subsets.Remove(subset);
            }

            public Subset Find(string vertex)
            {
                foreach (var subset in Subsets)
                {
                    if (subset.Vertices.Contains(vertex))
                        return subset;
                }

                return null;
            }

            public int NumberOfCuts(List<(string, string)> Edges)
            {
                int numberOfCuts = 0;
                foreach ((string source, string target) edge in Edges)
                {
                    var sub1 = Find(edge.source);
                    var sub2 = Find(edge.target);

                    if (sub1 != sub2)
                        numberOfCuts++;
                }

                return numberOfCuts;
            }
        }

        public class Subset
        {
            public List<string> Vertices { get; set; }

            public Subset(List<string> v)
            {
                Vertices = v;
            }
        }

        // A very basic implementation of Karger's randomized
        // algorithm for finding the minimum cut. Please note
        // that Karger's algorithm is a Monte Carlo Randomized algo
        // and the cut returned by the algorithm may not be
        // minimum always
        public void KargerAlgorithm(ComponentGraph graph, SubsetClass subsets)
        {
            for (int v = 0; v < graph.Vertices.Count; ++v)
            {
                subsets.AddItem(new Subset(new List<string>() { graph.Vertices[v] }));
            }
            int vertices = graph.Vertices.Count;

            while (vertices > 2)
            {
                // Pick a random edge
                int i = (new Random().Next()) % graph.Edges.Count;

                // Find vertices (or sets) of two corners
                // of current edge
                var subset1 = subsets.Find(graph.Edges[i].Item1);
                var subset2 = subsets.Find(graph.Edges[i].Item2);

                // If two corners belong to same subset,
                // then no point considering this edge
                if (subset1 == subset2)
                {
                    continue;
                }
                // Else contract the edge (or combine the
                // corners of edge into one vertex)
                else
                {
                    vertices--;
                    //Console.WriteLine("Contracting edge " + edges[i].Item1 + "-" + edges[i].Item2);
                    //Union(subsets, subset1, subset2);
                    subsets.RemoveItem(subset2);
                    subset1.Vertices.AddRange(subset2.Vertices);
                }
            }
        }

        private List<Edge<string>> FindBestEdgesToRemove(UndirectedGraph<string, Edge<string>> graph)
        {
            int maxProduct = 0;
            List<Edge<string>> bestEdgesToRemove = null;

            var allEdges = graph.Edges.ToList();
            for (int i = 0; i < allEdges.Count; i++)
            {
                for (int j = i + 1; j < allEdges.Count; j++)
                {
                    for (int k = j + 1; k < allEdges.Count; k++)
                    {
                        // Remove edges
                        var edgesToRemove = new List<Edge<string>> { allEdges[i], allEdges[j], allEdges[k] };
                        foreach (var edge in edgesToRemove)
                            graph.RemoveEdge(edge);

                        // Calculate the product of the sizes of the two largest components
                        var sizes = FindLargestComponents(graph);
                        int product = sizes[0] * sizes[1];

                        // Check if this is the best solution so far
                        if (product > maxProduct)
                        {
                            maxProduct = product;
                            bestEdgesToRemove = edgesToRemove.ToList();
                        }

                        // Re-add edges for the next iteration
                        foreach (var edge in edgesToRemove)
                            graph.AddEdge(edge);
                    }
                }
            }

            return bestEdgesToRemove;
        }

        private List<int> FindLargestComponents(UndirectedGraph<string, Edge<string>> graph)
        {
            var visited = new HashSet<string>();
            var sizes = new List<int>();

            foreach (var vertex in graph.Vertices)
            {
                if (!visited.Contains(vertex))
                {
                    int size = DFS(graph, vertex, visited);
                    sizes.Add(size);
                }
            }

            sizes.Sort((a, b) => b.CompareTo(a)); // Sort in descending order
            return sizes.Take(2).ToList(); // Return the two largest
        }

        private int DFS(UndirectedGraph<string, Edge<string>> graph, string start, HashSet<string> visited)
        {
            var stack = new Stack<string>();
            stack.Push(start);
            int size = 0;

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (visited.Contains(current))
                    continue;

                visited.Add(current);
                size++;
                foreach (var edge in graph.AdjacentEdges(current))
                {
                    var neighbor = edge.GetOtherVertex(current);
                    if (!visited.Contains(neighbor))
                    {
                        stack.Push(neighbor);
                    }
                }
            }

            return size;
        }


        public class ComponentGraph
        {
            private Dictionary<string, HashSet<string>> graph { get; set; }
            public List<(string, string)> Edges { get; set; }
            public List<string> Vertices { get; set; }

            public ComponentGraph()
            {
                graph = new Dictionary<string, HashSet<string>>();
                Edges = new List<(string, string)>();
                Vertices = new List<string>();
            }

            public void AddConnection(string a, string b)
            {
                if (!graph.ContainsKey(a))
                    graph[a] = new HashSet<string>();
                if (!graph.ContainsKey(b))
                    graph[b] = new HashSet<string>();

                graph[a].Add(b);
                graph[b].Add(a);
            }

            public void RemoveConnection(string a, string b)
            {
                if (graph.ContainsKey(a))
                    graph[a].Remove(b);
                if (graph.ContainsKey(b))
                    graph[b].Remove(a);
            }

            public int GroupSize(string start)
            {
                var visited = new HashSet<string>();
                var stack = new Stack<string>();
                stack.Push(start);
                int size = 0;

                while (stack.Count > 0)
                {
                    var current = stack.Pop();
                    if (visited.Contains(current))
                        continue;

                    visited.Add(current);
                    size++;
                    foreach (var neighbor in graph[current])
                    {
                        if (!visited.Contains(neighbor))
                        {
                            stack.Push(neighbor);
                        }
                    }
                }

                return size;//visited.Count;
            }

            public IEnumerable<string> Components => graph.Keys;

            public IEnumerable<Tuple<string, string>> Connections
            {
                get
                {
                    var connections = new HashSet<Tuple<string, string>>();
                    foreach (var kvp in graph)
                    {
                        foreach (var neighbor in kvp.Value)
                        {
                            var connection = new Tuple<string, string>(kvp.Key, neighbor);
                            var reversedConnection = new Tuple<string, string>(neighbor, kvp.Key);

                            if (!connections.Contains(reversedConnection))
                                connections.Add(connection);
                        }
                    }
                    return connections;
                }
            }
        }
    }
}
