using System;
using System.Linq;
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

        private static Nock.Nock.noun N(params int[] values)
        {
            return N(values.Select(N).ToArray());
        }

        private static Nock.Nock.noun N(params Nock.Nock.noun[] values)
        {
            if (values.Length == 2)
                return N(values[0], values[1]);
            else
                return N(values[0], N(values.Skip(1).ToArray()));
        }

        private static Nock.Nock.noun N(params object[] values)
        {
            return N(values.Select(v =>
            {
                var noun = v as Nn;
                if (noun != null)
                    return noun;
                else if (v is int)
                    return N((int)v);
                else
                    throw new ArgumentException("Invalid noun type");
            }).ToArray());
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

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FasThrowsOnInvalid()
        {
            Nock.Nock.fas(N(N(1, 2), N(3, 4)));
        }
        #endregion

        #region Tar test
        [TestMethod]
        public void TarIncrement()
        {
            var result = Nock.Nock.tar(N(42, 4, 0, 1));

            Assert.IsInstanceOfType(result, typeof(Nn.Atom));
            Assert.AreEqual(43, ((Nn.Atom)result).Item);
        }

        [TestMethod]
        public void TarEqualityDetectsEquality()
        {
            var result = Nock.Nock.tar(N(N(1, 1), N(5, 0, 1)));

            Assert.IsInstanceOfType(result, typeof(Nn.Atom));
            Assert.AreEqual(0, ((Nn.Atom)result).Item);
        }

        [TestMethod]
        public void TarEqualityDetectsInEquality()
        {
            var result = Nock.Nock.tar(N(N(1, 2), N(5, 0, 1)));

            Assert.IsInstanceOfType(result, typeof(Nn.Atom));
            Assert.AreEqual(1, ((Nn.Atom)result).Item);
        }

        [TestMethod]
        public void TarWutDetectsAtom()
        {
            // let a = 1
            // let b = [0 1]
            //
            // Given that:      *[a 3 b]  =>  ?*[a b]
            // Substitution:    *[1 3 [0 1]]
            // Produces to:     ?*[1 0 1]
            //
            // Given that:      *[a 0 b]  =>  /[b a]
            // Substitution:    *[1 0 1]
            // Produces to:     /[1 1]
            //
            // Given that:      /[1 a]  =>  a
            // Substitution:    /[1 1]
            // Produces to:     1
            //
            // Given that:      ?a  =>  1
            // Substitution:    ?1
            // Produces to:     1

            var result = Nock.Nock.tar(N(1, 3, 0, 1));

            Assert.IsInstanceOfType(result, typeof(Nn.Atom));
            Assert.AreEqual(N(1), result);
        }

        [TestMethod]
        public void TarWutDetectsCell()
        {
            var result = Nock.Nock.tar(N(N(1, 2), N(3, 0, 1)));

            Assert.IsInstanceOfType(result, typeof(Nn.Atom));
            Assert.AreEqual(0, ((Nn.Atom)result).Item);
        }

        [TestMethod]
        public void TarFas()
        {
            // Test from https://github.com/cgyarvin/urbit/blob/master/doc/book/1-nock.markdown
            // Section marked "Line 18:"
            //[[4 5] [6 14 15]] [0 7]   =>  [14 15]
            var n = N(N(N(4, 5), N(6, 14, 15)), N(0, 7));
            var result = Nock.Nock.tar(n);
            Assert.AreEqual(N(14, 15), result);
        }

        [TestMethod]
        public void TarDualFormulaApplication()
        {
            // a = 1
            // b = 0
            // c = 1
            // d = [0 1]
            // *[a [b c] d]     [*[a b c] *[a d]]
            // result = [1 1]
            // This makes *[a b c] be *[1 0 1] which evaluates to 1
            // and this makes [a d] be *[1 [0 1]] which is equivalent to *[1 0 1] and thus also evaluates to 1

            var result = Nock.Nock.tar(N(1, N(0, 1), N(0, 1)));

            Assert.IsInstanceOfType(result, typeof(Nn.Cell));
            Assert.AreEqual(N(1, 1), result);
        }

        [TestMethod]
        public void TarRecursion()
        {
            // Test from https://github.com/cgyarvin/urbit/blob/master/doc/book/1-nock.markdown
            // Section marked "Line 20:"
            // (77 [2 [1 42] [1 1 153 218]])    =>  [153 218]
            var n = N(77, N(2, N(1, 42), N(1, 1, 153, 218)));
            var result = Nock.Nock.tar(n);
            Assert.AreEqual(N(153, 218), result);
        }
        #endregion

        [TestMethod]
        public void Playground()
        {
            //[[4 5] [6 14 15]] [0 7]   =>  [14 15]
            var n = N(N(N(4, 5), N(6, 14, 15)), N(0, 7));
            var result = Nock.Nock.tar(n);
            Assert.AreEqual(N(14, 15), result);
        }

        [TestMethod]
        public void BasicNockTest1()
        {
            
            
            var result = Nock.Nock.tar(N(N(19, 42), N(0, 3)));

            Assert.IsInstanceOfType(result, typeof(Nn.Atom));
            Assert.AreEqual(N(42), result);
        }

        [TestMethod]
        public void BasicNockTest2()
        {
            var result = Nock.Nock.tar(N(N(132, 19), N(10, 37, 4, 0, 3)));

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
            var result = Nock.Nock.tar(N(41, 1, 153, 218));

            Assert.IsInstanceOfType(result, typeof(Nn.Cell));
            Assert.AreEqual(N(153, 218), result);
        }

        [TestMethod]
        public void Decrement()
        {
            var n = N(42, N(8, N(1, 0), 8, N(1, 6, N(5, N(0, 7), 4, 0, 6), N(0, 6), 9, 2, N(0, 2), N(4, 0, 6), 0, 7), 9, 2, 0, 1));
            var result = Nock.Nock.tar(n);

            Assert.AreEqual(N(41), result);
        }
    }
}
