
internal class RqOutput
{
    // create function read_output that takes 2 input a counter and the string file
    // reads all lines in the file and returns a dictionary containing 
    // the depth and the lca synset
    public static string[] read_output(string file)
    {
        string[] output = File.ReadAllLines(file);
        return output;
    }



}

