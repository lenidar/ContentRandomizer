using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentRandomizer
{
    internal class Program
    {
        static List<string> _fileToRead = new List<string>();
        static int[] _setUpVariables = new int[2];
        static Random _rnd = new Random();

        static void Main(string[] args)
        {
            List<string> initial = new List<string>();
            Dictionary<string, string> studentList = new Dictionary<string, string>();
            string temp = "";

            Console.Write("Reading the setup file... ");
            initial = FileRead("Setup.ini");
            Console.WriteLine(" Complete!");

            Console.Write("Parsing the setup file... ");
            temp = initial[0].Split(':')[1];
            foreach(string t in temp.Split(','))
                _fileToRead.Add(t);
            for(int x = 1; x < initial.Count; x++)
                _setUpVariables[x - 1] = int.Parse(initial[x].Split(':')[1]);
            Console.WriteLine(" Complete!");

            foreach (string _file in _fileToRead)
            {
                Console.Write($"Reading student list file {_file}... ");
                studentList = ListToDictionary(FileRead(_file));
                Console.WriteLine(" Complete!");

                Console.Write($"Beginning student list randomization. Randomizing {_setUpVariables[1]} time/s... ");
                initial = Shuffle(studentList.Keys.ToList<string>());
                Console.WriteLine(" Complete!");

                //foreach(string c in initial)
                //    Console.WriteLine($"{c} - {studentList[c]}");

                Console.Write($"Preparing to write the shuffled list. Mask is set to ");
                if (_setUpVariables[0] == 0)
                    Console.Write("false... ");
                else if (_setUpVariables[0] == 1)
                    Console.Write("true... ");

                FileWrite(_file.Split('.')[0] + "_Shuffled.csv", PrepareFinalList(initial, studentList));
                Console.WriteLine(" Complete!");
            }

            Console.ReadKey();
        }

        static List<string> FileRead(string fileName)
        {
            List<string> rawRead = new List<string>();
            using (StreamReader sr = new StreamReader(fileName))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    rawRead.Add(line);
                }
            }
            return rawRead;
        }

        static void FileWrite(string fileName, List<string> things)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                foreach (string thing in things)
                {
                    sw.WriteLine(thing);
                }
            }
        }

        static Dictionary<string, string> ListToDictionary(List<string> rawRead)
        {
            Dictionary<string, string> things = new Dictionary<string, string>();
            string[] temp = { };

            foreach(string raw in rawRead)
            {
                temp = raw.Split(',');
                things[temp[0]] = temp[1] + ',' + temp[2];
            }

            return things;
        }

        static List<string> Shuffle(List<string> keys)
        {
            Queue<string> drawOrder = new Queue<string>();
            int temp = 0;

            while(keys.Count > 0)
            {
                temp = _rnd.Next(keys.Count);
                drawOrder.Enqueue(keys[temp]);
                keys.RemoveAt(temp);
            }

            keys.Clear();
            while(drawOrder.Count > 0) 
                keys.Add(drawOrder.Dequeue());

            if (_setUpVariables[1] > 0)
            {
                _setUpVariables[1]--;
                keys = Shuffle(keys);
            }    

            return keys;
        }

        static List<string> PrepareFinalList(List<string> keys, Dictionary<string, string> studentList)
        {
            List<string> finalList = new List<string>();

            foreach (string key in keys)
            {
                if (_setUpVariables[0] == 1)
                    finalList.Add(key);
                else if (_setUpVariables[0] == 0)
                    finalList.Add($"{key} - {studentList[key]}");
            }

            return finalList;
        }
    }
}
