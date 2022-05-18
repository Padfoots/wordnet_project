
    internal class OutcastQuery
{
            // initialize function read outcast queries
    public static List<List<string>> read_outcast_queries(string file_name)
        {
            var reader = new StreamReader(file_name);

        // the first line contains the number of queries
       
        var n_outcastQueries=reader.ReadLine();
        
            List<List<string>> queries= new List<List<string>>();
            while (!reader.EndOfStream)
            {
            List<string> query = new List<string>();
                var line = reader.ReadLine();
                var words = line.Split(',');
                foreach(string word in words)
                {
                    query.Add(word);

                }
                queries.Add(query);
                

    
            }

         reader.Close();
         return queries;
        }



    public static List<string> read_outcast_queries_output(string file_name)
    {

        var reader = new StreamReader(file_name);


        List<string> outcast_queries = new List<string>();
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            outcast_queries.Add(line);
        }
        return outcast_queries;
    }
            
    
}

