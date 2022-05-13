using System.Diagnostics;



class Program
{

    static string tc;

    static void Main(string[] args)
    {
        // creating a menu of test cases

        Console.WriteLine("enter a testcase number:\n " +
            "1-Small\\Case1_100_100 \n" +
            "2-Small\\Case2_1000_500 \n"+
            "3-Medium\\Case1_10000_5000 \n"+
            "4-Medium\\Case2_10000_50000 \n" +
            "5-Large\\Case1_82K_100K_5000RQ \n"+
            "6-Large\\Case2_82K_300K_1500RQ\n"+
            "7-Large\\Case3_82K_300K_5000RQ\n");

        char choice=(char)Console.ReadLine()[0];
        switch (choice)
        {
            case '1': tc= dir.c_s1; break;
            case '2': tc= dir.c_s2; break;
            case '3': tc = dir.c_m1;break;
            case '4': tc=dir.c_m2;break;
            case '5': tc = dir.c_l1; break;
            case '6': tc = dir.c_l2; break;
            case '7': tc = dir.c_l3; break;


        }

        string synsets_file = tc + dir.synsets;
        string hypernyms_file = tc + dir.hypernyms;
        string RelationsQueries = tc + dir.Rq;
        string output = tc + dir.Rq_out;
        string OutcastQueries = tc + dir.oq;
        string OutcastQueries_output = tc + dir.oq_out;






        // calling function to read synsets_file and return a dictionary 

        Dictionary<int, List<string>> wordnet_dic = Synsets.read_synsets(synsets_file);

        // calling function to read hypernyms and return a dictionary with the child as key and a list of parents
        Stopwatch graph_sw=  new Stopwatch();
        graph_sw.Start();
        Dictionary<int, List<int>> hypernyms = Hypernyms.read_hypernyms(hypernyms_file);
        graph_sw.Stop();
        TimeSpan ts1 = graph_sw.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime1 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts1.Hours, ts1.Minutes, ts1.Seconds,
            ts1.Milliseconds / 10);
        Console.WriteLine("\n creating the graph time:" +elapsedTime1 );


        // reading the queries


        var reader3 = new StreamReader(RelationsQueries);

        // initialize the number of queries
        int n_of_queries = int.Parse(reader3.ReadLine());
        int counter = 0;


        Stopwatch rq_sw = new Stopwatch();
        rq_sw.Start();
        
