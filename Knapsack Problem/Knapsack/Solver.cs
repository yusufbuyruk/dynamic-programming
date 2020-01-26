using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace Solver
{
    class Item
    {
        public int Value { get; set; }
        public int Weight { get; set; }
        public int Index { get; set; }
        public float Ratio { get; set; }

        public Item(int index, int value, int weight)
        {
            Index = index;
            Value = value;
            Weight = weight;
            Ratio = (float)value / (float)weight;
        }
    }

    class Solution
    {
        public List<Item> Items { get; set; }
        public List<Item> ItemsByRatioDesc { get; set; }
        public List<Item> ItemsByWeight { get; set; }

        public int Capacity { get; set; }
        public int N_Items { get; set; }

        public int[] Taken { get; set; }
        public int Value { get; set; }

        public int MaxValue { get; set; }
        public int MaxWeight { get; set; }

        public Solution(string file_name)
        {
            using (StreamReader sr = new StreamReader(file_name))
            {
                string line = sr.ReadLine();
                string[] tokens = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                N_Items = Convert.ToInt32(tokens[0]);
                Capacity = Convert.ToInt32(tokens[1]);

                Items = new List<Item>();

                for (int i = 0; i < N_Items; i++)
                {
                    line = sr.ReadLine();
                    tokens = line.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

                    int value = Convert.ToInt32(tokens[0]);
                    int weight = Convert.ToInt32(tokens[1]);
                    Items.Add(new Item(i, value, weight));
                }

                Taken = new int[N_Items];
                Array.Clear(Taken, 0, Taken.Length);
            }

            ItemsByRatioDesc = Items.OrderByDescending(i => i.Ratio).ToList<Item>();
            ItemsByWeight = Items.OrderBy(i => i.Weight).ToList<Item>();
            // var ItemsWeight2000 = Items.Where(i => i.Weight < 10000).OrderBy(i => i.Weight).ToList<Item>();
        }

        public int[] Solve(List<Item> items, int weightBound, int valueBound)
        {
            /*
            Solve ilk defa çalıştığında 
            |  weightBound = Capacity  |  valueBound = 0  |  
            olacak
            */

            int[][] mat = new int[2][];
            mat[0] = new int[weightBound + 1];   // tek satirlar icin | odd_lines
            mat[1] = new int[weightBound + 1];   // cift satirlar icin | even_lines

            Array.Clear(mat[0], 0, mat[0].Length);
            Array.Clear(mat[1], 0, mat[1].Length);

            Item lastSelectedItem = new Item(0, 0, 0);

            int maxValue = 0;
            int maxWeight = 0;

            int i = 0;
            while (i < N_Items)
            {
                int j = 0;
                int selectedProfit = 0;

                int weight = items[i].Weight;
                int value = items[i].Value;

                if (i % 2 != 0)
                {
                    while (++j <= weightBound)
                    {
                        if (weight <= j)
                        {
                            selectedProfit = value + mat[0][j - weight];

                            if (selectedProfit > maxValue)
                            {
                                maxValue = selectedProfit;
                                maxWeight = j;
                                lastSelectedItem = items[i];
                            }

                            if (weight <= j)
                                mat[1][j] = Math.Max(selectedProfit, mat[0][j]);
                            else
                                mat[1][j] = mat[0][j];
                        }
                        else
                            mat[1][j] = mat[0][j];
                    }
                }

                else
                {
                    while (++j <= weightBound)
                    {
                        if (weight <= j)
                        {
                            selectedProfit = value + mat[1][j - weight];
                            if (selectedProfit > maxValue)
                            {
                                maxValue = selectedProfit;
                                maxWeight = j;
                                lastSelectedItem = items[i];
                            }

                            if (weight <= j)
                                mat[0][j] = Math.Max(selectedProfit, mat[1][j]);
                            else
                                mat[0][j] = mat[1][j];
                        }
                        else
                            mat[0][j] = mat[1][j];
                    }
                }

                if (maxValue == valueBound)
                {
                    // Console.WriteLine(String.Format("V-{0}, W-{1}, VB-{2}, WB-{3}", lastSelectedItem.Value, lastSelectedItem.Weight, valueBound - lastSelectedItem.Value, weightBound - lastSelectedItem.Weight));
                    Taken[lastSelectedItem.Index] = 1;
                    return new int[] { weightBound - lastSelectedItem.Weight, valueBound - lastSelectedItem.Value };
                    // Bu fonksiyon her çalıştığında daha küçük bir kapasite (weightBound) ve daha küçük bir profit (valueBound) değeri güncellenerek program hızlandırılır.
                }

                i++;
            }

            // Solve yalnızca ilk kez çalıştırıldığında program bu bölüme gelecek.
            weightBound = maxWeight - lastSelectedItem.Weight;
            valueBound = maxValue - lastSelectedItem.Value;

            MaxWeight = maxWeight;
            MaxValue = maxValue;

            // Console.WriteLine(String.Format("V-{0}, W-{1}, VB-{2}, WB-{3}", lastSelectedItem.Value, lastSelectedItem.Weight, valueBound, weightBound));
            Taken[lastSelectedItem.Index] = 1;
            return new int[] { weightBound , valueBound };
        }
        
        public void SolveProblem()
        {
            int weightBound = Capacity;
            int valueBound = 0;

            do
            {
                int[] knapsackReturn = Solve(ItemsByWeight, weightBound, valueBound);
                weightBound = knapsackReturn[0];
                valueBound = knapsackReturn[1];
            }
            while (valueBound > 0);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(MaxValue.ToString());
            builder.AppendLine(String.Join(" ", Taken));
            return builder.ToString();
        }
    }

    class Solver
    {
        public static readonly Random random = new Random();

        static void Main(string[] args)
        {
            string file_name;

            if (args.Length != 1)
            {
                Console.WriteLine("Usage: CSharp Solver input_file\n");

                var rand = new Random();
                var files = Directory.GetFiles("data");
                file_name = files[rand.Next(files.Length)];

                // Environment.Exit(0);
            }

            else
            {
                file_name = args[0];
            }

            Solution solution = new Solution(file_name);
            solution.SolveProblem();
            Console.Write(solution);
            Console.ReadKey();
        }
    }
}
