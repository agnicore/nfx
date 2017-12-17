/*<FILE_LICENSE>
* NFX (.NET Framework Extension) Unistack Library
* Copyright 2003-2017 ITAdapter Corp. Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
</FILE_LICENSE>*/


using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using NFX.Scripting;
using NFX.Instrumentation.Analytics;

namespace NFX.UTest.Instrumentation.Analytics
{
    [Runnable]
    public class HistogramTests
    {
        [Run]
        public void Histogram1D()
        {
            const string EXPECTED_1D_PRINTOUT =
                "1D Histogram\n" +
                "|ValBucket| Count|     %|Total%|\n" +
                "|---------|------|------|------|\n" +
                "|   [0..3]|     2| 66.67| 66.67|\n" +
                "|   [4..7]|     1| 33.33|100.00|\n";

            var hist = new Histogram<int>("1D Histogram",
                new Dimension<int>(
                    "ValBucket",
                    partCount: 3,
                    partitionFunc: (x, v) => {
                        return v > 7 ? 2 : (v > 3 ? 1 : 0);
                    },
                    partitionNameFunc: (i) =>
                        i == 0 ? "[0..3]" : (i == 1 ? "[4..7]" : "[8..9]")
                )
            );

            hist.Sample(1);
            hist.Sample(2);
            hist.Sample(7);

            string output = hist.ToStringReport();

            Aver.AreEqual(EXPECTED_1D_PRINTOUT, output);

            Debug.Write(output);

            int count;

            Aver.Throws<KeyNotFoundException>(delegate { var x = hist[2]; });

            Aver.AreEqual(2, hist.Value(2));
            Aver.AreEqual(1, hist.Value(7));
            Aver.AreEqual(0, hist.Value(8));
            Aver.AreEqual(2, hist[0]);
            Aver.AreEqual(1, hist[1]);
            Aver.AreEqual(true,  hist.TryGet(0, out count)); Aver.AreEqual(2, count);
            Aver.AreEqual(true,  hist.TryGet(1, out count)); Aver.AreEqual(1, count);
            Aver.AreEqual(false, hist.TryGet(2, out count));
        }

        [Run]
        public void Histogram2D()
        {
            const string EXPECTED_2D_PRINTOUT =
                "2D Histogram\n" +
                "|ValBucket|    StdDev| Count|     %|Total%|\n" +
                "|---------|----------|------|------|------|\n" +
                "|   [0..3]|1.0 .. 2.0|     1| 20.00| 20.00|\n" +
                "|   [0..3]|    >  2.0|     1| 20.00| 40.00|\n" +
                "|   [4..7]|    >  2.0|     1| 20.00| 60.00|\n" +
                "|   [8..9]|    >  2.0|     2| 40.00|100.00|\n";

            var hist = new Histogram<int, double>("2D Histogram",
                new Dimension<int>(
                    "ValBucket",
                    partCount: 3,
                    partitionFunc: (x, v) => {
                        return v > 7 ? 2 : (v > 3 ? 1 : 0);
                    },
                    partitionNameFunc: (i) =>
                        i == 0 ? "[0..3]" : (i == 1 ? "[4..7]" : "[8..9]")
                ),
                new Dimension<double>(
                    "StdDev",
                    partCount: 3,
                    partitionFunc: (x, v) => v > 2.0 ? 2 : (v >= 1.0 ? 1 : 0),
                    partitionNameFunc: (i) =>
                        i == 0 ? "<  1.0" : (i == 1 ? "1.0 .. 2.0" : ">  2.0")
                )
            );
                
            hist.Sample(2, 1.0);
            hist.Sample(2, 5.0);
            hist.Sample(6, 2.5);
            hist.Sample(8, 2.8);
            hist.Sample(8, 3.4);

            string output = hist.ToStringReport();

            Aver.AreEqual(EXPECTED_2D_PRINTOUT, output);

            Debug.Write(output);

            Aver.AreObjectsEqual(new HistogramKeys(0, 1), hist.Keys(2, 1.0));
            Aver.AreObjectsEqual(new HistogramKeys(0, 2), hist.Keys(2, 5.0));
            Aver.AreObjectsEqual(new HistogramKeys(1, 2), hist.Keys(6, 2.5));
            Aver.AreObjectsEqual(new HistogramKeys(2, 2), hist.Keys(8, 2.8));
            Aver.AreObjectsEqual(new HistogramKeys(2, 2), hist.Keys(8, 3.4));
            Aver.AreObjectsEqual(new HistogramKeys(0, 0), hist.Keys(1, 0.9));
            Aver.AreObjectsEqual(new HistogramKeys(2, 0), hist.Keys(9, 0.5));

            Aver.AreEqual(1, hist.Value(2, 1.0));
            Aver.AreEqual(1, hist.Value(2, 5.0));
            Aver.AreEqual(1, hist.Value(6, 2.5));
            Aver.AreEqual(2, hist.Value(8, 2.8));
            Aver.AreEqual(2, hist.Value(8, 3.4));
            Aver.AreEqual(0, hist.Value(1, 0.9));
            Aver.AreEqual(0, hist.Value(9, 0.5));

            Aver.AreEqual(1, hist[0, 1]);
            Aver.AreEqual(1, hist[0, 2]);
            Aver.Throws<KeyNotFoundException>(delegate { var x = hist[0, 0]; });

            int count;

            Aver.AreEqual(false, hist.TryGet(0, 0, out count));
            Aver.AreEqual(true,  hist.TryGet(0, 1, out count)); Aver.AreEqual(1, count);
            Aver.AreEqual(true,  hist.TryGet(0, 2, out count)); Aver.AreEqual(1, count);
            Aver.AreEqual(false, hist.TryGet(1, 0, out count));
            Aver.AreEqual(false, hist.TryGet(1, 1, out count));
            Aver.AreEqual(true,  hist.TryGet(1, 2, out count)); Aver.AreEqual(1, count);
            Aver.AreEqual(false, hist.TryGet(2, 0, out count));
            Aver.AreEqual(false, hist.TryGet(2, 1, out count));
            Aver.AreEqual(true,  hist.TryGet(2, 2, out count)); Aver.AreEqual(2, count);
        }

