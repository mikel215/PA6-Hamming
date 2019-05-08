using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace PA6_Hamming
{
    class Program
    {
        static int Main(string[] args)
        {
            //encode ABC.txt to ABC.txt.coded
            EncodeFile("sample1.txt.coded");
            Console.WriteLine("File encoded.");
            /*
            if(args.Length != 2)
            {
                Console.WriteLine("Expected format: PA6.exe <encode / decode> <file_name>");
                return 0;
            }
            if(args[0] == "encode")
            {
                //encode ABC.txt to ABC.txt.coded
                EncodeFile(args[1]);
                Console.WriteLine("File encoded.");
            }
            /*
            else if(args[0] == "decode")
            {
                //decode ABC.txt.coded to ABC.decoded.txt
                DecodeFile(args[1]);
                Console.WriteLine("File decoded.");
            }
            else
            {
                Console.WriteLine("Unexpected command.");
            }
            */

            return 0;
        }

        static void EncodeFile(string fileName)
        {
            List<string> text = ParseText($"../../../sample.txt");
            Dictionary<string, string> binaryMap = GetBinary(text);
            Dictionary<string, List<bool>> hammingMap = GetHamming(binaryMap);
            
            //string txtToFile = "";
            using (FileStream stream = new FileStream($"../../../{fileName}", FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    foreach (string line in text)
                    {
                        if (line == "")
                        {
                            bw.Write("\r\n");
                            continue;
                        }
                        string[] wrds = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        foreach (string word in wrds)
                        {
                            if (wrds[wrds.Length - 1] == word)
                            {
                                //txtToFile += hammingMap[word];
                                //txtToFile += "\r\n";
                                foreach (bool bit in hammingMap[word])
                                {
                                    bw.Write(bit);
                                }
                                continue;
                            }
                            //txtToFile += hammingMap[word];
                            //txtToFile += " ";
                            foreach (bool bit in hammingMap[word])
                            {
                                bw.Write(bit);
                            }
                            bw.Write(" ");
                        }
                    }
                }
            }
            //File.WriteAllText($"../../../{ fileName }", txtToFile);
            return;
        }
        static void DecodeFile(string fileName)
        {
            return;
        }

        static List<string> ParseText(string fileName)
        {
            string line;
            List<string> list = new List<string>();
            using (StreamReader reader = File.OpenText(fileName))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    list.Add(line);
                }
            } 
            return list;
        }

        static Dictionary<string,List<bool>> GetHamming(Dictionary<string,string> keyValues)
        {
            Dictionary<string, List<bool>> hammingMap = new Dictionary<string, List<bool>>();

            // Assign each word a hammingCode
            foreach(KeyValuePair<string,string> pair in keyValues)
            {
                // list contains all 4-bit sequences per word
                List<string> list = new List<string>();
                for(int i = 0; i < pair.Value.Length; i += 4)
                {
                    list.Add(pair.Value.Substring(i, 4));
                }
                // apply hamming algorithm to each sequence
                //string fullHamming = "";
                List<bool> fullHamming = new List<bool>();
                foreach (string sequence in list)
                {
                    // transform each sequence into 4x1 matrix
                    //List<List<int>> temp = new List<List<int>>();
                    int[][] temp = new int[4][];
                    for(int i = 0; i < 4; i++)
                    {
                        int bit = Int32.Parse(sequence[i].ToString());
                        temp[i] = new int[1] { bit };
                    }
                    //matrix multiply
                    int[][] matrixCodes = Multiply(HammingCode.GetCodeGenerator(), temp);
                    for(int i = 0; i<7; i++)
                    {
                        int nextBit = matrixCodes[i][0] % 2;
                        if (nextBit == 0)
                        {
                            fullHamming.Add(false);
                        }
                        else
                        {
                            fullHamming.Add(true);
                        }
                    }
                    fullHamming.Add(false);
                    //fullHamming += "0";
                }
                hammingMap[pair.Key] = fullHamming;
            }
            return hammingMap;
        }

        static int[][] Multiply(int[][] a, int[][] b )
        {
            //List<List<int>> product = new List<List<int>>();
            int[][] product = new int[7][];
            for(int i = 0; i< 7; i++)
            {
                product[i] = new int[4];
            }

            for(int i = 0; i< a.Length; i++)
            {
                for(int j = 0; j < b.Length; j++)
                {
                    for(int k = 0; k < b.Length; k++)
                    {
                        product[i][j] += a[i][k] * b[k][0];
                    }
                }
            }
            return product;
        }

        static Dictionary<string,string> GetBinary(List<string> vs)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
            
            foreach(string line in vs)
            {
                string[] words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach(string word in words)
                {
                    /*
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] by = encoding.GetBytes(word);
                    StringBuilder binaryString = new StringBuilder();
                    foreach(byte b in by)
                    {
                        binaryString.Append(Convert.ToString(b, 2));
                    }
                    */
                    string binaryString = "";
                    foreach(char c in word)
                    {
                        binaryString += Convert.ToString(c, 2).PadLeft(8, '0');
                    }
                    keyValuePairs[word] = binaryString.ToString();
                }
            }
            return keyValuePairs;
        }
    }
}
