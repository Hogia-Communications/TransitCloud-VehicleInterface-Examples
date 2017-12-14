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

namespace Example
{
    using System;
    using System.IO;
    using System.Text;

    /// <summary>Example of how to create an UDP message to TransitCloud</summary>
    /// <remarks>
    /// Based on TransitCloud Vehicle Interface Specification 1.9, Hogia Standard/Extended Position Message.
    /// For details, please consult this document before implementing the protocol.
    /// All values should be written with Windows standards, that is little endian byte order, see https://en.wikipedia.org/wiki/Endianness
    /// </remarks>
    public static class VehicleInterface
    {

        public static byte[] GetBytes(DateTimeOffset timestamp, float latitude, float longitude, float speed, float heading, TypeOfFix fix, FixQuality quality, string vehicleRef, string driverRef, string taskRef, string accountRef, UInt64? unitId, UInt16? sequenceNumber, Signal powerOn, Signal doorReleased, Signal stopRequested, Signal inService)
        {
            var outputStream = new MemoryStream();
            using (var writer = new BinaryWriter(outputStream))
            {
                // Message type, always 2 for Hogia Extended Position Message.
                writer.Write((byte)0x02);
                // Message priority, default 127. Note that priority is not supported, its for future use.
                writer.Write((byte)127);
                // Device id that is globally unique, e.g. MAC adress. If not provided (=0), a a vehicleRef must be provided.
                writer.Write((UInt64)(unitId ?? 0UL));
                // Sequence number of message. Shall be reset to zero at device start. 
                writer.Write((UInt16)(sequenceNumber ?? 0));
                // Timestamp is in milliseconds from midnight
                writer.Write((UInt32)((timestamp.UtcDateTime - timestamp.UtcDateTime.Date).TotalMilliseconds));
                // Latitude and longitude is in IEEE single precision floating number. A 0,0 position is always regarded as invalid.
                writer.Write(latitude);
                writer.Write(longitude);
                // Speed is in meters per second times 100.
                writer.Write((UInt16)(speed * 100.0f));
                // Heading is in degrees 0 <= value < 360 times 100.
                writer.Write((UInt16)((heading * 100.0f) % 36000));
                // Fix type and quality is combined into one byte
                writer.Write((byte)(fix + (int)quality * 16));
                // The four signals is compined into one byte
                writer.Write((byte)((int)powerOn + (int)doorReleased * 4 + (int)stopRequested * 16 + (int)inService * 64));
                // Distance is a special feature that normally is not sent from vehicles, so write an zero value.
                writer.Write((UInt32)0);
                // The id of the vehicle, unique within a customers context, i.e. TransitCloud account. This field must be provided if unit id is not provided. It can also be provided when a unit id is provided.
                writer.WriteString(vehicleRef);
                // The current id of the driver. Optional.
                writer.WriteString(driverRef);
                // The task id that the vehicle is currently working. See the documentation for how to populate this field. If empty, it is assumed that the vehicle is not working any task. 
                writer.WriteString(taskRef);
                // Account id is normally not used. Optional.
                writer.WriteString(accountRef);
            }
            return outputStream.ToArray();
        }

        /// <summary>This is the correct way to write UNICODE string values.</summary>
        /// <param name="writer"></param>
        /// <param name="value">A UNICODE string containing only valid ASCII characters.</param>
        private static void WriteString(this BinaryWriter writer, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                writer.Write((byte)0);
            }
            else
            {
                var encoded = Encoding.ASCII.GetBytes(value);
                if (encoded.Length > 255) throw new ArgumentOutOfRangeException(nameof(value), "Strings must not be longer than 255 characters.");
                writer.Write((byte)encoded.Length);
                writer.Write(encoded);
            }
        }

        public enum TypeOfFix
        {
            InvalidFix = 0,
            Fix = 1,
            DifferentialFix = 2,
            PPSFix = 3,
            RealTimeKinematicFix = 4,
            FloatRealTimeKinematicFix = 5,
            EstimatedFix = 6,
            ManualFix = 7,
            SimulatedFix = 8,
            WiFi = 10,
            HandsetFingerprinting = 11,
            HandsetCellIdentification = 12,
            CellularNetworkForwardLink = 13,
            CellularNetworkTriangulation = 14,
            Other = 15
        }

        public enum FixQuality
        {
            Undefined = 0,
            Within1Meter = 1,
            Within2Meters = 2,
            Within5Meters = 3,
            Within10Meters = 4,
            Within20Meters = 5,
            Within50Meters = 6,
            Within100Meters = 7,
            Within200Meters = 8,
            Within500Meters = 9,
            Within1000Meters = 10,
            Within2000Meters = 11,
            Within5000Meters = 12,
            Over5000Meters = 13
        }

        public enum Signal
        {
            Undefined = 0b00,
            Error = 0b01,
            Off = 0b10,
            On = 0b11
        }
    }
}
