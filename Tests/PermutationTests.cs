using FiniteGroupCalc.UlongProcessors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace FiniteGroupCalc.Tests
{
    [TestClass]
    public class PermutationTests
    {
        [TestMethod]
        public void InversionWorks()
        {
            var proc = new PermutationProcessor();
            proc.Order = 4;
            var all = Enumerable.Range(0, 24).Select(proc.GetIth).ToArray();
            for (int i = 0; i < 24; i++)
            {
                var inv = proc.InvertPerm(all[i]);
                Assert.AreEqual(proc.Identity, proc.Product(all[i], inv));
                Assert.AreEqual(proc.Identity, proc.Product(inv, all[i]));
            }
        }
        [TestMethod]
        public void PermutationIndexIsCorrect()
        {
            var testOrder = 10;
            var proc = new PermutationProcessor();
            proc.Order = testOrder;
            var fac = (int)PermutationProcessor.Factorial(testOrder);
            var all = Enumerable.Range(0, fac).Select(proc.GetIth).ToArray();
            for (int i = 0; i < fac; i++)
            {
                Assert.AreEqual((ulong)i, proc.PermIndex(all[i]));
            }
        }

        /// <summary>Ensures that applying the permutation to a ulong is the same thing as composing, if that ulong is a permutation itself</summary>
        [TestMethod]
        public void ApplyIsCompose()
        {
            var proc = new PermutationProcessor();
            proc.Order = 4;
            for (int i = 0; i < 24; i++)
            {
                var perm = proc.GetIth(i);
                for (int j = 0; j < 24; j++)
                {
                    var perm2 = proc.GetIth(j);
                    var composed = proc.ApplyPermutation(perm2, perm);
                    var product = proc.Product(perm, perm2);
                    Assert.AreEqual(composed, product);
                    ulong[] results = new ulong[24];
                    for (int k = 0; k < 24; k++)
                    {
                        var kth = proc.GetIth(k);
                        var result = results[k] = proc.ApplyPermutation(composed, kth);
                        Assert.AreEqual(result, proc.ApplyPermutation(perm2, proc.ApplyPermutation(perm, kth)));
                    }
                    Assert.IsTrue(results.Distinct().Count() == 24);
                }
            }
        }

        /// <summary>Ensures that the identity permutation of the permutation processor is correct</summary>
        [TestMethod]
        public void IdentitiesAreCorrect()
        {
            var proc = new PermutationProcessor();
            var fullIdentity = 0xfedcba9876543210ul;
            for (int i = 0; i < 13; i++)
            {
                proc.Order = i;
                Assert.AreEqual(fullIdentity & ((1ul << (i * 4)) - 1), proc.Identity);
            }
        }

        /// <summary>Reverses the hex digits of a ulong</summary>
        /// <param name="a">The ulong to reverse the hex digits of</param>
        /// <returns>The ulong composed from the hex digits of the input ulong, taken in reverse order</returns>
        public ulong ReverseHexDigits(ulong a)
        {
            ulong res = 0;
            for (int i = 0; i < 16; i++)
            {
                res |= PermutationProcessor.GetDigit(a, (ulong)i) << ((15 - i) * 4);
            }
            return res;
        }

        /// <summary>
        /// Ensures that the digit removal function works correctly for all digits (0-15)
        /// </summary>
        [TestMethod]
        public void RemoveDigitWorks()
        {
            string RemoveChar(string s, int i) => s.Substring(0, 15 - i) + s.Substring(16 - i);
            var fullIdentity = 0xfedcba9876543210ul;
            var stringId = "fedcba9876543210";
            Assert.AreEqual(0x0123456789abcdeful, ReverseHexDigits(fullIdentity));
            for (int i = 0; i < 16; i++)
            {
                var ul = PermutationProcessor.RemoveDigit(fullIdentity, (ulong)i);
                var str = RemoveChar(stringId, i);
                Assert.AreEqual(ul, Convert.ToUInt64(str, 16));
            }
        }

        /// <summary>
        /// Ensures that the permutations are lexicographically ordered
        /// </summary>
        [TestMethod]
        public void PermutationsAreLexicographicallyOrdered()
        {
            var proc = new PermutationProcessor();
            proc.Order = 4;
            var all = Enumerable.Range(0, 24).Select(proc.GetIth).ToArray();
            for (int i = 0; i < all.Length - 1; i++)
            {
                Assert.IsTrue(ReverseHexDigits(all[i]) < ReverseHexDigits(all[i + 1]));
            }
        }

        /// <summary>
        /// Ensures that the fast factorial function using a precomputed array is correct
        /// </summary>
        [TestMethod]
        public void FactorialsAreCorrect() => Assert.IsTrue(Enumerable.Range(0, 13).All(x => 
            PermutationProcessor.Factorial(x) == 
            (ulong)Enumerable.Range(1, x).Aggregate(1, (a, b) => a * b)));

        [TestMethod]
        public void GetIthValuesAreOrdered()
        {
            for (int i = 1; i < 120; i++)
            {
                var prev = PermutationProcessor.GetIthPermutationOfOrder((ulong)i - 1, 5);
                var cur = PermutationProcessor.GetIthPermutationOfOrder((ulong)i, 5);
                Assert.IsTrue(ReverseHexDigits(prev) < ReverseHexDigits(cur));                
            }
        }
        [TestMethod]
        public void PermutationsAreGeneratedCorrectly()
        {
            var proc = new PermutationProcessor();
            proc.Order = 4; 
            var count = proc.UlongCount;
            ulong[] GetPermsUsingNextPerm() 
            { 
                ulong cur = proc.Identity;
                List<ulong> res = [cur];
                for (ulong i = 1; i < proc.UlongCount; i++)
                {
                    cur = proc.NextPermutationUlong(cur);
                    res.Add(cur);
                }
                return res.ToArray();
            }
            ulong[] check = [0x3210, 0x2310, 0x3120, 0x1320, 0x2130, 0x1230, 
                             0x3201, 0x2301, 0x3021, 0x0321, 0x2031, 0x0231,
                             0x3102, 0x1302, 0x3012, 0x0312, 0x1032, 0x0132,
                             0x2103, 0x1203, 0x2013, 0x0213, 0x1023, 0x0123];

            var ithGen = Enumerable.Range(0, 24).Select(i => PermutationProcessor.GetIthPermutationOfOrder((ulong)i, 4)).ToArray();
            var nGen = GetPermsUsingNextPerm();
            Assert.IsTrue(check.SequenceEqual(ithGen));
            Assert.IsTrue(check.SequenceEqual(nGen));
        }

        /// <summary>
        /// Ensures we actually win from generating the next permutation
        /// from the previous one, rather than generating each permutation from its index
        /// </summary>
        [TestMethod]
        public void SequentialGenerationIsFaster()
        {
            ulong[] CreateEachFromScratch(int order)
            {
                int fac = (int)PermutationProcessor.Factorial(order); 
                var res = new ulong[fac];
                for (int i = 0; i < fac; i++)
                {
                    res[i] = PermutationProcessor.GetIthPermutationOfOrder((ulong)i, (ulong)order);
                }
                return res;
            }
            ulong[] CreateSequential(int order)
            {
                int fac = (int)PermutationProcessor.Factorial(order);
                var res = new ulong[fac];
                var proc = new PermutationProcessor { Order = order };
                var cur = proc.Identity;
                res[0] = cur;
                for (int i = 1; i < fac; i++)
                {
                    cur = proc.NextPermutationUlong(cur);
                    res[i] = cur;
                }
                return res;
            }

            var stopwatch = Stopwatch.StartNew();
            var sequentialArray = CreateSequential(10);
            stopwatch.Stop();
            var sequentialTime = stopwatch.ElapsedMilliseconds;
            stopwatch.Restart();
            var fromScratchArray = CreateEachFromScratch(10);
            stopwatch.Stop();
            var fromScratchTime = stopwatch.ElapsedMilliseconds;
            Assert.IsTrue(sequentialArray.SequenceEqual(fromScratchArray));
            Assert.IsTrue(sequentialTime < fromScratchTime);
        }
    }
}
