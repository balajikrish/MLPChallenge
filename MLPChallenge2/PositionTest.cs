using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MLPChallenge2.NetPositions;

namespace MLPChallenge2
{
    [TestClass]
    public class PositionTest
    {
        [TestMethod]
        public void TestNetPosition_TwoLongPositions()
        {
            List<Position> positions = new List<Position>
            {
                new Position { Trader ="AAA", Broker = "BCY", Symbol = "AAPL", Quantity = 100 },
                new Position { Trader ="AAA", Broker = "BCY", Symbol = "AAPL", Quantity = 100 },                
            };

            PositionsCalculator calc = new PositionsCalculator();
            var netted = calc.GetNettedPositionsByTrader(positions);

            foreach ( var pos in netted)
            {
                if (pos.Trader == "AAA")
                    Assert.AreEqual(200, pos.Quantity);
            }

        }

        [TestMethod]
        public void TestNetPosition_LongShortPositions()
        {
            List<Position> positions = new List<Position>
            {
                new Position { Trader ="AAA", Broker = "BCY", Symbol = "AAPL", Quantity = 100 },
                new Position { Trader ="AAA", Broker = "BCY", Symbol = "AAPL", Quantity = -200 },
            };

            PositionsCalculator calc = new PositionsCalculator();
            var netted = calc.GetNettedPositionsByTrader(positions);

            foreach (var pos in netted)
            {
                if (pos.Trader == "AAA")
                    Assert.AreEqual(-100, pos.Quantity);
            }

        }

        [TestMethod]
        public void TestNetPosition_TwoDistinctPositions()
        {
            List<Position> positions = new List<Position>
            {
                new Position { Trader ="AAA", Broker = "BCY", Symbol = "AAPL", Quantity = 100 },
                new Position { Trader ="AAA", Broker = "BCY", Symbol = "IBM", Quantity = -200 },
            };

            PositionsCalculator calc = new PositionsCalculator();
            var netted = calc.GetNettedPositionsByTrader(positions);

            foreach (var pos in netted)
            {
                if (pos.Symbol == "AAPL")
                    Assert.AreEqual(100, pos.Quantity);
                if (pos.Symbol == "IBM")
                    Assert.AreEqual(-200, pos.Quantity);
            }

        }

        [TestMethod]
        public void TestBoxedPositions_ValidBoxedPosition()
        {
            List<Position> positions = new List<Position>
            {
                new Position { Trader ="AAA", Broker = "BCY", Symbol = "AAPL", Quantity = 100 },
                new Position { Trader ="AAA", Broker = "DB", Symbol = "AAPL", Quantity = -50 },
            };

            PositionsCalculator calc = new PositionsCalculator();
            var boxed = calc.GetBoxedPositions(positions);

            Assert.AreEqual(1, boxed.Count);            
        }

        [TestMethod]
        public void TestBoxedPositions_NoBoxedPosition_SameBroker()
        {
            List<Position> positions = new List<Position>
            {
                new Position { Trader ="AAA", Broker = "BCY", Symbol = "AAPL", Quantity = -100 },
                new Position { Trader ="AAA", Broker = "BCY", Symbol = "AAPL", Quantity = -50 },
            };

            PositionsCalculator calc = new PositionsCalculator();
            var boxed = calc.GetBoxedPositions(positions);

            Assert.AreEqual(0, boxed.Count);
        }

        [TestMethod]
        public void TestBoxedPositions_NoBoxedPosition_BothLong()
        {
            List<Position> positions = new List<Position>
            {
                new Position { Trader ="AAA", Broker = "BCY", Symbol = "AAPL", Quantity = 100 },
                new Position { Trader ="AAA", Broker = "DB", Symbol = "AAPL", Quantity = 50 },
            };

            PositionsCalculator calc = new PositionsCalculator();
            var boxed = calc.GetBoxedPositions(positions);

            Assert.AreEqual(0, boxed.Count);
        }

        [TestMethod]
        public void TestBoxedPositions_NoBoxedPosition_BothShort()
        {
            List<Position> positions = new List<Position>
            {
                new Position { Trader ="AAA", Broker = "BCY", Symbol = "AAPL", Quantity = -100 },
                new Position { Trader ="AAA", Broker = "DB", Symbol = "AAPL", Quantity = -50 },
            };

            PositionsCalculator calc = new PositionsCalculator();
            var boxed = calc.GetBoxedPositions(positions);

            Assert.AreEqual(0, boxed.Count);
        }
    }
}
