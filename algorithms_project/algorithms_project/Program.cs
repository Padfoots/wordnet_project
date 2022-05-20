using System.Diagnostics;
class Program
{

    private static string output_folder;
    static void Main(string[] args)
    {
        // creating a menu to chose a test case
        string tc = Menu.create_menu();
        output_folder = tc + "\\my_output\\";
        // concatenating the folder of chosen tc with i/o files
        string synsets_file = tc + dir.synsets;
        string hypernyms_file = tc + dir.hypernyms;
        string RelationsQueries = tc + dir.Rq;
        string OutcastQueries = tc + dir.oq;

        // calling function to read the synsets file
        Synsets.read_synsets(synsets_file);

        // creating the graph 
        Stopwatch graph_sw = new Stopwatch();
        graph_sw.Start();

        Dictionary<int, List<int>> hypernyms = Hypernyms.read_hypernyms(hypernyms_file);

        graph_sw.Stop();
        Console.WriteLine("\n#######################################################################################################\n");
        Console.WriteLine("GRAPH CONSTRUCTION TIME: " + graph_sw.ElapsedMilliseconds + " ms");

        // relational queries using dfs
        Stopwatch dfs_sw = new Stopwatch();
        dfs_sw.Start();

        rq_dfs(hypernyms, RelationsQueries);

        dfs_sw.Stop();

        Console.WriteLine("\n#######################################################################################################\n");
        Console.WriteLine("RELATIONAL QUERIES TIME USING DFS: " + dfs_sw.ElapsedMilliseconds + " ms");


        // relational queries using bfs
        Stopwatch bfs_sw = new Stopwatch();
        bfs_sw.Start();

        rq_bfs(hypernyms, RelationsQueries);

        bfs_sw.Stop();
        Console.WriteLine("\n#######################################################################################################\n");
        Console.WriteLine("RELATIONAL QUERIES TIME USING BFS: " + bfs_sw.ElapsedMilliseconds + " ms");


        // reading outcast query file
        var queries = OutcastQuery.read_outcast_queries(OutcastQueries);


        // outcast queries detection
        Stopwatch outcast_q_sw = new Stopwatch();
        outcast_q_sw.Start();

        outcast_detection(hypernyms, queries);

        outcast_q_sw.Stop();

        Console.WriteLine("\n#######################################################################################################\n");
        Console.WriteLine("OUTCAST QUERY DETECTION TIME: " + outcast_q_sw.ElapsedMilliseconds + " ms");



        // more efficient solution for outcast queries detection
        Stopwatch outcast_q_sw_e = new Stopwatch();
        outcast_q_sw_e.Start();

        outcast_detection_efficient(hypernyms, queries);

        outcast_q_sw_e.Stop();

        // printing time of outcast detection
        Console.WriteLine("\n#######################################################################################################\n");
        Console.WriteLine("MORE EFFICIENT QUERY DETECTION TIME: " + outcast_q_sw_e.ElapsedMilliseconds + " ms");
        Console.WriteLine("\n#######################################################################################################\n");
    }

