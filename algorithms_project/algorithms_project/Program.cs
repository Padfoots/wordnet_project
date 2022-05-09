class Program
{
    static string synsets_file = "E:\\6\\Algorithms\\project\\Testcases\\Complete\\1-Small\\Case1_100_100\\Input\\1synsets.txt";
    static string hypernyms_file = "E:\\6\\Algorithms\\project\\Testcases\\Complete\\1-Small\\Case1_100_100\\Input\\2hypernyms.txt";
    static string RelationsQueries = "E:\\6\\Algorithms\\project\\Testcases\\Complete\\1-Small\\Case1_100_100\\Input\\3RelationsQueries.txt";
    static string OutcastQueries = "E:\\6\\Algorithms\\project\\Testcases\\Complete\\1-Small\\Case1_100_100\\Input\\4OutcastQueries.txt";
    static string output = "E:\\6\\Algorithms\\project\\Testcases\\Complete\\1-Small\\Case1_100_100\\Output\\Output1.txt";


    static void Main(string[] args)
    {
        // create a dictionary for the synsets where the key is the id (int) and the value is a list of strings

        Dictionary<int, List<string>> wordNetDic = new Dictionary<int, List<string>>();


        // read data from the synsets file and put it into the dictionary

        var reader = new StreamReader(synsets_file);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(',');
            int id = int.Parse(values[0]);
            var synset = values[1].Split(' '); //[a A]
            var synsets = new List<string>(synset);
            wordNetDic.Add(id, synsets);
        }
        reader.Close();

        /*
                foreach (KeyValuePair<int, List<string>> keyValuePair in wordNetDic)
                {
                    Console.WriteLine("key:" + keyValuePair.Key + " values: " + String.Join(",", keyValuePair.Value));
                }*/


        //create an adjacency list

        var hypernyms = new Dictionary<int, List<int>>();

        // reading hypernyms file

        var reader2 = new StreamReader(hypernyms_file);
        while (!reader2.EndOfStream)
        {
            var line = reader2.ReadLine();
            var values = line.Split(',');
            int id = int.Parse(values[0]);
            var ancestors = new List<int>();

            // loop through parents in the array
            for (int i = 1; i < values.Length; i++)
            {
                ancestors.Add(int.Parse(values[i]));
            }

            hypernyms.Add(id, ancestors);
        }
        reader.Close();

        /*        foreach (KeyValuePair<int, List<int>> keyValuePair in hypernyms)
                {
                    Console.WriteLine("key:" + keyValuePair.Key + " values: " + String.Join(",", keyValuePair.Value));
                }*/


        // reading the queries


        var reader3 = new StreamReader(RelationsQueries);
        string n_of_queries = reader3.ReadLine();
        var wordset1 = new List<int>();
        var wordset2 = new List<int>();
        while (!reader3.EndOfStream)
        {
            var line = reader3.ReadLine();

            var values = line.Split(',');
            string word1 = values[0];
            string word2 = values[1];

            // call function find_ids to find all ids for the 2 queries words and return a list that contains the set of synsets 
            var founded_queries = find_ids(word1, word2, wordNetDic);
            wordset1 = founded_queries[0];
            wordset2 = founded_queries[1];

            Console.WriteLine("founded " + word1 + " with ids: {" + String.Join(",", wordset1) + "}\n");
            Console.WriteLine("founded " + word2 + " with ids: {" + String.Join(",", wordset2) + "}\n");


            // loop through all ids in a wordset
            foreach (int id in wordset1)
            {
                Console.WriteLine("looping through id: " + id);

                // calling the dfs function to return a list of ancestors for a node

                var root_path = new List<int>();
                var ancestors=new List<List<int>>();
                var word1_ancestors = dfs(id, hypernyms, root_path,ancestors);
                foreach(var l in word1_ancestors)
                {
                    
                Console.WriteLine("ancestors of word 1: (" + string.Join(',', l) + ")\n");
                }

                /*                var root_path = new List<int>();
                                 List<List<int>> ancestors = new List<List<int>>();
                                Dictionary<int,bool>visited=new Dictionary<int,bool>();
                                var word1_ancestors = dfs(id, hypernyms, root_path,ancestors);
                                foreach(var ls in word1_ancestors)
                                {
                                    Console.WriteLine("ancestors: "+ String.Join(',',ls));
                                }*/

            }
/*            foreach (int id in wordset2)
            {
                *//*Console.WriteLine("looping through id: "+ id);*//*

                // calling the dfs function to return a list of ancestors for a node

                var root_path = new List<int>();
                var word2_ancestors = dfs(id, hypernyms, root_path);


                Console.WriteLine("ancestors of word 2: (" + string.Join(',', word2_ancestors) + ")\n");
            }*/



        }
        reader.Close();
    }

    // function to traverse the dictionary to find the id of the 2 queries words
    public static List<List<int>> find_ids(string word1, string word2, Dictionary<int, List<string>> wordNetDic)
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

    // creating a dfs function that takes a node id and the adjacency list (hypernyms dictionary)
    // and returns a list of ids of ancestors till the root ancestor

    public static List<List<int>> dfs(int node, Dictionary<int, List<int>> hypernyms, List<int> path_to_root,List<List<int>>ancestors)
    {

            if(hypernyms[node].Count > 0)
            {

                ancestors.Add(path_to_root.GetRange(0, 8));


            }
        foreach (var adjacent in hypernyms[node])
        {
            path_to_root.Add(adjacent);
            dfs(adjacent, hypernyms, path_to_root,ancestors);
        }
        return ancestors;


    }


    /*    public static List<List<int>> dfs(int node, Dictionary<int, List<int>> hypernyms, List<int> path_to_root,List<List<int>>ancestors)
        {

            // if we reached the root and still other adjacencies wasn't visited we should separate the 

                Stack<int> stk = new Stack<int>(); 
                stk.Push(node);

            while(stk.Count > 0)
            {
                node = stk.Peek();
                path_to_root.Add(node);
                stk.Pop();
                if (hypernyms[node].Count == 0)
                {
                    ancestors.Add(path_to_root);
                    stk.Clear();


                }
                foreach(var adjacents in hypernyms[node])
                {
                    stk.Push(adjacents);
                }
            }

            return ancestors;




        }*/
    /*    public static List<List<int>> ancestors;
    */
    /*    public static List<List<int>> dfs(int node, List<list<int>> hypernyms , List<int> root_path,)
        {
            if (hypernyms[node].Count() == 0)
            {
                // add the list to ancestors list

                // clear the list
            }
            is_visited.Add(node, true);
            foreach (int adjacents in hypernyms[node])
            {
                if (!is_visited[adjacents])
                {
                    root_path.Add(adjacents);
                    dfs(adjacents, is_visited, root_path, hypernyms);
                }
                root_path.Remove(adjacents);
            }
        }*/

}




