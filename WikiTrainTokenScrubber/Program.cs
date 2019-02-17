using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiTrainTokenScrubber
{
    class Program
    {

        static void HashCharsMatrixes()
        {
            Random rand = new Random();
            var index = new List<string>();

            //Load document
            using (var sr = new StreamReader("C:\\Users\\brett\\Documents\\MATLAB\\wikitext-2\\wiki.train.tokens"))
            {
                using (var sw = new StreamWriter("C:\\Users\\brett\\Documents\\MATLAB\\wikitext-2\\wiki.train.txt"))
                {
                    //Read to end of the stream.
                    while (!sr.EndOfStream)
                    {

                        var masterList = new List<KeywordPair>();

                        // Get the current line
                        var sline = sr.ReadLine();
                        // Make sure the line actually has words
                        if (sline != "" && sline != " ")
                        {
                            // Get rid of the equal signs
                            sline = sline.Replace('=', ' ');
                            // Create an array of words by splitting up the line by spaces.
                            var splity = sline.Split(' ');
                            // Make sure its not an empty spot and iterate through the array.
                            if (splity.Length > 1)
                                for (int i = 0; i < splity.Length - 1; i++)
                                {
                                    //Strip the words of non-alphabetical characters and add the current word and the next word to the list.
                                    masterList.Add(new KeywordPair(KeywordPair.GetOnlyChars(splity[i]), KeywordPair.GetOnlyChars(splity[i + 1])));
                                }
                        }

                        foreach (var s in masterList)
                        {
                            // Make sure that none of the words are empty or "unk" (I think it means unknown)
                            if (s.First != "" && s.Second != "" && s.First != "unk" && s.Second != "unk")
                            {
                                // Initialize arrays for the word
                                var mats1 = new int[18];
                                var mats2 = new int[18];
                                // Iterate through the characters in the word
                                for (int i = 0; i < 18; i++)
                                {
                                    // Make sure the word is long enough
                                    if (s.First.Length > i)
                                    {
                                        // Convert the char to its ASCII index integer 
                                        mats1[i] = KeywordPair.GetCharBin(s.First[i], rand);
                                    }
                                    else
                                    {
                                        // Add some int value that is not an alphabetical ASCII int.
                                        mats1[i] = KeywordPair.GetCharBin(' ', rand);
                                    }
                                    // Do the same thing for the target
                                    if (s.Second.Length > i)
                                    {
                                        mats2[i] = KeywordPair.GetCharBin(s.Second[i], rand);
                                    }
                                    else
                                    {
                                        // Put the value on the lower end of the the ASCII index for the target. Hopefully it does something kool.
                                        mats2[i] = KeywordPair.GetCharBin(' ', rand);
                                    }
                                }

                                sw.WriteLine(KeywordPair.ConvertMatToString(mats1) + " " + KeywordPair.ConvertMatToString(mats2));
                                Console.WriteLine(KeywordPair.ConvertMatToString(mats1) + " " + KeywordPair.ConvertMatToString(mats2));
                            }
                        }

                    }
                }
            }

        }

        static string Alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ.?! ";

        static string ConvertFloatListToString(List<float> listy)
        {
            var result = "";
            result += listy[0];
            for (int i = 1; i < listy.Count; i++)
            {
                result += " " + listy[i];
            }

            return result;

        }


        //1349980
        static void Main(string[] args)
        {
            //Scrub into array based off word index.

            Random rand = new Random();
            var index = new List<string>();

            //Load document
            using (var sr = new StreamReader("C:\\Users\\brett\\Documents\\MATLAB\\wikitext-2\\wiki.train.tokens"))
            {
                using (var sw = new StreamWriter("C:\\Users\\brett\\Documents\\MATLAB\\wikitext-2\\wikiindextokens.txt"))
                {
                    //Read to end of the stream.
                    while (!sr.EndOfStream)
                    {


                        // Get the current line
                        var sline = sr.ReadLine();
                        // Make sure the line actually has words and isn't a category
                        if (sline != "" && sline != " " && !sline.Contains('='))
                        {


                            // Remove non-alphabetical characters or non-sentence ending chars
                            var schars = sline;
                            foreach (var c in schars)
                            {
                                if (!Alphabet.Contains(c) && c != ' ')
                                {
                                    sline = sline.Replace(c.ToString(), "");
                                }
                            }

                            // Get the sentences
                            var sentences = sline.Split('.', '?', '!');

                            //Iterate through the sentences
                            foreach (var sentence in sentences)
                            {
                                // Initalize array of floats (our 5x5 matrix represented in 1-dim)
                                var array = new List<float>();
                                // Create an array of words by splitting up the line by spaces.
                                var splity = sentence.Split(' ');
                                // Make sure its not an empty spot or a sentence with more than 25 words
                                //      and iterate through the words.
                                //      I also don't want the sentences with "unk"
                                if (splity.Length > 1 && splity.Length < 26 && !splity.Contains("unk"))
                                {
                                    for (int i = 0; i < splity.Length; i++)
                                    {
                                        if (index.Exists(x => x == splity[i]))
                                        {
                                            array.Add(index.FindIndex(x => x == splity[i]) + 1);
                                        }
                                        else
                                        {
                                            index.Add(splity[i]);
                                            array.Add(index.Count);
                                        }
                                    }
                                    for (int i = splity.Length; i < 26; i++)
                                    {
                                        array.Add(0);
                                    }
                                    sw.WriteLine(ConvertFloatListToString(array));
                                }



                            }
                        }




                    }


                }
                using (var sw = new StreamWriter("C:\\Users\\brett\\Documents\\MATLAB\\wikitext-2\\wikiindex.txt"))
                {
                    for(int i = 0; i < index.Count; i++)
                    {
                        sw.WriteLine(i + " " + index[i]);
                    }
                }
            }

        }


    }
}



class KeywordPair
{
    public string First { get; set; }
    public string Second { get; set; }
    public KeywordPair(string first, string second)
    {
        First = first;
        Second = second;
    }

    private static string alphabet = "abcdefghijklmnopqrstuvwxyz";

    public static string GetOnlyChars(string s)
    {
        var newstring = "";
        foreach (var c in s)
        {
            if (alphabet.Contains(c) || alphabet.ToUpper().Contains(c))
            {
                newstring += c;
            }
        }
        return newstring;
    }

    public static int GetCharBin(char c, Random rand)
    {

        if (alphabet.Contains(c))
        {
            return ((alphabet.IndexOf(c) + 1) * 10) + rand.Next(0, 9);
        }
        else if (alphabet.ToUpper().Contains(c))
        {
            return ((alphabet.ToUpper().IndexOf(c) + 1) * 10) + rand.Next(0, 9) + 260;
        }
        else
        {
            return rand.Next(0, 9);
        }
    }

    public static string ConvertMatToString(int[] mat)
    {
        var s = "";
        foreach (var inty in mat)
        {
            s += inty + " ";
        }
        s = s.Remove(s.Length - 1);
        return s;
    }
}


