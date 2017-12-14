/*
MIT License

Copyright(c) 2017 Hogia Communications AB

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

namespace Example.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;

    [TestClass]
    public class TestExamples
    {
        [TestMethod]
        public void Example1()
        {
            var data = VehicleInterface.GetBytes(
                new DateTimeOffset(2017, 12, 13, 8, 15, 34, 494, TimeSpan.Zero),
                57.092230F,
                14.24075F,
                20.0F,
                270.0F,
                VehicleInterface.TypeOfFix.Fix,
                VehicleInterface.FixQuality.Within2Meters,
                "123.buses",
                "4567.drivers",
                "67.89.lines",
                "operator.co.uk",
                0x0807060504030201,
                1,
                VehicleInterface.Signal.Off,
                VehicleInterface.Signal.Undefined,
                VehicleInterface.Signal.Undefined,
                VehicleInterface.Signal.Undefined
            );
            const string expected = "027F010203040506070801005EB6C501725E64421DDA6341D0077869210200000000093132332E62757365730C343536372E647269766572730B36372E38392E6C696E65730E6F70657261746F722E636F2E756B";
            var actual = BitConverter.ToString(data).Replace("-", "");
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void Example2()
        {
            var data = VehicleInterface.GetBytes(
                new DateTimeOffset(2017, 12, 13, 12, 14, 56, 404, TimeSpan.Zero),
                57.092230F,
                14.24075F,
                20.0F,
                270.0F,
                VehicleInterface.TypeOfFix.Fix,
                VehicleInterface.FixQuality.Within2Meters,
                "9031001001200045",
                "9051001001204567",
                "9015001008900067",
                "1",
                0x0807060504030201,
                2,
                VehicleInterface.Signal.Off,
                VehicleInterface.Signal.Undefined,
                VehicleInterface.Signal.Undefined,
                VehicleInterface.Signal.Undefined
            );
            const string expected = "027F0102030405060708020094DBA002725E64421DDA6341D00778692102000000001039303331303031303031323030303435103930353130303130303132303435363710393031353030313030383930303036370131";
            var actual = BitConverter.ToString(data).Replace("-", "");
            Assert.AreEqual(expected, actual);
        }

    }
}
