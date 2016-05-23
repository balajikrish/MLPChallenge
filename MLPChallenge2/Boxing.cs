// This test should take around 45 minutes to 1 hour.

//TODO: 
// 1. Refactor code below to make it testable. You can put all classes in one file.
// 2. Find "Boxed Positions" from the input file, "test_data.csv". Save the output to a file called "boxed_position.csv". 
// 3. Write unit tests for "boxed positions" and "net positions".

/*
 * Boxed Position:
 * A trader has long (quantity > 0) and short (quantity < 0) positions for a same symbol at different brokers.
 * 
 * This is an example of a boxed position.
 * TRADER   BROKER  SYMBOL  QUANTITY    PRICE
 * Joe      ML      IBM.N     100         50      <------Has at least one positive quantity for Trader = Joe and Symbol = IBM
 * Joe      DB      IBM.N    -50          50      <------Has at least one negative quantity for Trader = Joe and Symbol = IBM
 * Joe      CS      IBM.N     30          30
 * 
 * This is NOT a boxed position. Since no trader has both long and short positions at different brokers.
 * TRADER   BROKER  SYMBOL  QUANTITY    PRICE
 * Joe      ML      IBM.N     100         50      
 * Joe      DB      IBM.N     50          50      
 * Joe      CS      IBM.N     30          30
 * Mike     DB      IBM.N    -50          50     
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MLPChallenge2.NetPositions
{
    internal static class Constants
    {
        public static readonly string INPUT_FILE = @"test_data.csv";
        public static readonly string NETTED_OUTPUTFILE = @"netted_positions.csv";
        public static readonly string BOXED_OUTPUTFILE = @"boxed_positions.csv";
    }

    internal class Position
    {
        public string Trader { get; set; }
        public string Broker { get; set; }
        public string Symbol { get; set; }

        public int Quantity
        {
            get
            {
                return _quantity;
            }

            set
            {
                _quantity = value;
            }
        }

        private int _quantity;


        private decimal _price;

        public bool SetPrice(string price)
        {
            return decimal.TryParse(price, out _price);
        }

        public decimal GetPrice()
        {
            return _price;
        }

    }

    internal static class PositionFileParser
    {
        internal static List<Position> ReadPositionsFile(string inputFile)
        {
            var positions = new List<Position>();
            string[] lines = File.ReadAllLines(inputFile);
            for (int i = 1; i < lines.Length; i++)
            {
                string[] line = lines[i].Split(',');
                positions.Add(ProcessPositionLine(line));
            }
            return positions;
        }

        private static Position ProcessPositionLine(string[] line)
        {
            var position = new Position
            {
                Trader = line[0],
                Broker = line[1],
                Symbol = line[2],
            };

            int quantity = 0;
            if (int.TryParse(line[3], out quantity))
                position.Quantity = quantity;

            position.SetPrice(line[4]);
            return position;
        }

        internal static void WriteNettedPositionFile(List<Position> positions)
        {
            string[] outlines = new string[positions.Count + 1];
            // write the header.
            int lineindex = 0;
            outlines[lineindex] = "TRADER,SYMBOL,QUANTITY";

            foreach (var position in positions)
            {
                // write each line at offset of + 1.
                string outline = position.Trader + ',' + position.Symbol + ',' +
                                 position.Quantity;
                outlines[++lineindex] = outline;
            }
            File.WriteAllLines(Constants.NETTED_OUTPUTFILE, outlines);
        }

        internal static void WriteBoxedPositionFile(List<Tuple<Position, Position>> boxed)
        {
            string[] outlines = new string[boxed.Count * 2 + 1];
            // write the header.
            int lineindex = 0;
            outlines[lineindex] = "TRADER,SYMBOL,BROKER,QUANTITY,BOX_KEY";
            int boxKey = 0;
            foreach (var boxedPos in boxed)
            {
                boxKey++;
                string outline = String.Format("{0},{1},{2},{3},Boxed-{4}", boxedPos.Item1.Trader, boxedPos.Item1.Symbol, boxedPos.Item1.Broker, boxedPos.Item1.Quantity, boxKey);
                outlines[++lineindex] = outline;
                outline = String.Format("{0},{1},{2},{3},Boxed-{4}", boxedPos.Item2.Trader, boxedPos.Item2.Symbol, boxedPos.Item2.Broker, boxedPos.Item2.Quantity, boxKey);
                outlines[++lineindex] = outline;
            }
            File.WriteAllLines(Constants.BOXED_OUTPUTFILE, outlines);
        }
    }

    internal class PositionsCalculator
    {
        private class BoxKey
        {
            public string Trader { get; set; }
            public string Symbol { get; set; }
        }

        public List<Position> GetNettedPositionsByTrader(List<Position> positions)
        {
            var net = from pos in positions
                      group pos by new { pos.Trader, pos.Symbol } into netted
                      select new Position { Trader = netted.First().Trader, Symbol = netted.First().Symbol, Quantity = netted.Sum(_ => _.Quantity) };
            return net.ToList();
        }

        public List<Tuple<Position, Position>> GetBoxedPositions(List<Position> positions)
        {

            var res = from pos1 in positions
                      join pos2 in positions
                          on new { pos1.Trader, pos1.Symbol } equals new { pos2.Trader, pos2.Symbol }
                      where ((pos1.Quantity > 0 && pos2.Quantity < 0)) && pos1.Broker != pos2.Broker
                      select Tuple.Create(pos1, pos2);

            return res.ToList();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //in  header = "TRADER,BROKER,SYMBOL,QUANTITY,PRICE"
            //out header = "TRADER,SYMBOL,QUANTITY"

            var inputPositions = PositionFileParser.ReadPositionsFile(Constants.INPUT_FILE);
            var posCalculator = new PositionsCalculator();
            var netted = posCalculator.GetNettedPositionsByTrader(inputPositions);
            PositionFileParser.WriteNettedPositionFile(netted);

            var boxed = posCalculator.GetBoxedPositions(inputPositions);
            PositionFileParser.WriteBoxedPositionFile(boxed);
        }
    }
}
