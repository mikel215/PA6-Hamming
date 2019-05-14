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
            //EncodeFile("sample.txt");
            //DecodeFile("sample.txt.coded");
            if(args.Length != 2)
            {
                //encode ABC.txt to ABC.txt.coded
                Console.WriteLine("Expected format: PA6.exe <encode / decode> <file_name>");
                return 0;
            }
            if(args[0] == "encode")
            {
                //encode ABC.txt to ABC.txt.coded
                EncodeFile(args[1]);
                Console.WriteLine("File encoded.");
            }
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

            return 0;
        }

        static void EncodeFile(string fileName)
        {
            List<string> text = ParseText(fileName);
            var binaryMap = GetBinary(text);
            var hammingMap = GetHamming(binaryMap);
            
            using (FileStream stream = new FileStream($"{fileName}.coded", FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    foreach(List<bool> binaryLine in hammingMap)
                    {
                        foreach (bool bit in binaryLine)
                        {
                            bw.Write(bit);
                        }
                    }
                }
            }
            return;
        }
        static void DecodeFile(string fileName)
        {
            // split string into 8 bit strings
            List<string> bin = ParseBinary(fileName);
            int errors = 0;
            List<string> binaryDecoded = DecodeBits(bin, ref errors);
            if(errors > 0)
            {
                Console.WriteLine("Error in file has been detected.\r\n" +
                                   "Would you like to keep attempting to decode?(y/n)\n\r");
                var ans = Console.Read();
                if (ans == 'n')
                {
                    Console.WriteLine("Program Terminating");
                    Environment.Exit(0);
                }

            }
            List<string> binChar = new List<string>();
            for(int i = 0; i < binaryDecoded.Count; i += 2)
            {
                string completeChar = "";
                completeChar += binaryDecoded[i];
                completeChar += binaryDecoded[i + 1];
                binChar.Add(completeChar);
            }
            var binList = new List<byte>();
            foreach(string binSeq in binChar)
            {
                binList.Add(Convert.ToByte(binSeq,2));
            }
            Byte[] getChar = binList.ToArray();
            var txt = Encoding.ASCII.GetString(getChar);

            File.WriteAllText(fileName + ".decoded.txt", txt);

            return;
        }

        static List<string> DecodeBits(List<string> list, ref int errors)
        {
            List<string> returnList = new List<string>();
            foreach (string sequence in list)
            {
                int[][] temp = new int[7][];
                for (int i = 0; i < 7; i++)
                {
                    int bit = Int32.Parse(sequence[i].ToString());
                    temp[i] = new int[1] { bit };
                }
                //matrix multiply for error checking
                int[][] matrixCodes = MultiplyDecode(HammingCode.GetParityMatrix(), temp);
                List<int> vs = new List<int>()
                {
                    (matrixCodes[2][0] % 2),
                    (matrixCodes[1][0] % 2),
                    (matrixCodes[0][0] % 2)
                };
                if(vs.Contains(1))
                {
                    errors++;
                }
                string stuff = "";
                for(int i = 2; i < 7; i ++)
                {
                    if(i == 3)
                    {
                        continue;
                    }
                    stuff += temp[i][0].ToString();
                }
                returnList.Add(stuff);
            }
            return returnList;
        }

        static int[][] MultiplyDecode(int[][] a, int[][] b )
        {
            //List<List<int>> product = new List<List<int>>();
            int[][] product = new int[3][];
            for(int i = 0; i< 3; i++)
            {
                product[i] = new int[8];
            }
            int a1 = a.Length;
            int b1 = b.Length;

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

        static List<string> ParseBinary(string fileName)
        {
            List<string> list = new List<string>();
            /*
            string txt = File.ReadAllText($"../../../{fileName}");
            string newString = "";
            for(int i = 0; i < txt.Length; i += 2)
            {
                string substr = txt.Substring(i, 1); 
                if(substr == "\0" || substr == "\\u")
                {
                    continue;
                }
                newString += substr;

            }
            */
            byte[] fileBytes = File.ReadAllBytes(fileName);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in fileBytes)
            {
                sb.Append(b.ToString());
            }
            string workString = sb.ToString();
            for(int i = 0; i < workString.Length; i += 8)
            {
                list.Add(workString.Substring(i, 8));
            }
            return list;
        }

        static List<List<bool>> GetHamming(List<string>keyValues)
        {
            //Dictionary<string, List<bool>> hammingMap = new Dictionary<string, List<bool>>();
            var fullHamming = new List<List<bool>>();

            // Assign each word a hammingCode
            foreach(string line in keyValues)
            {
                var lineToBinary = new List<bool>();
                // list contains all 4-bit sequences per line
                List<string> list = new List<string>();
                for(int i = 0; i < line.Length; i += 4)
                {
                    list.Add(line.Substring(i, 4));
                }
                // apply hamming algorithm to each sequence
                foreach (string sequence in list)
                {
                    // transform each sequence into 4x1 matrix
                    int[][] temp = new int[4][];
                    for(int i = 0; i < 4; i++)
                    {
                        int bit = Int32.Parse(sequence[i].ToString());
                        temp[i] = new int[1] { bit };
                    }
                    //matrix multiply
                    int[][] matrixCodes = MultiplyEncode(HammingCode.GetCodeGenerator(), temp);
                    for(int i = 0; i<7; i++)
                    {
                        int nextBit = matrixCodes[i][0] % 2;
                        if (nextBit == 0)
                        {
                            lineToBinary.Add(false);
                        }
                        else
                        {
                            lineToBinary.Add(true);
                        }
                    }
                    lineToBinary.Add(false);
                    //fullHamming += "0";
                }
                //hammingMap[pair.Key] = fullHamming;
                fullHamming.Add(lineToBinary);
            }
            return fullHamming;
        }

        static int[][] MultiplyEncode(int[][] a, int[][] b )
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

        static List<string> GetBinary(List<string> vs)
        {
            var list = new List<string>();           
            foreach(string line in vs)
            {
                //string[] words = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                /*
                foreach(string word in words)
                {
                    UTF8Encoding encoding = new UTF8Encoding();
                    byte[] by = encoding.GetBytes(word);
                    StringBuilder binaryString = new StringBuilder();
                    foreach(byte b in by)
                    {
                        binaryString.Append(Convert.ToString(b, 2));
                    }
                    string binaryString = "";
                    foreach(char c in word)
                    {
                        binaryString += Convert.ToString(c, 2).PadLeft(8, '0');
                    }
                    keyValuePairs[word] = binaryString.ToString();
                }
                */
                string binaryString = "";
                foreach(char c in line)
                {
                    binaryString += Convert.ToString(c, 2).PadLeft(8, '0');
                }
                list.Add(binaryString);
            }
            return list;
        }
    }
}