        while (!reader3.EndOfStream)
        {   
            var line = reader3.ReadLine();

            var values = line.Split(',');
            string word1 = values[0];
            string word2 = values[1];

            var founded_queries = find_2ids(word1, word2, wordnet_dic);


            List<int> wordset1 = founded_queries[0];
            List<int> wordset2 = founded_queries[1];

            Console.WriteLine("\nfounded word1--> " + "\"" + word1 + "\" " + "with ids: {" + String.Join(",", wordset1) + "}\n");
            Console.WriteLine("\nfounded word2--> " + "\"" + word2 + "\" " + "with ids: {" + String.Join(",", wordset2) + "}\n");


            var word1_pathes2root = pathes2root(wordset1, hypernyms);

            foreach (var list in word1_pathes2root)
            {
                Console.WriteLine("\npossible pathes from node to root: " + string.Join(" -> ", list) + "\n");
            }

            var word2_pathes2root = pathes2root(wordset2, hypernyms);

            foreach (var list in word2_pathes2root)
            {
                Console.WriteLine("\npossible pathes from node to root: " + string.Join(" -> ", list) + "\n");
            }




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

            int lowest_depth = lcas.Keys.Min();

            // get the index of the value at the lowest depth

            int lca_index = lcas[lowest_depth];

            // get the id of the lca

            int lca_id = lcas[lowest_depth];

            // get the synset of the lca
            List<string> lca_synset = wordnet_dic[lca_id];

            // read the output file and compare with our result
            string[] output_array = RqOutput.read_output(output);
            if (counter != n_of_queries)
            {


                string output_line = output_array[counter];
                var output_parse = output_line.Split(',');
                int actual_depth = int.Parse(output_parse[0]);
                string lca_actual_synset = output_parse[1];

                if (lowest_depth != actual_depth)
                {
                    Console.WriteLine("xxxxxxxxxxxxxxxx      depth is wrong      xxxxxxxxxxxxxxxxxxxxx");
                    throw new Exception("the depth is wrong");
                }

                Console.WriteLine("depth: " + lowest_depth + "\t\t expected depth is: " + actual_depth);
                Console.WriteLine("lca synset \"" + string.Join(',', lca_synset) + "\"" + "\t" +
                                  " actual lca synset: \"" + lca_actual_synset + "\"");
                Console.WriteLine("==================================================================================================");

            }

            counter++;
            lcas.Clear();
        }
        rq_sw.Stop();
        TimeSpan ts2 = rq_sw.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime2 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts2.Hours, ts2.Minutes, ts2.Seconds,
            ts2.Milliseconds / 10);
        Console.WriteLine("\n relational queries time:" + elapsedTime2);
        Console.WriteLine("\n");
        // outcast queries

        Stopwatch outcast_q_sw=new Stopwatch();
        outcast_q_sw.Start();
        var queries = OutcastQuery.read_outcast_queries(OutcastQueries);
        foreach (var query in queries)
        {

            var outcastQueryDic = OutcastQuery.find_ids(wordnet_dic, query);
            var outcast_dic = new Dictionary<string, int>();

            // call the lca
            for (int i = 0; i < outcastQueryDic.Count; i++)
            {
                var wordList = new List<int>();
                for (int j = 1; j < outcastQueryDic.Count; j++)
                {
                    var word1_ids = outcastQueryDic.ElementAt(i).Value;
                    var word2_ids = outcastQueryDic.ElementAt(j).Value;
                    var word1_pathes2root = pathes2root(word1_ids, hypernyms);
                    var word2_pathes2root = pathes2root(word2_ids, hypernyms);

                    //

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

                    int lowest_depth = lcas.Keys.Min();
                    wordList.Add(lowest_depth);





                }
                outcast_dic.Add(outcastQueryDic.ElementAt(i).Key, wordList.Sum());

            }
            var maximum_depth = outcast_dic.Values.Max();
            foreach (KeyValuePair<string, int> kvp in outcast_dic)
            {
                if (kvp.Value == maximum_depth)
                {
                    Console.WriteLine("odd word is : " + kvp.Key + " with total depth:  " + kvp.Value);
                    break;
                }

            }


            // sum distances 

            // save the total distance into an list

        }
        outcast_q_sw.Stop();
        TimeSpan ts3 = outcast_q_sw.Elapsed;

        // Format and display the TimeSpan value.
        string elapsedTime3 = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts3.Hours, ts3.Minutes, ts3.Seconds,
            ts3.Milliseconds / 10);
        Console.WriteLine("\n  outcast query detection time:" + elapsedTime3);
        Console.WriteLine("\n");

    }

    // functions 

    // function to traverse the dictionary to find the id of the 2 queries words

    public static List<List<int>> find_2ids(string word1, string word2, Dictionary<int, List<string>> wordNetDic)
    {
        var founded_synsets = new List<List<int>>();
        var word1_synset_list = new List<int>();
        var word2_synset_list = new List<int>();
        foreach (KeyValuePair<int, List<string>> x in wordNetDic)
        {
            if (x.Value.Contains(word1))
            {
                word1_synset_list.Add(x.Key);
            }
            if (x.Value.Contains(word2))
            {
                word2_synset_list.Add(x.Key);
            }
        }
        founded_synsets.Add(word1_synset_list);
        founded_synsets.Add(word2_synset_list);
        return founded_synsets;
    }

    // function pathes2root
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

    static List<int> list = new List<int>();
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
    

}