    static void rq_dfs(Dictionary<int, List<int>> hypernyms, string RelationsQueries)
    {
        var reader = new StreamReader(RelationsQueries);
        int n_of_queries = int.Parse(reader.ReadLine());
        List<string> output1 = new List<string>();
        string file = output_folder+"\\rq_dfs.txt";

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            string word1 = values[0];
            string word2 = values[1];

            // get the ids for word1 query and word2 query
            List<int> wordset1 = Synsets.get_ids(word1);
            List<int> wordset2 = Synsets.get_ids(word2);

            // get possible pathes from wordset1 to root node
            var word1_pathes2root = pathes2root(wordset1, hypernyms);

            // get possible pathes from wordset2 to root node
            var word2_pathes2root = pathes2root(wordset2, hypernyms);

            // printing pathes 
/*            foreach (var list in word1_pathes2root)
            {
                Console.WriteLine("\npossible pathes from node 1 to root: " + string.Join(" -> ", list) + "\n");
            }
            foreach (var list in word2_pathes2root)
            {
                Console.WriteLine("\npossible pathes from node 2 to root: " + string.Join(" -> ", list) + "\n");
            }*/

            // create a dictionary to hold depths and lcas id to select the one with the lowest depth
            Dictionary<int, int> lcas = new Dictionary<int, int>();

            // loop through all possible pathes for node 1 and 2
            foreach (var word1_path2root in word1_pathes2root)
            {
                foreach (var word2_path2root in word2_pathes2root)
                {
                    lca(word1_path2root, word2_path2root, lcas);
                }
            }

            // get the lowest depth
            int lowest_depth = lcas.Values.Min();
            
            // create a list of int to store lcas ids
            var lcas_ids=new List<int>();
            // loop through all keys that have a value of the lowest depth and add them to a list
            foreach(var lca in lcas)
            {
                if (lca.Value == lowest_depth)
                {
                    lcas_ids.Add(lca.Key);

                }
            }

            string answer = null;
            List<string> lcas_synset =new List<string>();
            foreach (var id in lcas_ids)
            {
               string s = string.Join(' ',Synsets.get_synsets(id));
               lcas_synset.Add(s);

            }
            answer= string.Join(" OR ", lcas_synset);

            // writing results to a file
            output1.Add(lowest_depth + "," + string.Join(' ', answer));
        }
        using (StreamWriter writer = new StreamWriter(file))
        {
            foreach (string ln in output1)
            {
                writer.WriteLine(ln);
            }
        }
        // To display current contents of the file
/*        Console.WriteLine(File.ReadAllText(file));*/

    }


    public static List<List<int>> pathes2root(List<int> wordset, Dictionary<int, List<int>> hypernyms)
    {
        List<List<int>> word_pathes2root = new List<List<int>>();
        foreach (int id in wordset)
        {
            //Console.WriteLine("\nlooping through id: " + id);

            // calling the dfs function to return a list of ancestors for a node
            List<int> localPathList = new List<int>();
            localPathList.Add(id);
            dfs(id, hypernyms, word_pathes2root, localPathList);


        }

        return word_pathes2root;

    }


    public static List<List<int>> dfs(int node, Dictionary<int, List<int>> hypernyms, List<List<int>> pathes2root, List<int> localPathList)
    {
        if (hypernyms[node].Count == 0)
        {

            var ls = new List<int>(localPathList);
            pathes2root.Add(ls);


        }



        foreach (var adjacent in hypernyms[node])
        {
            localPathList.Add(adjacent);
            dfs(adjacent, hypernyms, pathes2root, localPathList);
            localPathList.Remove(adjacent);

        }

        return pathes2root;
    }



    public static void lca(List<int> word1_list, List<int> word2_list, Dictionary<int, int> lcas)
    {
        int word1_depth = word1_list.Count - 1;
        int word2_depth = word2_list.Count - 1;
        int word1_current_index = 0;
        int word2_current_index = 0;
        int depth = 0;

        while (word1_depth != word2_depth)
        {
            if (word1_depth > word2_depth)
            {
                word1_current_index++;
                word1_depth--;
                depth++;
            }
            else
            {
                word2_current_index++;
                word2_depth--;
                depth++;
            }
        }

        while (word1_list[word1_current_index] != word2_list[word2_current_index])
        {
            word1_current_index++;
            word2_current_index++;
            depth = depth + 2;
        }
        var lca_id=word1_list[word1_current_index];

         if(lcas.ContainsKey(lca_id) && depth < lcas[lca_id])
         {
                lcas[lca_id] = depth;
         }
         else if (!lcas.ContainsKey(lca_id))
        {
            lcas.Add(lca_id, depth);
        }

    }


    public static void rq_bfs(Dictionary<int, List<int>> hypernyms, string RelationsQueries)
    {
        var reader = new StreamReader(RelationsQueries);
        int n_queries = int.Parse(reader.ReadLine());
        List<string> output1 = new List<string>();
        string file = output_folder+"\\bfs_output.txt";
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            string word1 = values[0];
            string word2 = values[1];

            // getting all ids of 2 queries words

            List<int> wordset1 = Synsets.get_ids(word1);
            List<int> wordset2 = Synsets.get_ids(word2);

            // get the ancestors for 2 queries list of wordset ids

            Dictionary<int, int> w1_ancestors = get_ancestors(hypernyms, wordset1);
            Dictionary<int, int> w2_ancestors = get_ancestors(hypernyms, wordset2);

            // intersection between the ancestors of the 2 words

            // find_lcas returns a dictionary that have a list of all common ancestors with the same distance as a value

            var dic = find_scas(w1_ancestors, w2_ancestors);
            var sca_ids = dic.First().Key;
            var shortest_distance = dic.First().Value;


            string scas = null;

            List<string> synset = new List<string>();

            foreach (var id in sca_ids)
            {

                string s = string.Join(' ', Synsets.get_synsets(id));
                synset.Add(s);

            }

            scas = string.Join(" OR ", synset);

            output1.Add(shortest_distance + "," + scas);

        }
        using (StreamWriter writer = new StreamWriter(file))
        {
            foreach (string ln in output1)
            {
                writer.WriteLine(ln);
            }
        }

        // To display current contents of the file

