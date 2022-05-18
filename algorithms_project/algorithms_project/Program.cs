using System.Diagnostics;



class Program
{
    static void Main(string[] args)
    {
       
        // creating a menu to chose a test case

        string tc = Menu.create_menu();

        // concatenating the folder of chosen tc with i/o files

        string synsets_file = tc + dir.synsets;
        string hypernyms_file = tc + dir.hypernyms;
        string RelationsQueries = tc + dir.Rq;
        string output = tc + dir.Rq_out;
        string OutcastQueries = tc + dir.oq;
        string OutcastQueries_output = tc + dir.oq_out;
        
        // read the relational queries output file for testing

        string[] output_array = RqOutput.read_output(output);

        // calling function to read the synsets files 

        Synsets.read_synsets(synsets_file);


        Stopwatch graph_sw = new Stopwatch();
        graph_sw.Start();

        Dictionary<int, List<int>> hypernyms = Hypernyms.read_hypernyms(hypernyms_file);

        graph_sw.Stop();
        TimeSpan ts1 = graph_sw.Elapsed;
        string elapsedTime1 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",ts1.Hours, ts1.Minutes, ts1.Seconds,ts1.Milliseconds / 10);
        
        Console.WriteLine("\n creating the graph time:" + elapsedTime1);

        // relational queries 

        var reader = new StreamReader(RelationsQueries);
        int n_of_queries = int.Parse(reader.ReadLine());
        int counter = 0;

        Stopwatch rq_sw = new Stopwatch();
        rq_sw.Start();

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

            // getting the output result for the current query

            string output_line = output_array[counter];
            var output_parse = output_line.Split(',');
            int ex_distance = int.Parse(output_parse[0]);
            string sca_ex_output = output_parse[1];

            Console.WriteLine("\nLCA SYNSET:      " + scas);
            Console.WriteLine("\nEXPECTED SYNSET: " + sca_ex_output);
            Console.WriteLine("\nDISTANCE: " + shortest_distance + "\t\t EXPECTED DISTANCE: " + ex_distance);
            Console.WriteLine("\n==================================================================================================\n");

            // throwing exception if the distance is wrong

            if (shortest_distance != ex_distance) throw new Exception("xxxxx THE DISTANCE IS WRONG xxxxx");

            counter++;
        }

        rq_sw.Stop();
        TimeSpan ts2 = rq_sw.Elapsed;
        string elapsedTime2 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}", ts2.Hours, ts2.Minutes, ts2.Seconds, ts2.Milliseconds / 10);
        Console.WriteLine("\n#######################################################################################################\n");
        Console.WriteLine("relational queries time:" + elapsedTime2);
        Console.WriteLine("\n#######################################################################################################\n");
        // reading the outcast query output file 

        var Outcast_query_output=OutcastQuery.read_outcast_queries_output(OutcastQueries_output);

        // outcast queries detection

        Stopwatch outcast_q_sw = new Stopwatch();
        outcast_q_sw.Start();
        var queries = OutcastQuery.read_outcast_queries(OutcastQueries);
            int c = 0;

        foreach (var query in queries)
        {
            List<Dictionary<int, int>> query_ancestors = new List<Dictionary<int, int>>();
            List<string> query_words= new List<string>();
            List<int> word_total_distance = new List<int>();

            foreach (var word in query)
            {
                var word_ids = Synsets.get_ids(word);
                var word_ancestors = get_ancestors(hypernyms, word_ids);
                query_ancestors.Add(word_ancestors);
                query_words.Add(word);

            }


            
            for (int i = 0; i < query_ancestors.Count; i++)
            {
                List<int> min_distances = new List<int>();


                for (int j = 0; j < query_ancestors.Count; j++)
                {   if (i == j) continue;
                    var dic1 = query_ancestors[i];
                    var dic2=query_ancestors[j];

                    Dictionary<List<int>, int> lcas = find_scas(dic1,dic2);

                    var lowest_depth = lcas.Values.Min();

                    min_distances.Add(lowest_depth);
                }
                
                word_total_distance.Add(min_distances.Sum());
            }

           int max_distance= word_total_distance.Max();
            List<int> indexes = new List<int>();
            List<string> syn = new List<string>();

            for (int i=0;i<word_total_distance.Count;i++ )
            {
                if (word_total_distance[i]== max_distance)
                {
                    indexes.Add(i);
                }
            }

            foreach(int i in indexes)
            {
                syn.Add(query_words[i]);
            }
            string lca_synset = string.Join(" OR ", syn);
            
            Console.WriteLine("\nOUTCAST QUERY WORD:              " + lca_synset + " with total distance of : " + max_distance);
            Console.WriteLine("\nEXPECTED OUTCAST QUERIES OUTPUT: " + Outcast_query_output[c]);
            Console.WriteLine("\n----------------------------------------------------------------------\n");

             c++;
        }
        outcast_q_sw.Stop();
        TimeSpan ts3 = outcast_q_sw.Elapsed;

        string elapsedTime3 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts3.Hours, ts3.Minutes, ts3.Seconds,
            ts3.Milliseconds/10);
        Console.WriteLine("\n  outcast query detection time:" + elapsedTime3);
        Console.WriteLine("\n");


    }

   public static List<List<int>> pathes2root(List<int> wordset, Dictionary<int, List<int>> hypernyms)
    {
        List<List<int>> word_pathes2root = new List<List<int>>();
        foreach (int id in wordset)
        {
            //Console.WriteLine("\nlooping through id: " + id);

            // calling the dfs function to return a list of ancestors for a node1
            List<int> localPathList = new List<int>();
            localPathList.Add(id);
            dfs(id, hypernyms, word_pathes2root, localPathList);


        }

        return word_pathes2root;

    }

   public static List<List<int>> dfs(int node, Dictionary<int, List<int>> hypernyms,
                                      List<List<int>> pathes2root, List<int> localPathList)
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

        if (!lcas.ContainsKey(depth)) lcas.Add(depth, word1_list[word1_current_index]);

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
}
