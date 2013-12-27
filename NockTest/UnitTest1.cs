using System;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nn = Nock.Nock.noun;

namespace NockTest
{
    [TestClass]
    public class UnitTest1
    {
        #region noun factories
        private static Nock.Nock.noun N(int i)
        {
            return Nock.Nock.noun.NewAtom(i);
        }

        private static Nock.Nock.noun N(Nock.Nock.noun a, Nock.Nock.noun b)
        {
            return Nock.Nock.noun.NewCell(a, b);
        }

        private static Nock.Nock.noun N(int a, Nock.Nock.noun b)
        {
            return Nock.Nock.noun.NewCell(N(a), b);
        }

        private static Nock.Nock.noun N(Nock.Nock.noun a, int b)
        {
            return Nock.Nock.noun.NewCell(a, N(b));
        }

        private static Nock.Nock.noun N(int a, int b)
        {
            return Nock.Nock.noun.NewCell(N(a), N(b));
        }

        private static Nock.Nock.noun N(Nock.Nock.noun a, Nock.Nock.noun b, Nock.Nock.noun c)
        {
            return N(a, N(b, c));
        }

        private static Nock.Nock.noun N(Nock.Nock.noun a, Nock.Nock.noun b, int c)
        {
            return N(a, N(b, c));
        }

        private static Nock.Nock.noun N(Nock.Nock.noun a, int b, Nock.Nock.noun c)
        {
            return N(a, N(b, c));
        }

        private static Nock.Nock.noun N(Nock.Nock.noun a, int b, int c)
        {
            return N(a, N(b, c));
        }

        private static Nock.Nock.noun N(int a, Nock.Nock.noun b, Nock.Nock.noun c)
        {
            return N(a, N(b, c));
        }

        private static Nock.Nock.noun N(int a, Nock.Nock.noun b, int c)
        {
            return N(a, N(b, c));
        }

        private static Nock.Nock.noun N(int a, int b, Nock.Nock.noun c)
        {
            return N(a, N(b, c));
        }

        private static Nock.Nock.noun N(int a, int b, int c)
        {
            return N(a, N(b, c));
        }
        #endregion

        #region Wut test
        [TestMethod]
        public void WutIdentifiesCell()
        {
            var result = Nock.Nock.wut(N(1, 3));
            Assert.IsInstanceOfType(result, typeof(Nock.Nock.noun.Atom));
            Assert.AreEqual(N(0), result);
        }

        [TestMethod]
        public void WutIdentifiesAtom()
        {
            var result = Nock.Nock.wut(N(1));
            Assert.IsInstanceOfType(result, typeof(Nock.Nock.noun.Atom));
            Assert.AreEqual(N(1), result);
        }
        #endregion

        #region Lus test
        [TestMethod]
        public void LusIncrementsAtom()
        {
            var result = Nock.Nock.lus(N(1));
            Assert.IsInstanceOfType(result, typeof(Nock.Nock.noun.Atom));
            Assert.AreEqual(N(2), result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void LusThrowsOnCell()
        {
            Nock.Nock.lus(N(1, 2));
        }
        #endregion

        #region Tis test
        [TestMethod]
        public void TisIdentifiesEquality()
        {
            var result = Nock.Nock.tis(N(1, 1));
            Assert.IsInstanceOfType(result, typeof(Nock.Nock.noun.Atom));
            Assert.AreEqual(N(0), result);
        }

        [TestMethod]
        public void TisIdentifiesInEquality()
        {
            var result = Nock.Nock.tis(N(1, 2));
            Assert.IsInstanceOfType(result, typeof(Nock.Nock.noun.Atom));
            Assert.AreEqual(N(1), result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void TisThrowsOnAtom()
        {
            Nock.Nock.tis(N(1));
        }
        #endregion

        #region Fas test
        [TestMethod]
        public void FasSelectOne()
        {
            var result = Nock.Nock.fas(N(1, 2));
            Assert.IsInstanceOfType(result, typeof(Nock.Nock.noun.Atom));
            Assert.AreEqual(N(2), result);
        }

        [TestMethod]
        public void FasSelectTwo()
        {
            var result = Nock.Nock.fas(N(2, N(1, 2)));
            Assert.IsInstanceOfType(result, typeof(Nock.Nock.noun.Atom));
            Assert.AreEqual(N(1), result);
        }

        [TestMethod]
        public void FasSelectThree()
        {
            var result = Nock.Nock.fas(N(3, N(1, 2)));
            Assert.IsInstanceOfType(result, typeof(Nock.Nock.noun.Atom));
            Assert.AreEqual(N(2), result);
        }

        [TestMethod]
        public void FasSelectLeftTree()
        {
            var result = Nock.Nock.fas(N(6, N(97, 2), N(1, 42, 0)));
            Assert.IsInstanceOfType(result, typeof(Nock.Nock.noun.Atom));
            Assert.AreEqual(N(1), result);
        }

        [TestMethod]
        public void FasSelectRightTreeTree()
        {
            var result = Nock.Nock.fas(N(7, N(97, 2), N(1, 42, 0)));
            Assert.IsInstanceOfType(result, typeof(Nock.Nock.noun.Cell));
            Assert.AreEqual(N(42, 0), result);
        }
        #endregion

        #region Tar test

        #endregion

        [TestMethod]
        public void BasicNockTest1()
        {
            var result = Nock.Nock.tar(N(N(19, 42), N(0, 3)));

            Assert.IsInstanceOfType(result, typeof(Nn.Atom));
            Assert.AreEqual(42, ((Nn.Atom) result).Item);
        }

        [TestMethod]
        public void BasicNockTest2()
        {
            var result = Nock.Nock.tar(N(N(132, 19), N(10, N(37, N(4, N(0, 3))))));

            Assert.IsInstanceOfType(result, typeof(Nn.Atom));
            Assert.AreEqual(20, ((Nn.Atom) result).Item);
        }

        [TestMethod]
        public void BasicNockTest3()
        {
            var result = Nock.Nock.tar(N(N(N(4, 5), N(6, 14, 15)), 0, 7));

            Assert.IsInstanceOfType(result, typeof(Nn.Cell));
            Assert.AreEqual(N(14, 15), result);
        }

        [TestMethod]
        public void BasicNockTest4()
        {
            var result = Nock.Nock.tar(N(41, N(1, 153, 218)));

            Assert.IsInstanceOfType(result, typeof(Nn.Cell));
            Assert.AreEqual(N(153, 218), result);
        }
    }
}