        [Run]
        public void Histogram3D()
        {
            const string EXPECTED_3D_PRINTOUT =
                "3D Histogram\n" +
                "|ValBucket|    StdDev|   Sex| Count|     %|Total%|\n" +
                "|---------|----------|------|------|------|------|\n" +
                "|   [0..3]|1.0 .. 2.0|  Male|     1| 33.33| 33.33|\n" +
                "|   [0..3]|1.0 .. 2.0|Female|     1| 33.33| 66.67|\n" +
                "|   [0..3]|    >  2.0|  Male|     1| 33.33|100.00|\n";

            var hist = new Histogram<int, double, bool>("3D Histogram",
                new Dimension<int>(
                    "ValBucket",
                    partCount: 3,
                    partitionFunc: (x, v) =>
                    {
                        return v > 7 ? 2 : (v > 3 ? 1 : 0);
                    },
                    partitionNameFunc: (i) =>
                        i == 0 ? "[0..3]" : (i == 1 ? "[4..7]" : "[8..9]")
                ),
                new Dimension<double>(
                    "StdDev",
                    partCount: 3,
                    partitionFunc: (x, v) => v > 2.0 ? 2 : (v >= 1.0 ? 1 : 0),
                    partitionNameFunc: (i) =>
                        i == 0 ? "<  1.0" : (i == 1 ? "1.0 .. 2.0" : ">  2.0")
                ),
                new Dimension<bool>(
                    "Sex",
                    partCount: 2,
                    partitionFunc: (x, v)  => v ? 0 : 1,
                    partitionNameFunc: (i) => i == 0 ? "Male" : "Female"
                )
            );

            hist.Sample(2, 1.0, true);
            hist.Sample(2, 1.1, false);
            hist.Sample(2, 5.0, true);

            string output = hist.ToStringReport();

            Aver.AreEqual(EXPECTED_3D_PRINTOUT, output);

            Debug.Write(output);

            Aver.AreObjectsEqual(new HistogramKeys(0, 1, 0), hist.Keys(2, 1.0, true));
            Aver.AreObjectsEqual(new HistogramKeys(0, 1, 1), hist.Keys(2, 1.1, false));
            Aver.AreObjectsEqual(new HistogramKeys(0, 2, 0), hist.Keys(2, 5.0, true));
            Aver.AreObjectsEqual(new HistogramKeys(2, 2, 0), hist.Keys(8, 2.8, true));
            Aver.AreObjectsEqual(new HistogramKeys(2, 2, 1), hist.Keys(8, 3.4, false));
            Aver.AreObjectsEqual(new HistogramKeys(0, 0, 0), hist.Keys(1, 0.9, true));
            Aver.AreObjectsEqual(new HistogramKeys(2, 0, 1), hist.Keys(9, 0.5, false));

            Aver.AreEqual(1, hist.Value(2, 1.0, true));
            Aver.AreEqual(1, hist.Value(2, 1.1, false));
            Aver.AreEqual(1, hist.Value(2, 5.0, true));
            Aver.AreEqual(0, hist.Value(8, 2.8, true));
            Aver.AreEqual(0, hist.Value(8, 3.4, true));
            Aver.AreEqual(0, hist.Value(1, 0.9, false));
            Aver.AreEqual(0, hist.Value(9, 0.5, false));

            Aver.AreEqual(1, hist[0, 1, 0]);
            Aver.AreEqual(1, hist[0, 1, 1]);
            Aver.Throws<KeyNotFoundException>(delegate { var x = hist[0, 0, 0]; });

            int count;

            Aver.AreEqual(false, hist.TryGet(0, 0, 0, out count));
            Aver.AreEqual(true,  hist.TryGet(0, 2, 0, out count)); Aver.AreEqual(1, count);
            Aver.AreEqual(true,  hist.TryGet(0, 1, 0, out count)); Aver.AreEqual(1, count);
            Aver.AreEqual(true,  hist.TryGet(0, 1, 1, out count)); Aver.AreEqual(1, count);
            Aver.AreEqual(false, hist.TryGet(1, 0, 0, out count));
            Aver.AreEqual(false, hist.TryGet(1, 1, 1, out count));

        }
    }
}