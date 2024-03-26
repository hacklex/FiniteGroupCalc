using System.Runtime.CompilerServices;
using FiniteGroupCalc.UlongProcessors;

namespace FiniteGroupCalc
{
    public class DistanceStatistics
    {
        private MainWindow _mainWindow;
        public DistanceStatistics(MainWindow mainWindow) => _mainWindow = mainWindow;

        public int[] GetDistanceHistogram(sbyte[] distances)
        {
            var histogram = new int[255];
            for (int i = 0; i < distances.Length; i++)
            {
                if (distances[i] != -1)
                {
                    histogram[distances[i]]++;
                }
            }
            return histogram.TakeWhile(x => x > 0).ToArray();
        }
        public Dictionary<sbyte, int> GetHistogram(sbyte[] data)
        {
            var histogram = new Dictionary<sbyte, int>();
            for (int i = 0; i < data.Length; i++)
            {
                if (histogram.ContainsKey(data[i]))
                {
                    histogram[data[i]]++;
                }
                else
                {
                    histogram[data[i]] = 1;
                }
            }
            return histogram;
        }


        public sbyte[] GetPartialDistanceTableForBasis(UlongProcessorBase processor, ulong initialState, ulong[] basis,
            Action<int, int> progressReport)
        {
            var ulongComparer = processor.GetUlongComparer();
            SortedList<ulong, ulong> keysOfPerms = new SortedList<ulong, ulong>(ulongComparer);
            var cur = processor.Identity;
            keysOfPerms.Add(cur, 0);
            for (ulong i = 1; i < processor.UlongCount; i++)
            {
                cur = processor.GetNext(cur);
                keysOfPerms.Add(cur, i);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ulong Times(ulong a, ulong b) => processor.Product(a, b);

            sbyte[] distances = new sbyte[processor.UlongCount];
            Array.Fill(distances, (sbyte)-1);
            bool stateIsKey = processor.StateIsKey;
            ulong keyOfState = stateIsKey ? initialState : (ulong)keysOfPerms.IndexOfKey(initialState);
            distances[keyOfState] = 0;

            sbyte currentJumpCount = 0;
            HashSet<ulong> accessibleStates = [initialState];
            bool changed;
            int totalFound = 1;
            int oldTotalFound = 1;
            do
            {
                var stateArray = accessibleStates.ToArray();
                changed = false;
                HashSet<ulong> BlockAction(int blockStart, int blockSize, int currentJumpCount)
                {
                    HashSet<ulong> newStates = new();
                    for (int i = blockStart; i < blockSize + blockStart; i++)
                    {
                        var cur = stateArray[i];
                        var curIndex = stateIsKey ? cur : (ulong)keysOfPerms.IndexOfKey(cur);

                        if (distances[curIndex] == currentJumpCount)
                        {
                            for (int j = 0; j < basis.Length; j++)
                            {
                                var next = Times(cur, basis[j]);
                                var nextIndex = stateIsKey ? next : (ulong)keysOfPerms.IndexOfKey(next);
                                if (distances[nextIndex] == -1)
                                {
                                    newStates.Add(next);
                                    distances[nextIndex] = (sbyte)(currentJumpCount + 1);
                                    changed = true;
                                }
                            }
                        }
                    }
                    return newStates;
                }
                int blockSize = 256 * 1024;
                int total = stateArray.Length / blockSize;
                int totalWithRemainder = total + (stateArray.Length % blockSize > 0 ? 1 : 0);
                HashSet<ulong>[] news = new HashSet<ulong>[totalWithRemainder];
                Parallel.For(0, total, i =>
                {
                    news[i] = BlockAction(i * blockSize, blockSize, currentJumpCount);
                });
                if (stateArray.Length % blockSize > 0)
                {
                    news[total] = BlockAction((stateArray.Length / blockSize) * blockSize, stateArray.Length % blockSize, currentJumpCount);
                }
                foreach (var set in news)
                {
                    accessibleStates.UnionWith(set);
                }
                if (totalFound != oldTotalFound)
                {
                    _mainWindow.BeginInvoke(() => progressReport?.Invoke(currentJumpCount, 100));
                    oldTotalFound = totalFound;
                }
                currentJumpCount++;
            } while (changed);
            return distances;
        }

        public sbyte[] GetDistanceTable(UlongProcessorBase processor, ulong initialState,
            Action<int, int> progressReport)
        {
            var ulongComparer = processor.GetUlongComparer();
            SortedList<ulong, ulong> keysOfPerms = new SortedList<ulong, ulong>(ulongComparer);
            var cur = processor.Identity;
            keysOfPerms.Add(cur, 0);
            for (ulong i = 1; i < processor.UlongCount; i++)
            {
                cur = processor.GetNext(cur);
                keysOfPerms.Add(cur, i);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            ulong Times(ulong a, ulong b) => processor.Product(a, b);
            ulong[] basis = processor.GetStandardBasis();
            var totalUlongs = processor.UlongCount;
            sbyte[] distances = new sbyte[totalUlongs];
            Array.Fill(distances, (sbyte)-1);
            bool stateIsKey = processor.StateIsKey;
            ulong keyOfInitialState = stateIsKey ? initialState : (ulong)keysOfPerms.IndexOfKey(initialState);
            distances[keyOfInitialState] = 0;
            sbyte currentJumpCount = 0;
            bool changed;
            int totalFound = 1;
            int oldTotalFound = 1;
            do
            {
                changed = false;
                void BlockAction(int blockStart, int blockSize, int currentJumpCount)
                {
                    var cur = processor.GetIth(blockStart);
                    for (var curId = blockStart; curId - blockStart < blockSize; curId++)
                    {
                        var curIndex = curId;                        
                        if (distances[curIndex] == currentJumpCount)
                        {
                            for (int i = 0; i < basis.Length; i++)
                            {
                                var next = Times((ulong)cur, basis[i]);
                                var nextIndex = stateIsKey ? next : (ulong)keysOfPerms.IndexOfKey(next);
                                if (distances[nextIndex] == -1)
                                {
                                    Interlocked.Increment(ref totalFound);
                                    distances[nextIndex] = (sbyte)(currentJumpCount + 1);
                                    changed = true;
                                }
                            }
                        }
                        cur = processor.GetNext(cur);
                    }
                }
                int blockSize = 256 * 1024;
                int total = distances.Length / blockSize;
                Parallel.For(0, total, i =>
                {
                    BlockAction(i * blockSize, blockSize, currentJumpCount);
                });
                if (distances.Length % blockSize > 0)
                {
                    BlockAction((distances.Length / blockSize) * blockSize, distances.Length % blockSize, currentJumpCount);
                }
                if (totalFound != oldTotalFound)
                {
                    _mainWindow.BeginInvoke(() => progressReport?.Invoke(totalFound, distances.Length));
                    oldTotalFound = totalFound;
                }
                currentJumpCount++;
            } while (changed);
            return distances;
        }


    }
}