/*        Console.WriteLine("\n#######################################################################################################\n");
        Console.WriteLine(File.ReadAllText(file));
        Console.WriteLine("\n#######################################################################################################\n");*/
    }


    public static Dictionary<int, int> get_ancestors(Dictionary<int, List<int>> hypernyms, List<int> wordset_ids)
    {

        // creating a dic to hold all of word 1 ancestors including the different ids for it

        Dictionary<int, int> ancestors = new Dictionary<int, int>();

        // loop through all possible ids of word 1
        // foreach id we run a bfs on it and add all the ancestors and distances 

        foreach (var id in wordset_ids)
        {
            // an id could be already a visited ancestor of the same word in this case we should reach it from a distance 0.

            if (ancestors.ContainsKey(id))
            {
                ancestors[id] = 0;
            }
            else ancestors.Add(id, 0);

            // calling the bfs1 function to add ancestors to the w1_ancestors dic along with the depth as key

            bfs(id, hypernyms, ancestors);
        }

        return ancestors;
    }


    public static void bfs(int word, Dictionary<int, List<int>> hypernyms, Dictionary<int, int> ancestors)
    {

        Queue<int> queue = new Queue<int>();

        queue.Enqueue(word);

        while (queue.Count > 0)
        {
            int current_word = queue.Dequeue();

            foreach (var parent in hypernyms[current_word])
            {
                if (ancestors.ContainsKey(parent) && ancestors[current_word] + 1 < ancestors[parent])
                {
                    ancestors[parent] = ancestors[current_word] + 1;

                }
                if (!ancestors.ContainsKey(parent))
                {
                    ancestors.Add(parent, ancestors[current_word] + 1);

                }

                queue.Enqueue(parent);
            }


        }


    }


    public static Dictionary<List<int>, int> find_scas(Dictionary<int, int> w1_ancestors, Dictionary<int, int> w2_ancestors)
    {
        Dictionary<int, int> common_ancestors = new Dictionary<int, int>();
        common_ancestors = w1_ancestors.Where(x => w2_ancestors.ContainsKey(x.Key)).ToDictionary
            (x => x.Key, x => x.Value + w2_ancestors[x.Key]);



        // find the lowest depth


        var lowest_depth = common_ancestors.Values.Min();
        var lca_ids = new List<int>();
        foreach (var kvp in common_ancestors)
        {
            if (kvp.Value == lowest_depth)
            {
                lca_ids.Add(kvp.Key);
            }
        }
        var dic = new Dictionary<List<int>, int>();
        dic.Add(lca_ids, lowest_depth);
        return dic;
    }



    public static void outcast_detection(Dictionary<int, List<int>> hypernyms, List<List<string>> queries)
    {
        List<string > output2=new List<string>();
        string file = output_folder + "\\outcast_detection.txt";
        // loop through all queries 

        foreach (var query in queries)
        {
            // list of dictionaries to hold ancestors of every word in a query
            List<Dictionary<int, int>> query_ancestors = new List<Dictionary<int, int>>();
            
            // list of string to hold the query words
            List<string> query_words = new List<string>();

            // list of integer to hold the total sum of distances between a word and every other one in the query
            List<int> word_total_distance = new List<int>();

            // loop through all words in a query
            foreach (var word in query)
            {
                // get the word ids in a list of int
                var word_ids = Synsets.get_ids(word);

                // get all of the ancestors for the word
                var word_ancestors = get_ancestors(hypernyms, word_ids);

                // add the ancestors to the list of ancestors
                query_ancestors.Add(word_ancestors);

                // add the word to the list of string
                query_words.Add(word);

            }


            // loop through each word to get the sca and lowest distance between it and every other one
            for (int i = 0; i < query_ancestors.Count; i++)
            {
                // initialize a list to hold the lowest distances between a word i and every other words j
                List<int> min_distances = new List<int>();


                for (int j = 0; j < query_ancestors.Count; j++)
                {
                    if (i == j) continue;
                    var dic1 = query_ancestors[i];
                    var dic2 = query_ancestors[j];
                    // if item is not null in the 2d matrix only
                    Dictionary<List<int>, int> lcas = find_scas(dic1, dic2);

                    var lowest_depth = lcas.Values.Min();

                    min_distances.Add(lowest_depth);
                }

                word_total_distance.Add(min_distances.Sum());
            }

            int max_distance = word_total_distance.Max();
            List<int> indexes = new List<int>();
            List<string> syn = new List<string>();
            int index = 0;
            for (int i = 0; i < word_total_distance.Count; i++)
            {
                if (word_total_distance[i] == max_distance)
                {
                    index = i;
                    
                }
            }
            output2.Add(query_words.ElementAt(index));
        }
            using (StreamWriter writer = new StreamWriter(file))
            {
                foreach (string ln in output2)
                {
                    writer.WriteLine(ln);
                }
            }
        // To display current contents of the file
/*        Console.WriteLine("\n#######################################################################################################\n");
        Console.WriteLine(File.ReadAllText(file));
        Console.WriteLine("\n#######################################################################################################\n");
*/

    }










    public static void outcast_detection_efficient(Dictionary<int, List<int>> hypernyms, List<List<string>> queries)
    {
        List<string> output2 = new List<string>();
        string file = output_folder + "\\outcast_efficient.txt";


        foreach (var query in queries)
        {
            List<Dictionary<int, int>> query_ancestors = new List<Dictionary<int, int>>();
            List<string> query_words = new List<string>();
            List<int> word_total_distance = new List<int>();
            int[,] arr = new int[query.Count, query.Count];
            for (int i = 0; i < query.Count; i++)
            {
                for (int j = 0; j < query.Count; j++)
                {
                    arr[i, j] = -1;
                }
            }




            foreach (var word in query)
            {
                query_words.Add(word);
                var word_ids = Synsets.get_ids(word);
                var word_ancestors = get_ancestors(hypernyms, word_ids);
                query_ancestors.Add(word_ancestors);

            }



            for (int i = 0; i < query.Count; i++)
            {



                for (int j = 0; j < query.Count; j++)
                {
                    if (i == j) arr[i, j] = 0;
                    // if item is not null in the 2d matrix only
                    if (arr[i, j] == -1)
                    {
                        var dic1 = query_ancestors[i];
                        var dic2 = query_ancestors[j];
                        var lowest_depth = find_scas(dic1, dic2).Values.Min();
                        arr[i, j] = lowest_depth;
                        arr[j, i] = lowest_depth;
                    }
                }

            }

            for (int x = 0; x < query.Count; x++)
            {
                int sum = 0;
                for (int y = 0; y < query.Count; y++)
                {
                    sum += arr[x, y];
                }
                word_total_distance.Add(sum);
            }

            var index_odd_word = word_total_distance.IndexOf(word_total_distance.Max());
            var odd_word = query_words.ElementAt(index_odd_word);
            output2.Add(odd_word);

        }

        using (StreamWriter writer = new StreamWriter(file))
        {
            foreach (string ln in output2)
            {
                writer.WriteLine(ln);
            }
        }

        // To display current contents of the file
        
        /*        Console.WriteLine("\n#######################################################################################################\n");
                Console.WriteLine(File.ReadAllText(file));
                Console.WriteLine("\n#######################################################################################################\n");
        */


    }

}