#region license

/************************************************************************

DirectShowLib - Provide access to DirectShow interfaces via .NET
Copyright (C) 2007
http://sourceforge.net/projects/directshownet/

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

**************************************************************************/

#endregion

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Reflection;
using System.Security;

using DirectShowLib.Dvd;

using JetBrains.Annotations;
#if !USING_NET11
using System.Runtime.InteropServices.ComTypes;
#endif

namespace DirectShowLib
{
    #region Declarations

    /// <summary>
    /// Not from DirectShow
    /// </summary>
    public enum PinConnectedStatus
    {
        /// <summary>
        /// Pin is not connected
        /// </summary>
        Unconnected,

        /// <summary>
        /// Pin is connected
        /// </summary>
        Connected
    }

    /// <summary>
    /// The structure defines the dimensions and color information for a DIB.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [PublicAPI]
    public struct BitmapInfo
    {
        /// <summary>
        /// The structure that contains information about the dimensions of color format.
        /// </summary>
        public BitmapInfoHeader bmiHeader;

        /// <summary>
        /// The bmiColors member contains one of the following:
        /// <list type="bullet">
        /// <item><description>An array of RGBQUAD. The elements of the array that make up the color table.</description></item>
        /// <item><description>An array of 16-bit unsigned integers that specifies indexes into the currently 
        /// realized logical palette. This use of bmiColors is allowed for functions that use DIBs. 
        /// When bmiColors elements contain indexes to a realized logical palette, they must also call the following bitmap functions:
        /// <list type="bullet">
        /// <item><description><b>CreateDIBitmap</b></description></item>
        /// <item><description><b>CreateDIBPatternBrush</b></description></item>
        /// <item><description><b>CreateDIBSection</b></description></item>
        /// </list>
        /// </description></item>
        /// </list>
        /// </summary>
        public int[] bmiColors;
    }

    /// <summary>
    /// The structure contains information about the dimensions and color format of a DIB.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    [PublicAPI]
    public class BitmapInfoHeader
    {
        /// <summary>
        /// The number of bytes required by the structure.
        /// </summary>
        public int Size;

        /// <summary>
        /// The width of the bitmap, in pixels.
        /// If <see cref="Compression"/> is <b>BI_JPEG</b> or <b>BI_PNG</b>, the <see cref="Width"/> member specifies the width of the 
        /// decompressed JPEG or PNG image file, respectively.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the bitmap, in pixels.If <see cref="Height"/> is positive, the bitmap is a bottom-up DIB and its origin is the 
        /// lower-left corner.If <see cref="Height"/> is negative, the bitmap is a top-down DIB and its origin is the upper-left corner.
        /// If <see cref="Height"/> is negative, indicating a top-down DIB, biCompression must be either <b>BI_RGB</b> or <b>BI_BITFIELDS</b>.
        /// Top-down DIBs cannot be compressed.
        /// If <see cref="Compression"/> is <b>BI_JPEG</b> or <b>BI_PNG</b>, the <see cref="Height"/> member specifies the height of the 
        /// decompressed JPEG or PNG image file, respectively.
        /// </summary>
        public int Height;

        /// <summary>
        /// The number of planes for the target device. This value must be set to 1.
        /// </summary>
        public short Planes;

        /// <summary>
        /// The number of bits-per-pixel. The <see cref="BitCount"/> member of the <see cref="BitmapInfoHeader"/> structure determines the 
        /// number of bits that define each pixel and the maximum number of colors in the bitmap. This member must be one of the following values.
        /// <list type="bullet">
        /// <item><description>0 - The number of bits-per-pixel is specified or is implied by the JPEG or PNG format.</description></item>
        /// <item><description>1 - 	The bitmap is monochrome, and the <see cref="BitmapInfo.bmiColors"/> member of BITMAPINFO contains two entries. 
        /// Each bit in the bitmap array represents a pixel. If the bit is clear, the pixel is displayed with the color of the first 
        /// entry in the bmiColors table; if the bit is set, the pixel has the color of the second entry in the table.</description></item>
        /// <item><description>4 - The bitmap has a maximum of 16 colors, and the <see cref="BitmapInfo.bmiColors"/> member of BITMAPINFO 
        /// contains up to 16 entries. Each pixel in the bitmap is represented by a 4-bit index into the color table. For example, if the 
        /// first byte in the bitmap is 0x1F, the byte represents two pixels. The first pixel contains the color in the second table entry, 
        /// and the second pixel contains the color in the sixteenth table entry.</description></item>
        /// <item><description>8 - The bitmap has a maximum of 256 colors, and the <see cref="BitmapInfo.bmiColors"/> member of BITMAPINFO contains up to 256 entries. 
        /// In this case, each byte in the array represents a single pixel.</description></item>
        /// </list>
        /// </summary>
        public short BitCount;

        /// <summary>
        /// The type of compression for a compressed bottom-up bitmap (top-down DIBs cannot be compressed). This member can be one of the following values.
        /// <list type="bullet">
        /// <item><description>BI_RGB - An uncompressed format.</description></item>
        /// <item><description>BI_RLE8 - A run-length encoded (RLE) format for bitmaps with 8 bpp. The compression format is a 2-byte format consisting of a count byte followed by a byte containing a color index.</description></item>
        /// <item><description>BI_RLE4 - An RLE format for bitmaps with 4 bpp. The compression format is a 2-byte format consisting of a count byte followed by two word-length color indexes.</description></item>
        /// <item><description>BI_BITFIELDS - Specifies that the bitmap is not compressed and that the color table consists of three DWORD color masks that specify the red, green, and blue components, respectively, of each pixel. This is valid when used with 16- and 32-bpp bitmaps.</description></item>
        /// <item><description>BI_JPEG - Indicates that the image is a JPEG image.</description></item>
        /// <item><description>BI_PNG - Indicates that the image is a PNG image.</description></item>
        /// </list>
        /// </summary>
        public int Compression;

        /// <summary>
        /// The size, in bytes, of the image. This may be set to zero for BI_RGB bitmaps.
        /// </summary>
        public int ImageSize;

        /// <summary>
        /// The horizontal resolution, in pixels-per-meter, of the target device for the bitmap. 
        /// An application can use this value to select a bitmap from a resource group that best matches the characteristics of the 
        /// current device.
        /// </summary>
        public int XPelsPerMeter;

        /// <summary>
        /// The vertical resolution, in pixels-per-meter, of the target device for the bitmap.
        /// </summary>
        public int YPelsPerMeter;

        /// <summary>
        /// The number of color indexes in the color table that are actually used by the bitmap. If this value is zero, the bitmap uses 
        /// the maximum number of colors corresponding to the value of the <see cref="BitCount"/> member for the compression mode specified 
        /// by <see cref="Compression"/>.
        /// If <see cref="ClrUsed"/> is nonzero and the biBitCount member is less than 16, the <see cref="ClrUsed"/> member specifies the 
        /// actual number of colors the graphics engine or device driver accesses. If <see cref="BitCount"/> is 16 or greater, the <see cref="ClrUsed"/> 
        /// member specifies the size of the color table used to optimize performance of the system color palettes.If <see cref="BitCount"/> equals 
        /// 16 or 32, the optimal color palette starts immediately following the three DWORD masks.
        /// When the bitmap array immediately follows the <see cref="BitmapInfo"/> structure, it is a packed bitmap.Packed bitmaps are referenced by a 
        /// single pointer. Packed bitmaps require that the <see cref="ClrUsed"/> member must be either zero or the actual size of the color table.
        /// </summary>
        public int ClrUsed;

        /// <summary>
        /// The number of color indexes that are required for displaying the bitmap. If this value is zero, all colors are required.
        /// </summary>
        public int ClrImportant;
    }

    /// <summary>
    /// The structure describes the pixel format of a DirectDrawSurface object. 
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    [PublicAPI]
    public struct DDPixelFormat
    {
        /// <summary>
        /// Specifies the size in bytes of the <see cref="DDPixelFormat"/> structure. The driver must initialize this member before the structure is used.
        /// </summary>
        [FieldOffset(0)]
        public int dwSize;

        /// <summary>
        /// Indicates a set of flags that specify optional control flags. This member is a bitwise OR of any of the following values:
        /// </summary>
        [FieldOffset(4)]
        public int dwFlags;

        /// <summary>
        /// Specifies a surface format code including any of the codes in the D3DFORMAT enumerated type. Some FOURCC codes are part of 
        /// D3DFORMAT. For more information about D3DFORMAT, see the SDK documentation. Hardware vendors can also define and supply format 
        /// codes that are specific to their hardware. 
        /// </summary>
        [FieldOffset(8)]
        public int dwFourCC;

        /// <summary>
        /// Specifies the number of RGB bits per pixel (4, 8, 16, 24, or 32). 
        /// </summary>
        [FieldOffset(12)]
        public int dwRGBBitCount;

        /// <summary>
        /// Specifies the number of YUV bits per pixel. 
        /// </summary>
        [FieldOffset(12)]
        public int dwYUVBitCount;

        /// <summary>
        /// Specifies the Z-buffer bit depth (8, 16, 24, or 32 bits). 
        /// </summary>
        [FieldOffset(12)]
        public int dwZBufferBitDepth;

        /// <summary>
        /// Specifies the Alpha channel bit depth. 
        /// </summary>
        [FieldOffset(12)]
        public int dwAlphaBitDepth;

        /// <summary>
        /// Specifies the mask for red bits.
        /// </summary>
        [FieldOffset(16)]
        public int dwRBitMask;

        /// <summary>
        /// Specifies the mask for Y bits.
        /// </summary>
        [FieldOffset(16)]
        public int dwYBitMask;

        /// <summary>
        /// Specifies the mask for green bits. 
        /// </summary>
        [FieldOffset(20)]
        public int dwGBitMask;

        /// <summary>
        /// Specifies the mask for U bits. 
        /// </summary>
        [FieldOffset(20)]
        public int dwUBitMask;

        /// <summary>
        /// Specifies the mask for blue bits. 
        /// </summary>
        [FieldOffset(24)]
        public int dwBBitMask;

        /// <summary>
        /// Specifies the mask for V bits. 
        /// </summary>
        [FieldOffset(24)]
        public int dwVBitMask;

        /// <summary>
        /// Specify the masks for alpha channel. 
        /// </summary>
        [FieldOffset(28)]
        public int dwRGBAlphaBitMask;

        /// <summary>
        /// Specify the masks for alpha channel. 
        /// </summary>
        [FieldOffset(28)]
        public int dwYUVAlphaBitMask;

        /// <summary>
        /// Specifies the masks for the z channel. 
        /// </summary>
        [FieldOffset(28)]
        public int dwRGBZBitMask;

        /// <summary>
        /// Specifies the masks for the z channel. 
        /// </summary>
        [FieldOffset(28)]
        public int dwYUVZBitMask;
    }

    /// <summary>
    /// From CAUUID
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [PublicAPI]
    public struct DsCAUUID
    {
        /// <summary>
        /// 
        /// </summary>
        public int cElems;

        /// <summary>
        /// 
        /// </summary>
        public IntPtr pElems;

        /// <summary>
        /// Perform a manual marshaling of pElems to retrieve an array of System.Guid.
        /// Assume this structure has been already filled by the ISpecifyPropertyPages.GetPages() method.
        /// </summary>
        /// <returns>A managed representation of pElems (cElems items)</returns>
        public Guid[] ToGuidArray()
        {
            Guid[] retval = new Guid[this.cElems];

            for (int i = 0; i < this.cElems; i++)
            {
                IntPtr ptr = new IntPtr(this.pElems.ToInt64() + (Marshal.SizeOf(typeof(Guid)) * i));
                retval[i] = (Guid)Marshal.PtrToStructure(ptr, typeof(Guid));
            }

            return retval;
        }
    }

    /// <summary>
    /// DirectShowLib.DsLong is a wrapper class around a <see cref="System.Int64"/> value type.
    /// </summary>
    /// <remarks>
    /// This class is necessary to enable null paramters passing.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    [PublicAPI]
    public class DsLong
    {
        private readonly long _value;

        /// <summary>
        /// Constructor
        /// Initialize a new instance of DirectShowLib.DsLong with the Value parameter
        /// </summary>
        /// <param name="value">Value to assign to this new instance</param>
        public DsLong(long value)
        {
            _value = value;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsLong Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return _value.ToString();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsLong and System.Int64 for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="ToInt64"/> for similar functionality.
        /// <code>
        ///   // Define a new DsLong instance
        ///   DsLong dsL = new DsLong(9876543210);
        ///   // Do implicit cast between DsLong and Int64
        ///   long l = dsL;
        ///
        ///   Console.WriteLine(l.ToString());
        /// </code>
        /// </summary>
        /// <param name="value">DirectShowLib.DsLong to be cast</param>
        /// <returns>A casted System.Int64</returns>
        public static implicit operator long(DsLong value)
        {
            return value._value;
        }

        /// <summary>
        /// Define implicit cast between System.Int64 and DirectShowLib.DsLong for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="FromInt64"/> for similar functionality.
        /// <code>
        ///   // Define a new Int64 instance
        ///   long l = 9876543210;
        ///   // Do implicit cast between Int64 and DsLong
        ///   DsLong dsl = l;
        ///
        ///   Console.WriteLine(dsl.ToString());
        /// </code>
        /// </summary>
        /// <param name="value">System.Int64 to be cast</param>
        /// <returns>A casted DirectShowLib.DsLong</returns>
        public static implicit operator DsLong(long value)
        {
            return new DsLong(value);
        }

        /// <summary>
        /// Get the System.Int64 equivalent to this DirectShowLib.DsLong instance.
        /// </summary>
        /// <returns>A System.Int64</returns>
        public long ToInt64()
        {
            return _value;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsLong instance for a given System.Int64
        /// </summary>
        /// <param name="value">The System.Int64 to wrap into a DirectShowLib.DsLong</param>
        /// <returns>A new instance of DirectShowLib.DsLong</returns>
        public static DsLong FromInt64(long value)
        {
            return new DsLong(value);
        }
    }

    /// <summary>
    /// DirectShowLib.DsGuid is a wrapper class around a System.Guid value type.
    /// </summary>
    /// <remarks>
    /// This class is necessary to enable null parameters passing.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    [PublicAPI]
    public class DsGuid
    {
        [FieldOffset(0)]
        private Guid _guid;

        public static readonly DsGuid Empty = Guid.Empty;

        /// <summary>
        /// Empty constructor. 
        /// Initialize it with System.Guid.Empty
        /// </summary>
        public DsGuid()
        {
            _guid = Guid.Empty;
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid string representation.
        /// </summary>
        /// <param name="value">A valid System.Guid as string</param>
        public DsGuid(string value)
        {
            _guid = new Guid(value);
        }

        /// <summary>
        /// Constructor.
        /// Initialize this instance with a given System.Guid.
        /// </summary>
        /// <param name="value">A System.Guid value type</param>
        public DsGuid(Guid value)
        {
            _guid = value;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsGuid Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return _guid.ToString();
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsGuid Instance with a specific format.
        /// </summary>
        /// <param name="format"><see cref="Guid.ToString()"/> for a description of the format parameter.</param>
        /// <returns>A string representing this instance according to the format parameter</returns>
        public string ToString(string format)
        {
            return _guid.ToString(format);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _guid.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsGuid and System.Guid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="ToGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new DsGuid instance
        ///   DsGuid dsG = new DsGuid("{33D57EBF-7C9D-435e-A15E-D300B52FBD91}");
        ///   // Do implicit cast between DsGuid and Guid
        ///   Guid g = dsG;
        ///
        ///   Console.WriteLine(g.ToString());
        /// </code>
        /// </summary>
        /// <param name="value">DirectShowLib.DsGuid to be cast</param>
        /// <returns>A casted System.Guid</returns>
        public static implicit operator Guid(DsGuid value)
        {
            return value._guid;
        }

        /// <summary>
        /// Define implicit cast between System.Guid and DirectShowLib.DsGuid for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="FromGuid"/> for similar functionality.
        /// <code>
        ///   // Define a new Guid instance
        ///   Guid g = new Guid("{B9364217-366E-45f8-AA2D-B0ED9E7D932D}");
        ///   // Do implicit cast between Guid and DsGuid
        ///   DsGuid dsG = g;
        ///
        ///   Console.WriteLine(dsG.ToString());
        /// </code>
        /// </summary>
        /// <param name="value">System.Guid to be cast</param>
        /// <returns>A casted DirectShowLib.DsGuid</returns>
        public static implicit operator DsGuid(Guid value)
        {
            return new DsGuid(value);
        }

        /// <summary>
        /// Get the System.Guid equivalent to this DirectShowLib.DsGuid instance.
        /// </summary>
        /// <returns>A System.Guid</returns>
        public Guid ToGuid()
        {
            return _guid;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsGuid instance for a given System.Guid
        /// </summary>
        /// <param name="value">The System.Guid to wrap into a DirectShowLib.DsGuid</param>
        /// <returns>A new instance of DirectShowLib.DsGuid</returns>
        public static DsGuid FromGuid(Guid value)
        {
            return new DsGuid(value);
        }
    }

    /// <summary>
    /// DirectShowLib.DsInt is a wrapper class around a <see cref="Int32"/> value type.
    /// </summary>
    /// <remarks>
    /// This class is necessary to enable null parameters passing.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    [PublicAPI]
    public class DsInt
    {
        private readonly int _value;

        /// <summary>
        /// Constructor
        /// Initialize a new instance of DirectShowLib.DsInt with the Value parameter
        /// </summary>
        /// <param name="value">Value to assign to this new instance</param>
        public DsInt(int value)
        {
            _value = value;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsInt Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return _value.ToString();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsInt and System.Int64 for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="ToInt32"/> for similar functionality.
        /// <code>
        ///   // Define a new DsInt instance
        ///   DsInt dsI = new DsInt(0x12345678);
        ///   // Do implicit cast between DsInt and Int32
        ///   int i = dsI;
        ///
        ///   Console.WriteLine(i.ToString());
        /// </code>
        /// </summary>
        /// <param name="value">DirectShowLib.DsInt to be cast</param>
        /// <returns>A casted System.Int32</returns>
        public static implicit operator int(DsInt value)
        {
            return value._value;
        }

        /// <summary>
        /// Define implicit cast between System.Int32 and DirectShowLib.DsInt for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="FromInt32"/> for similar functionality.
        /// <code>
        ///   // Define a new Int32 instance
        ///   int i = 0x12345678;
        ///   // Do implicit cast between Int64 and DsInt
        ///   DsInt dsI = i;
        ///
        ///   Console.WriteLine(dsI.ToString());
        /// </code>
        /// </summary>
        /// <param name="value">System.Int32 to be cast</param>
        /// <returns>A casted DirectShowLib.DsInt</returns>
        public static implicit operator DsInt(int value)
        {
            return new DsInt(value);
        }

        /// <summary>
        /// Get the System.Int32 equivalent to this DirectShowLib.DsInt instance.
        /// </summary>
        /// <returns>A System.Int32</returns>
        public int ToInt32()
        {
            return _value;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsInt instance for a given System.Int32
        /// </summary>
        /// <param name="value">The System.Int32 to wrap into a DirectShowLib.DsInt</param>
        /// <returns>A new instance of DirectShowLib.DsInt</returns>
        public static DsInt FromInt32(int value)
        {
            return new DsInt(value);
        }
    }

    /// <summary>
    /// DirectShowLib.DsShort is a wrapper class around a <see cref="System.Int16"/> value type.
    /// </summary>
    /// <remarks>
    /// This class is necessary to enable null parameters passing.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    [PublicAPI]
    public class DsShort
    {
        private readonly short _value;

        /// <summary>
        /// Constructor
        /// Initialize a new instance of DirectShowLib.DsShort with the Value parameter
        /// </summary>
        /// <param name="value">Value to assign to this new instance</param>
        public DsShort(short value)
        {
            _value = value;
        }

        /// <summary>
        /// Get a string representation of this DirectShowLib.DsShort Instance.
        /// </summary>
        /// <returns>A string representing this instance</returns>
        public override string ToString()
        {
            return _value.ToString();
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsShort and System.Int16 for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="DsShort.ToInt16"/> for similar functionality.
        /// <code>
        ///   // Define a new DsShort instance
        ///   DsShort dsS = new DsShort(0x1234);
        ///   // Do implicit cast between DsShort and Int16
        ///   short s = dsS;
        ///
        ///   Console.WriteLine(s.ToString());
        /// </code>
        /// </summary>
        /// <param name="value">DirectShowLib.DsShort to be cast</param>
        /// <returns>A casted System.Int16</returns>
        public static implicit operator short(DsShort value)
        {
            return value._value;
        }

        /// <summary>
        /// Define implicit cast between System.Int16 and DirectShowLib.DsShort for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="FromInt16"/> for similar functionality.
        /// <code>
        ///   // Define a new Int16 instance
        ///   short s = 0x1234;
        ///   // Do implicit cast between Int64 and DsShort
        ///   DsShort dsS = s;
        ///
        ///   Console.WriteLine(dsS.ToString());
        /// </code>
        /// </summary>
        /// <param name="value">System.Int16 to be cast</param>
        /// <returns>A casted DirectShowLib.DsShort</returns>
        public static implicit operator DsShort(short value)
        {
            return new DsShort(value);
        }

        /// <summary>
        /// Get the System.Int16 equivalent to this DirectShowLib.DsShort instance.
        /// </summary>
        /// <returns>A System.Int16</returns>
        public short ToInt16()
        {
            return _value;
        }

        /// <summary>
        /// Get a new DirectShowLib.DsShort instance for a given System.Int64
        /// </summary>
        /// <param name="value">The System.Int16 to wrap into a DirectShowLib.DsShort</param>
        /// <returns>A new instance of DirectShowLib.DsShort</returns>
        public static DsShort FromInt16(short value)
        {
            return new DsShort(value);
        }
    }

    /// <summary>
    /// DirectShowLib.DsRect is a managed representation of the Win32 RECT structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [PublicAPI]
    public class DsRect
    {
        /// <summary>
        /// Left coordinate
        /// </summary>
        public int left;

        /// <summary>
        /// Top coordinate
        /// </summary>
        public int top;

        /// <summary>
        /// Right coordinate
        /// </summary>
        public int right;

        /// <summary>
        /// Bottom coordinate
        /// </summary>
        public int bottom;

        /// <summary>
        /// Empty contructor. Initialize all fields to 0
        /// </summary>
        public DsRect()
        {
            left = 0;
            top = 0;
            right = 0;
            bottom = 0;
        }

        /// <summary>
        /// A parametred constructor. Initialize fields with given values.
        /// </summary>
        /// <param name="left">the left value</param>
        /// <param name="top">the top value</param>
        /// <param name="right">the right value</param>
        /// <param name="bottom">the bottom value</param>
        public DsRect(int left, int top, int right, int bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        /// <summary>
        /// A parametred constructor. Initialize fields with a given <see cref="System.Drawing.Rectangle"/>.
        /// </summary>
        /// <param name="rectangle">A <see cref="System.Drawing.Rectangle"/></param>
        /// <remarks>
        /// Warning, DsRect define a rectangle by defining two of his corners and <see cref="System.Drawing.Rectangle"/> define a rectangle with his upper/left corner, his width and his height.
        /// </remarks>
        public DsRect(Rectangle rectangle)
        {
            left = rectangle.Left;
            top = rectangle.Top;
            right = rectangle.Right;
            bottom = rectangle.Bottom;
        }

        /// <summary>
        /// Provide de string representation of this DsRect instance
        /// </summary>
        /// <returns>A string formated like this : [left, top - right, bottom]</returns>
        public override string ToString()
        {
            return string.Format("[{0}, {1} - {2}, {3}]", left, top, right, bottom);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return left.GetHashCode() | top.GetHashCode() | right.GetHashCode()
                   | bottom.GetHashCode();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var cmpRect = obj as DsRect;
            if (cmpRect != null)
            {
                return right == cmpRect.right && bottom == cmpRect.bottom && left == cmpRect.left && top == cmpRect.top;
            }

            if (obj is Rectangle)
            {
                Rectangle cmp = (Rectangle)obj;

                return right == cmp.Right && bottom == cmp.Bottom && left == cmp.Left && top == cmp.Top;
            }

            return false;
        }

        /// <summary>
        /// Define implicit cast between DirectShowLib.DsRect and System.Drawing.Rectangle for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="ToRectangle"/> for similar functionality.
        /// <code>
        ///   // Define a new Rectangle instance
        ///   Rectangle r = new Rectangle(0, 0, 100, 100);
        ///   // Do implicit cast between Rectangle and DsRect
        ///   DsRect dsR = r;
        ///
        ///   Console.WriteLine(dsR.ToString());
        /// </code>
        /// </summary>
        /// <param name="r">a DsRect to be cast</param>
        /// <returns>A casted System.Drawing.Rectangle</returns>
        public static implicit operator Rectangle(DsRect r)
        {
            return r.ToRectangle();
        }

        /// <summary>
        /// Define implicit cast between System.Drawing.Rectangle and DirectShowLib.DsRect for languages supporting this feature.
        /// VB.Net doesn't support implicit cast. <see cref="FromRectangle"/> for similar functionality.
        /// <code>
        ///   // Define a new DsRect instance
        ///   DsRect dsR = new DsRect(0, 0, 100, 100);
        ///   // Do implicit cast between DsRect and Rectangle
        ///   Rectangle r = dsR;
        ///
        ///   Console.WriteLine(r.ToString());
        /// </code>
        /// </summary>
        /// <param name="r">A System.Drawing.Rectangle to be cast</param>
        /// <returns>A casted DsRect</returns>
        public static implicit operator DsRect(Rectangle r)
        {
            return new DsRect(r);
        }

        /// <summary>
        /// Get the System.Drawing.Rectangle equivalent to this DirectShowLib.DsRect instance.
        /// </summary>
        /// <returns>A System.Drawing.Rectangle</returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Get a new DirectShowLib.DsRect instance for a given <see cref="System.Drawing.Rectangle"/>
        /// </summary>
        /// <param name="r">The <see cref="System.Drawing.Rectangle"/> used to initialize this new DirectShowLib.DsGuid</param>
        /// <returns>A new instance of DirectShowLib.DsGuid</returns>
        public static DsRect FromRectangle(Rectangle r)
        {
            return new DsRect(r);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    [PublicAPI]
    public struct NormalizedRect
    {
        /// <summary>
        /// Left coordinate
        /// </summary>
        public float left;

        /// <summary>
        /// Top coordinate
        /// </summary>
        public float top;

        /// <summary>
        /// Right coordinate
        /// </summary>
        public float right;

        /// <summary>
        /// Bottom coordinate
        /// </summary>
        public float bottom;

        /// <summary>
        /// Initializes new instance of the <see cref="NormalizedRect"/> structure.
        /// </summary>
        /// <param name="l"></param>
        /// <param name="t"></param>
        /// <param name="r"></param>
        /// <param name="b"></param>
        public NormalizedRect(float l, float t, float r, float b)
        {
            left = l;
            top = t;
            right = r;
            bottom = b;
        }

        public NormalizedRect(RectangleF r)
        {
            left = r.Left;
            top = r.Top;
            right = r.Right;
            bottom = r.Bottom;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Format("[{0}, {1} - {2}, {3}]", left, top, right, bottom);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return left.GetHashCode() | top.GetHashCode() | right.GetHashCode()
                   | bottom.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public static implicit operator RectangleF(NormalizedRect r)
        {
            return r.ToRectangleF();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public static implicit operator NormalizedRect(Rectangle r)
        {
            return new NormalizedRect(r);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator ==(NormalizedRect r1, NormalizedRect r2)
        {
            return ((r1.left == r2.left) && (r1.top == r2.top) && (r1.right == r2.right) && (r1.bottom == r2.bottom));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r1"></param>
        /// <param name="r2"></param>
        /// <returns></returns>
        public static bool operator !=(NormalizedRect r1, NormalizedRect r2)
        {
            return ((r1.left != r2.left) || (r1.top != r2.top) || (r1.right != r2.right) || (r1.bottom != r2.bottom));
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is NormalizedRect)) return false;

            NormalizedRect other = (NormalizedRect)obj;
            return (this == other);
        }

        /// <summary>
        /// Converts to <see cref="RectangleF"/> class
        /// </summary>
        /// <returns></returns>
        public RectangleF ToRectangleF()
        {
            return new RectangleF(left, top, right - left, bottom - top);
        }

        /// <summary>
        /// Converts from <see cref="RectangleF"/> class
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        public static NormalizedRect FromRectangle(RectangleF r)
        {
            return new NormalizedRect(r);
        }
    }

    #endregion

    #region Utility Classes

    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public static class DsResults
    {
        /// <summary>
        /// 
        /// </summary>
        public const int E_InvalidMediaType = unchecked((int)0x80040200);

        /// <summary>
        /// 
        /// </summary>
        public const int E_InvalidSubType = unchecked((int)0x80040201);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NeedOwner = unchecked((int)0x80040202);

        /// <summary>
        /// 
        /// </summary>
        public const int E_EnumOutOfSync = unchecked((int)0x80040203);

        /// <summary>
        /// 
        /// </summary>
        public const int E_AlreadyConnected = unchecked((int)0x80040204);

        /// <summary>
        /// 
        /// </summary>
        public const int E_FilterActive = unchecked((int)0x80040205);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoTypes = unchecked((int)0x80040206);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoAcceptableTypes = unchecked((int)0x80040207);

        /// <summary>
        /// 
        /// </summary>
        public const int E_InvalidDirection = unchecked((int)0x80040208);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NotConnected = unchecked((int)0x80040209);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoAllocator = unchecked((int)0x8004020A);

        /// <summary>
        /// 
        /// </summary>
        public const int E_RunTimeError = unchecked((int)0x8004020B);

        /// <summary>
        /// 
        /// </summary>
        public const int E_BufferNotSet = unchecked((int)0x8004020C);

        /// <summary>
        /// 
        /// </summary>
        public const int E_BufferOverflow = unchecked((int)0x8004020D);

        /// <summary>
        /// 
        /// </summary>
        public const int E_BadAlign = unchecked((int)0x8004020E);

        /// <summary>
        /// 
        /// </summary>
        public const int E_AlreadyCommitted = unchecked((int)0x8004020F);

        /// <summary>
        /// 
        /// </summary>
        public const int E_BuffersOutstanding = unchecked((int)0x80040210);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NotCommitted = unchecked((int)0x80040211);

        /// <summary>
        /// 
        /// </summary>
        public const int E_SizeNotSet = unchecked((int)0x80040212);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoClock = unchecked((int)0x80040213);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoSink = unchecked((int)0x80040214);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoInterface = unchecked((int)0x80040215);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NotFound = unchecked((int)0x80040216);

        /// <summary>
        /// 
        /// </summary>
        public const int E_CannotConnect = unchecked((int)0x80040217);

        /// <summary>
        /// 
        /// </summary>
        public const int E_CannotRender = unchecked((int)0x80040218);

        /// <summary>
        /// 
        /// </summary>
        public const int E_ChangingFormat = unchecked((int)0x80040219);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoColorKeySet = unchecked((int)0x8004021A);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NotOverlayConnection = unchecked((int)0x8004021B);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NotSampleConnection = unchecked((int)0x8004021C);

        /// <summary>
        /// 
        /// </summary>
        public const int E_PaletteSet = unchecked((int)0x8004021D);

        /// <summary>
        /// 
        /// </summary>
        public const int E_ColorKeySet = unchecked((int)0x8004021E);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoColorKeyFound = unchecked((int)0x8004021F);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoPaletteAvailable = unchecked((int)0x80040220);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoDisplayPalette = unchecked((int)0x80040221);

        /// <summary>
        /// 
        /// </summary>
        public const int E_TooManyColors = unchecked((int)0x80040222);

        /// <summary>
        /// 
        /// </summary>
        public const int E_StateChanged = unchecked((int)0x80040223);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NotStopped = unchecked((int)0x80040224);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NotPaused = unchecked((int)0x80040225);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NotRunning = unchecked((int)0x80040226);

        /// <summary>
        /// 
        /// </summary>
        public const int E_WrongState = unchecked((int)0x80040227);

        /// <summary>
        /// 
        /// </summary>
        public const int E_StartTimeAfterEnd = unchecked((int)0x80040228);

        /// <summary>
        /// 
        /// </summary>
        public const int E_InvalidRect = unchecked((int)0x80040229);

        /// <summary>
        /// 
        /// </summary>
        public const int E_TypeNotAccepted = unchecked((int)0x8004022A);

        /// <summary>
        /// 
        /// </summary>
        public const int E_SampleRejected = unchecked((int)0x8004022B);

        /// <summary>
        /// 
        /// </summary>
        public const int E_SampleRejectedEOS = unchecked((int)0x8004022C);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DuplicateName = unchecked((int)0x8004022D);

        /// <summary>
        /// 
        /// </summary>
        public const int S_DuplicateName = unchecked((int)0x0004022D);

        /// <summary>
        /// 
        /// </summary>
        public const int E_Timeout = unchecked((int)0x8004022E);

        /// <summary>
        /// 
        /// </summary>
        public const int E_InvalidFileFormat = unchecked((int)0x8004022F);

        /// <summary>
        /// 
        /// </summary>
        public const int E_EnumOutOfRange = unchecked((int)0x80040230);

        /// <summary>
        /// 
        /// </summary>
        public const int E_CircularGraph = unchecked((int)0x80040231);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NotAllowedToSave = unchecked((int)0x80040232);

        /// <summary>
        /// 
        /// </summary>
        public const int E_TimeAlreadyPassed = unchecked((int)0x80040233);

        /// <summary>
        /// 
        /// </summary>
        public const int E_AlreadyCancelled = unchecked((int)0x80040234);

        /// <summary>
        /// 
        /// </summary>
        public const int E_CorruptGraphFile = unchecked((int)0x80040235);

        /// <summary>
        /// 
        /// </summary>
        public const int E_AdviseAlreadySet = unchecked((int)0x80040236);

        /// <summary>
        /// 
        /// </summary>
        public const int S_StateIntermediate = unchecked((int)0x00040237);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoModexAvailable = unchecked((int)0x80040238);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoAdviseSet = unchecked((int)0x80040239);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoFullScreen = unchecked((int)0x8004023A);

        /// <summary>
        /// 
        /// </summary>
        public const int E_InFullScreenMode = unchecked((int)0x8004023B);

        /// <summary>
        /// 
        /// </summary>
        public const int E_UnknownFileType = unchecked((int)0x80040240);

        /// <summary>
        /// 
        /// </summary>
        public const int E_CannotLoadSourceFilter = unchecked((int)0x80040241);

        /// <summary>
        /// 
        /// </summary>
        public const int S_PartialRender = unchecked((int)0x00040242);

        /// <summary>
        /// 
        /// </summary>
        public const int E_FileTooShort = unchecked((int)0x80040243);

        /// <summary>
        /// 
        /// </summary>
        public const int E_InvalidFileVersion = unchecked((int)0x80040244);

        /// <summary>
        /// 
        /// </summary>
        public const int S_SomeDataIgnored = unchecked((int)0x00040245);

        /// <summary>
        /// 
        /// </summary>
        public const int S_ConnectionsDeferred = unchecked((int)0x00040246);

        /// <summary>
        /// 
        /// </summary>
        public const int E_InvalidCLSID = unchecked((int)0x80040247);

        /// <summary>
        /// 
        /// </summary>
        public const int E_InvalidMediaType2 = unchecked((int)0x80040248);

        /// <summary>
        /// 
        /// </summary>
        public const int E_BabKey = unchecked((int)0x800403F2);

        /// <summary>
        /// 
        /// </summary>
        public const int S_NoMoreItems = unchecked((int)0x00040103);

        /// <summary>
        /// 
        /// </summary>
        public const int E_SampleTimeNotSet = unchecked((int)0x80040249);

        /// <summary>
        /// 
        /// </summary>
        public const int S_ResourceNotNeeded = unchecked((int)0x00040250);

        /// <summary>
        /// 
        /// </summary>
        public const int E_MediaTimeNotSet = unchecked((int)0x80040251);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoTimeFormatSet = unchecked((int)0x80040252);

        /// <summary>
        /// 
        /// </summary>
        public const int E_MonoAudioHW = unchecked((int)0x80040253);

        /// <summary>
        /// 
        /// </summary>
        public const int S_MediaTypeIgnored = unchecked((int)0x00040254);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoDecompressor = unchecked((int)0x80040255);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoAudioHardware = unchecked((int)0x80040256);

        /// <summary>
        /// 
        /// </summary>
        public const int S_VideoNotRendered = unchecked((int)0x00040257);

        /// <summary>
        /// 
        /// </summary>
        public const int S_AudioNotRendered = unchecked((int)0x00040258);

        /// <summary>
        /// 
        /// </summary>
        public const int E_RPZA = unchecked((int)0x80040259);

        /// <summary>
        /// 
        /// </summary>
        public const int S_RPZA = unchecked((int)0x0004025A);

        /// <summary>
        /// 
        /// </summary>
        public const int E_ProcessorNotSuitable = unchecked((int)0x8004025B);

        /// <summary>
        /// 
        /// </summary>
        public const int E_UnsupportedAudio = unchecked((int)0x8004025C);

        /// <summary>
        /// 
        /// </summary>
        public const int E_UnsupportedVideo = unchecked((int)0x8004025D);

        /// <summary>
        /// 
        /// </summary>
        public const int E_MPEGNotConstrained = unchecked((int)0x8004025E);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NotInGraph = unchecked((int)0x8004025F);

        /// <summary>
        /// 
        /// </summary>
        public const int S_Estimated = unchecked((int)0x00040260);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoTimeFormat = unchecked((int)0x80040261);

        /// <summary>
        /// 
        /// </summary>
        public const int E_ReadOnly = unchecked((int)0x80040262);

        /// <summary>
        /// 
        /// </summary>
        public const int S_Reserved = unchecked((int)0x00040263);

        /// <summary>
        /// 
        /// </summary>
        public const int E_BufferUnderflow = unchecked((int)0x80040264);

        /// <summary>
        /// 
        /// </summary>
        public const int E_UnsupportedStream = unchecked((int)0x80040265);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoTransport = unchecked((int)0x80040266);

        /// <summary>
        /// 
        /// </summary>
        public const int S_StreamOff = unchecked((int)0x00040267);

        /// <summary>
        /// 
        /// </summary>
        public const int S_CantCue = unchecked((int)0x00040268);

        /// <summary>
        /// 
        /// </summary>
        public const int E_BadVideoCD = unchecked((int)0x80040269);

        /// <summary>
        /// 
        /// </summary>
        public const int S_NoStopTime = unchecked((int)0x00040270);

        /// <summary>
        /// 
        /// </summary>
        public const int E_OutOfVideoMemory = unchecked((int)0x80040271);

        /// <summary>
        /// 
        /// </summary>
        public const int E_VPNegotiationFailed = unchecked((int)0x80040272);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DDrawCapsNotSuitable = unchecked((int)0x80040273);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoVPHardware = unchecked((int)0x80040274);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoCaptureHardware = unchecked((int)0x80040275);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDOperationInhibited = unchecked((int)0x80040276);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDInvalidDomain = unchecked((int)0x80040277);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDNoButton = unchecked((int)0x80040278);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDGraphNotReady = unchecked((int)0x80040279);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDRenderFail = unchecked((int)0x8004027A);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDDecNotEnough = unchecked((int)0x8004027B);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DDrawVersionNotSuitable = unchecked((int)0x8004027C);

        /// <summary>
        /// 
        /// </summary>
        public const int E_CopyProtFailed = unchecked((int)0x8004027D);

        /// <summary>
        /// 
        /// </summary>
        public const int S_NoPreviewPin = unchecked((int)0x0004027E);

        /// <summary>
        /// 
        /// </summary>
        public const int E_TimeExpired = unchecked((int)0x8004027F);

        /// <summary>
        /// 
        /// </summary>
        public const int S_DVDNonOneSequential = unchecked((int)0x00040280);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDWrongSpeed = unchecked((int)0x80040281);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDMenuDoesNotExist = unchecked((int)0x80040282);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDCmdCancelled = unchecked((int)0x80040283);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDStateWrongVersion = unchecked((int)0x80040284);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDStateCorrupt = unchecked((int)0x80040285);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDStateWrongDisc = unchecked((int)0x80040286);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDIncompatibleRegion = unchecked((int)0x80040287);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDNoAttributes = unchecked((int)0x80040288);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDNoGoupPGC = unchecked((int)0x80040289);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDLowParentalLevel = unchecked((int)0x8004028A);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDNotInKaraokeMode = unchecked((int)0x8004028B);

        /// <summary>
        /// 
        /// </summary>
        public const int S_DVDChannelContentsNotAvailable = unchecked((int)0x0004028C);

        /// <summary>
        /// 
        /// </summary>
        public const int S_DVDNotAccurate = unchecked((int)0x0004028D);

        /// <summary>
        /// 
        /// </summary>
        public const int E_FrameStepUnsupported = unchecked((int)0x8004028E);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDStreamDisabled = unchecked((int)0x8004028F);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDTitleUnknown = unchecked((int)0x80040290);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDInvalidDisc = unchecked((int)0x80040291);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDNoResumeInformation = unchecked((int)0x80040292);

        /// <summary>
        /// 
        /// </summary>
        public const int E_PinAlreadyBlockedOnThisThread = unchecked((int)0x80040293);

        /// <summary>
        /// 
        /// </summary>
        public const int E_PinAlreadyBlocked = unchecked((int)0x80040294);

        /// <summary>
        /// 
        /// </summary>
        public const int E_CertificationFailure = unchecked((int)0x80040295);

        /// <summary>
        /// 
        /// </summary>
        public const int E_VMRNotInMixerMode = unchecked((int)0x80040296);

        /// <summary>
        /// 
        /// </summary>
        public const int E_VMRNoApSupplied = unchecked((int)0x80040297);

        /// <summary>
        /// 
        /// </summary>
        public const int E_VMRNoDeinterlace_HW = unchecked((int)0x80040298);

        /// <summary>
        /// 
        /// </summary>
        public const int E_VMRNoProcAMPHW = unchecked((int)0x80040299);

        /// <summary>
        /// 
        /// </summary>
        public const int E_DVDVMR9IncompatibleDec = unchecked((int)0x8004029A);

        /// <summary>
        /// 
        /// </summary>
        public const int E_NoCOPPHW = unchecked((int)0x8004029B);
    }

    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public static class DsError
    {
        /// <summary>
        /// The AMGetErrorText function retrieves the error message for a given return code, using the current language setting.
        /// </summary>
        /// <param name="hr">HRESULT value.</param>
        /// <param name="buf">Pointer to a character buffer that receives the error message.</param>
        /// <param name="max">Number of characters in <paramref name="buf"/>.</param>
        /// <returns>Returns the number of characters returned in the buffer, or zero if an error occurred.</returns>
        [DllImport("quartz.dll", CharSet = CharSet.Unicode, ExactSpelling = true, EntryPoint = "AMGetErrorTextW"),
         SuppressUnmanagedCodeSecurity]
        public static extern int AMGetErrorText(int hr, StringBuilder buf, int max);

        /// <summary>
        /// If hr has a "failed" status code (E_*), throw an exception.  Note that status
        /// messages (S_*) are not considered failure codes.  If DirectShow error text
        /// is available, it is used to build the exception, otherwise a generic com error
        /// is thrown.
        /// </summary>
        /// <param name="hr">The HRESULT to check</param>
        public static void ThrowExceptionForHR(int hr)
        {
            // If a severe error has occurred
            if (hr < 0)
            {
                var s = GetErrorText(hr);

                // If a string is returned, build a com error from it
                if (s != null)
                {
                    throw new COMException(s, hr);
                }

                // No string, just use standard com error
                Marshal.ThrowExceptionForHR(hr);
            }
        }

        /// <summary>
        /// Returns a string describing a DS error.  Works for both error codes
        /// (values &lt; 0) and Status codes (values >= 0)
        /// </summary>
        /// <param name="hr">HRESULT for which to get description</param>
        /// <returns>The string, or null if no error text can be found</returns>
        public static string GetErrorText(int hr)
        {
            const int MAX_ERROR_TEXT_LEN = 160;

            // Make a buffer to hold the string
            var buf = new StringBuilder(MAX_ERROR_TEXT_LEN, MAX_ERROR_TEXT_LEN);

            // If a string is returned, build a com error from it
            return AMGetErrorText(hr, buf, MAX_ERROR_TEXT_LEN) > 0 ? buf.ToString() : null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public static class DsUtils
    {
        /// <summary>
        /// Returns the PinCategory of the specified pin.  Usually a member of PinCategory.  Not all pins have a category.
        /// </summary>
        /// <param name="pPin"></param>
        /// <returns>Guid indicating pin category or Guid.Empty on no category.  Usually a member of PinCategory</returns>
        public static Guid GetPinCategory(IPin pPin)
        {
            Guid guidRet = Guid.Empty;

            // Memory to hold the returned guid
            int iSize = Marshal.SizeOf(typeof(Guid));
            IntPtr ipOut = Marshal.AllocCoTaskMem(iSize);

            try
            {
                int hr;
                int cbBytes;
                var g = PropSetID.Pin;

                // Get an IKsPropertySet from the pin
                var pKs = pPin as IKsPropertySet;

                if (pKs != null)
                {
                    // Query for the Category
                    hr = pKs.Get(g, (int)AMPropertyPin.Category, IntPtr.Zero, 0, ipOut, iSize, out cbBytes);
                    DsError.ThrowExceptionForHR(hr);

                    // Marshal it to the return variable
                    guidRet = (Guid)Marshal.PtrToStructure(ipOut, typeof(Guid));
                }
            }
            finally
            {
                Marshal.FreeCoTaskMem(ipOut);
            }

            return guidRet;
        }

        /// <summary>
        ///  Free the nested structures and release any
        ///  COM objects within an AMMediaType struct.
        /// </summary>
        public static void FreeAMMediaType(AMMediaType mediaType)
        {
            if (mediaType != null)
            {
                if (mediaType.formatSize != 0)
                {
                    Marshal.FreeCoTaskMem(mediaType.formatPtr);
                    mediaType.formatSize = 0;
                    mediaType.formatPtr = IntPtr.Zero;
                }

                if (mediaType.unkPtr != IntPtr.Zero)
                {
                    Marshal.Release(mediaType.unkPtr);
                    mediaType.unkPtr = IntPtr.Zero;
                }
            }
        }

        /// <summary>
        ///  Free the nested interfaces within a PinInfo structure.
        /// </summary>
        public static void FreePinInfo(PinInfo pinInfo)
        {
            if (pinInfo.filter != null)
            {
                Marshal.ReleaseComObject(pinInfo.filter);
                pinInfo.filter = null;
            }
        }

        /// <summary>
        /// Releases Filter information
        /// </summary>
        /// <param name="filterInfo"></param>
        public static void FreeFilterInfo(FilterInfo filterInfo)
        {
            if (filterInfo.pGraph != null)
            {
                ReleaseComObject(filterInfo.pGraph);
                filterInfo.pGraph = null;
            }
        }

        /// <summary>
        /// Releases COM object
        /// </summary>
        /// <param name="obj"></param>
        public static void ReleaseComObject(object obj)
        {
            if (obj != null)
            {
                Marshal.ReleaseComObject(obj);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public class DsROTEntry : IDisposable
    {
        [Flags]
        private enum ROTFlags
        {
            RegistrationKeepsAlive = 0x1,

            AllowAnyClient = 0x2
        }

        private int _cookie = 0;

        #region APIs

        [DllImport("ole32.dll", ExactSpelling = true), SuppressUnmanagedCodeSecurity]
#if USING_NET11
        private static extern int GetRunningObjectTable(int r, out UCOMIRunningObjectTable pprot);
#else
        private static extern int GetRunningObjectTable(int r, out IRunningObjectTable pprot);

#endif

        [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true), SuppressUnmanagedCodeSecurity]
#if USING_NET11
        private static extern int CreateItemMoniker(string delim, string item, out UCOMIMoniker ppmk);
#else
        private static extern int CreateItemMoniker(string delim, string item, out IMoniker ppmk);

#endif

        #endregion

        /// <summary>
        /// Initializes new instance of the <see cref="DsROTEntry"/> with graph
        /// </summary>
        /// <param name="graph">The filter graph</param>
        public DsROTEntry(IFilterGraph graph)
        {
            int hr = 0;
#if USING_NET11
            UCOMIRunningObjectTable rot = null;
            UCOMIMoniker mk = null;
#else
            IRunningObjectTable rot = null;
            IMoniker mk = null;
#endif
            try
            {
                // First, get a pointer to the running object table
                hr = GetRunningObjectTable(0, out rot);
                DsError.ThrowExceptionForHR(hr);

                // Build up the object to add to the table
                int id = Process.GetCurrentProcess().Id;
                IntPtr iuPtr = Marshal.GetIUnknownForObject(graph);
                string s;
                try
                {
                    s = iuPtr.ToString("x");
                }
                catch
                {
                    s = "";
                }
                finally
                {
                    Marshal.Release(iuPtr);
                }
                string item = string.Format("FilterGraph {0} pid {1:x8}", s, id);
                hr = CreateItemMoniker("!", item, out mk);
                DsError.ThrowExceptionForHR(hr);

                // Add the object to the table
#if USING_NET11
                rot.Register((int)ROTFlags.RegistrationKeepsAlive, graph, mk, out m_cookie);
#else
                _cookie = rot.Register((int)ROTFlags.RegistrationKeepsAlive, graph, mk);
#endif
            }
            finally
            {
                DsUtils.ReleaseComObject(mk);
                DsUtils.ReleaseComObject(rot);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ~DsROTEntry()
        {
            Dispose();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_cookie != 0)
            {
                GC.SuppressFinalize(this);
#if USING_NET11
                UCOMIRunningObjectTable rot;
#else
                IRunningObjectTable rot;
#endif

                // Get a pointer to the running object table
                int hr = GetRunningObjectTable(0, out rot);
                DsError.ThrowExceptionForHR(hr);

                try
                {
                    // Remove our entry
                    rot.Revoke(_cookie);
                    _cookie = 0;
                }
                finally
                {
                    DsUtils.ReleaseComObject(rot);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public class DsDevice : IDisposable
    {
#if USING_NET11
        private UCOMIMoniker _moniker;
#else
        private IMoniker _moniker;
#endif
        private string _name;

#if USING_NET11
        public DsDevice(UCOMIMoniker Mon)
#else

        /// <summary>
        /// Initializes new instance of the <see cref="DsDevice"/> class
        /// </summary>
        /// <param name="Mon"></param>
        public DsDevice(IMoniker Mon)
#endif
        {
            _moniker = Mon;
            _name = null;
        }

#if USING_NET11
        public UCOMIMoniker Mon
#else

        /// <summary>
        /// Gets the device moniker
        /// </summary>
        public IMoniker Mon
#endif
        {
            get { return _moniker; }
        }

        /// <summary>
        /// Gets the device name
        /// </summary>
        public string Name
        {
            get
            {
                return _name ?? (_name = GetPropBagValue("FriendlyName"));
            }
        }

        /// <summary>
        /// Gets a unique identifier for a device
        /// </summary>
        public string DevicePath
        {
            get
            {
                string s = null;

                try
                {
                    _moniker.GetDisplayName(null, null, out s);
                }
                catch
                {
                }

                return s;
            }
        }

        /// <summary>
        /// Gets the ClassID for a device
        /// </summary>
        public Guid ClassID
        {
            get
            {
                Guid g;

                _moniker.GetClassID(out g);

                return g;
            }
        }


        /// <summary>
        /// Returns an array of DsDevices of type devcat.
        /// </summary>
        /// <param name="filterCategory">Any one of FilterCategory</param>
        public static DsDevice[] GetDevicesOfCat(Guid filterCategory)
        {
            // Use arrayList to build the retun list since it is easily resizable
            DsDevice[] devret;
            ArrayList devs = new ArrayList();
#if USING_NET11
            UCOMIEnumMoniker enumMon;
#else
            IEnumMoniker enumMon;
#endif

            ICreateDevEnum enumDev = (ICreateDevEnum)new CreateDevEnum();
            var hr = enumDev.CreateClassEnumerator(filterCategory, out enumMon, 0);
            DsError.ThrowExceptionForHR(hr);

            // CreateClassEnumerator returns null for enumMon if there are no entries
            if (hr != 1)
            {
                try
                {
                    try
                    {
#if USING_NET11
                        UCOMIMoniker[] mon = new UCOMIMoniker[1];
#else
                        IMoniker[] mon = new IMoniker[1];
#endif

#if USING_NET11
                        int j;
                        while ((enumMon.Next(1, mon, out j) == 0))
#else
                        while ((enumMon.Next(1, mon, IntPtr.Zero) == 0))
#endif
                        {
                            try
                            {
                                // The devs array now owns this object.  Don't
                                // release it if we are going to be successfully
                                // returning the devret array
                                devs.Add(new DsDevice(mon[0]));
                            }
                            catch
                            {
                                DsUtils.ReleaseComObject(mon[0]);
                                throw;
                            }
                        }
                    }
                    finally
                    {
                        DsUtils.ReleaseComObject(enumMon);
                    }

                    // Copy the ArrayList to the DsDevice[]
                    devret = new DsDevice[devs.Count];
                    devs.CopyTo(devret);
                }
                catch
                {
                    foreach (DsDevice d in devs)
                    {
                        d.Dispose();
                    }
                    throw;
                }
            }
            else
            {
                devret = new DsDevice[0];
            }

            return devret;
        }

        /// <summary>
        /// Get a specific PropertyBag value from a moniker
        /// </summary>
        /// <param name="sPropName">The name of the value to retrieve</param>
        /// <returns>String or null on error</returns>
        public string GetPropBagValue(string sPropName)
        {
            IPropertyBag bag = null;
            string ret = null;
            object bagObj = null;
            object val = null;

            try
            {
                Guid bagId = typeof(IPropertyBag).GUID;
                _moniker.BindToStorage(null, null, ref bagId, out bagObj);

                bag = (IPropertyBag)bagObj;

                int hr = bag.Read(sPropName, out val, null);
                DsError.ThrowExceptionForHR(hr);

                ret = val as string;
            }
            catch
            {
                ret = null;
            }
            finally
            {
                bag = null;
                if (bagObj != null)
                {
                    DsUtils.ReleaseComObject(bagObj);
                    bagObj = null;
                }
            }

            return ret;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (Mon != null)
            {
                DsUtils.ReleaseComObject(Mon);
                _moniker = null;
                GC.SuppressFinalize(this);
            }
            _name = null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public static class DsFindPin
    {
        /// <summary>
        /// Scans a filter's pins looking for a pin in the specified direction
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="vDir">The direction to find</param>
        /// <param name="iIndex">Zero based index (ie 2 will return the third pin in the specified direction)</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByDirection(IBaseFilter vSource, PinDirection vDir, int iIndex)
        {
            int hr;
            IEnumPins ppEnum;
            PinDirection ppindir;
            IPin pRet = null;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                int fetched;
                while (ppEnum.Next(1, pPins, out fetched) >= 0 && (fetched == 1))
                {
                    // Read the direction
                    hr = pPins[0].QueryDirection(out ppindir);
                    DsError.ThrowExceptionForHR(hr);

                    // Is it the right direction?
                    if (ppindir == vDir)
                    {
                        // Is is the right index?
                        if (iIndex == 0)
                        {
                            pRet = pPins[0];
                            break;
                        }
                        iIndex--;
                    }
                    DsUtils.ReleaseComObject(pPins[0]);
                }
            }
            finally
            {
                DsUtils.ReleaseComObject(ppEnum);
            }

            return pRet;
        }

        /// <summary>
        /// Scans a filter's pins looking for a pin with the specified name
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="vPinName">The pin name to find</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByName(IBaseFilter vSource, string vPinName)
        {
            int hr;
            IEnumPins ppEnum;
            PinInfo ppinfo;
            IPin pRet = null;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                int fetched;
                while (ppEnum.Next(1, pPins, out fetched) >= 0 && fetched == 1)
                {
                    // Read the info
                    hr = pPins[0].QueryPinInfo(out ppinfo);
                    DsError.ThrowExceptionForHR(hr);

                    // Is it the right name?
                    if (ppinfo.name == vPinName)
                    {
                        DsUtils.FreePinInfo(ppinfo);
                        pRet = pPins[0];
                        break;
                    }
                    DsUtils.ReleaseComObject(pPins[0]);
                    DsUtils.FreePinInfo(ppinfo);
                }
            }
            finally
            {
                DsUtils.ReleaseComObject(ppEnum);
            }

            return pRet;
        }

        /// <summary>
        /// Scan's a filter's pins looking for a pin with the specified category
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="guidPinCat">The guid from PinCategory to scan for</param>
        /// <param name="iIndex">Zero based index (ie 2 will return the third pin of the specified category)</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByCategory(IBaseFilter vSource, Guid PinCategory, int iIndex)
        {
            int hr;
            IEnumPins ppEnum;
            IPin pRet = null;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                int fetched;
                while (ppEnum.Next(1, pPins, out fetched) >= 0 && fetched == 1)
                {
                    // Is it the right category?
                    if (DsUtils.GetPinCategory(pPins[0]) == PinCategory)
                    {
                        // Is is the right index?
                        if (iIndex == 0)
                        {
                            pRet = pPins[0];
                            break;
                        }
                        iIndex--;
                    }
                    DsUtils.ReleaseComObject(pPins[0]);
                }
            }
            finally
            {
                DsUtils.ReleaseComObject(ppEnum);
            }

            return pRet;
        }

        /// <summary>
        /// Scans a filter's pins looking for a pin with the specified connection status
        /// </summary>
        /// <param name="vSource">The filter to scan</param>
        /// <param name="vStat">The status to find (connected/unconnected)</param>
        /// <param name="iIndex">Zero based index (ie 2 will return the third pin with the specified status)</param>
        /// <returns>The matching pin, or null if not found</returns>
        public static IPin ByConnectionStatus(IBaseFilter vSource, PinConnectedStatus vStat, int iIndex)
        {
            int hr;
            IEnumPins ppEnum;
            IPin pRet = null;
            IPin pOutPin;
            IPin[] pPins = new IPin[1];

            if (vSource == null)
            {
                return null;
            }

            // Get the pin enumerator
            hr = vSource.EnumPins(out ppEnum);
            DsError.ThrowExceptionForHR(hr);

            try
            {
                // Walk the pins looking for a match
                int fetched;
                while (ppEnum.Next(1, pPins, out fetched) >= 0 && fetched == 1)
                {
                    // Read the connected status
                    hr = pPins[0].ConnectedTo(out pOutPin);

                    // Check for VFW_E_NOT_CONNECTED.  Anything else is bad.
                    if (hr != DsResults.E_NotConnected)
                    {
                        DsError.ThrowExceptionForHR(hr);

                        // The ConnectedTo call succeeded, release the interface
                        DsUtils.ReleaseComObject(pOutPin);
                    }

                    // Is it the right status?
                    if ((hr == 0 && vStat == PinConnectedStatus.Connected)
                        || (hr == DsResults.E_NotConnected && vStat == PinConnectedStatus.Unconnected))
                    {
                        // Is is the right index?
                        if (iIndex == 0)
                        {
                            pRet = pPins[0];
                            break;
                        }
                        iIndex--;
                    }
                    DsUtils.ReleaseComObject(pPins[0]);
                }
            }
            finally
            {
                DsUtils.ReleaseComObject(ppEnum);
            }

            return pRet;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public static class DsToString
    {
        /// <summary>
        /// Produces a usable string that describes the MediaType object
        /// </summary>
        /// <returns>Concatenation of MajorType + SubType + FormatType + Fixed + Temporal + SampleSize.ToString</returns>
        public static string AMMediaTypeToString(AMMediaType pmt)
        {
            return string.Format(
                "{0} {1} {2} {3} {4} {5}",
                MediaTypeToString(pmt.majorType),
                MediaSubTypeToString(pmt.subType),
                MediaFormatTypeToString(pmt.formatType),
                (pmt.fixedSizeSamples ? "FixedSamples" : "NotFixedSamples"),
                (pmt.temporalCompression ? "temporalCompression" : "NottemporalCompression"),
                pmt.sampleSize);
        }

        /// <summary>
        /// Converts AMMediaType.MajorType Guid to a readable string
        /// </summary>
        /// <returns>MajorType Guid as a readable string or Guid if unrecognized</returns>
        public static string MediaTypeToString(Guid guid)
        {
            // Walk the MediaSubType class looking for a match
            return WalkClass(typeof(MediaType), guid);
        }

        /// <summary>
        /// Converts the AMMediaType.SubType Guid to a readable string
        /// </summary>
        /// <returns>SubType Guid as a readable string or Guid if unrecognized</returns>
        public static string MediaSubTypeToString(Guid guid)
        {
            // Walk the MediaSubType class looking for a match
            string s = WalkClass(typeof(MediaSubType), guid);

            // There is a special set of Guids that contain the FourCC code
            // as part of the Guid.  Check to see if it is one of those.
            if (s.Length == 36 && s.Substring(8).ToUpper() == "-0000-0010-8000-00AA00389B71")
            {
                // Parse out the FourCC code
                byte[] asc =
                    {
                        Convert.ToByte(s.Substring(6, 2), 16), Convert.ToByte(s.Substring(4, 2), 16),
                        Convert.ToByte(s.Substring(2, 2), 16), Convert.ToByte(s.Substring(0, 2), 16)
                    };
                s = Encoding.ASCII.GetString(asc);
            }

            return s;
        }

        /// <summary>
        /// Converts the AMMediaType.FormatType Guid to a readable string
        /// </summary>
        /// <returns>FormatType Guid as a readable string or Guid if unrecognized</returns>
        public static string MediaFormatTypeToString(Guid guid)
        {
            // Walk the FormatType class looking for a match
            return WalkClass(typeof(FormatType), guid);
        }

        /// <summary>
        /// Use reflection to walk a class looking for a property containing a specified guid
        /// </summary>
        /// <param name="MyType">Class to scan</param>
        /// <param name="guid">Guid to scan for</param>
        /// <returns>String representing property name that matches, or Guid.ToString() for no match</returns>
        private static string WalkClass(Type MyType, Guid guid)
        {
            object o = null;

            // Read the fields from the class
            FieldInfo[] Fields = MyType.GetFields();

            // Walk the returned array
            foreach (FieldInfo m in Fields)
            {
                // Read the value of the property.  The parameter is ignored.
                o = m.GetValue(o);

                // Compare it with the sought value
                if ((Guid)o == guid)
                {
                    return m.Name;
                }
            }

            return guid.ToString();
        }
    }


    /// <summary>
    /// This abstract class contains definitions for use in implementing a custom marshaler.
    ///
    /// MarshalManagedToNative() gets called before the COM method, and MarshalNativeToManaged() gets
    /// called after.  This allows for allocating a correctly sized memory block for the COM call,
    /// then to break up the memory block and build an object that c# can digest.
    /// </summary>
    [PublicAPI]
    internal abstract class DsMarshaler : ICustomMarshaler
    {
        #region Data Members

        /// <summary>
        /// The cookie isn't currently being used.
        /// </summary>
        protected string m_cookie;

        /// <summary>
        /// The managed object passed in to MarshalManagedToNative, and modified in MarshalNativeToManaged
        /// </summary>
        protected object m_obj;

        #endregion

        /// <summary>
        /// Initializes new instance of the <see cref="DsMarshaler"/> class
        /// </summary>
        /// <param name="cookie"></param>
        public DsMarshaler(string cookie)
        {
            // If we get a cookie, save it.
            m_cookie = cookie;
        }

        /// <summary>
        /// Called just before invoking the COM method.  The returned IntPtr is what goes on the stack
        /// for the COM call.  The input arg is the parameter that was passed to the method.
        /// </summary>
        /// <param name="managedObj"></param>
        /// <returns></returns>
        public virtual IntPtr MarshalManagedToNative(object managedObj)
        {
            // Save off the passed-in value.  Safe since we just checked the type.
            m_obj = managedObj;

            // Create an appropriately sized buffer, blank it, and send it to the marshaler to
            // make the COM call with.
            int iSize = GetNativeDataSize() + 3;
            IntPtr p = Marshal.AllocCoTaskMem(iSize);

            for (int x = 0; x < iSize / 4; x++)
            {
                Marshal.WriteInt32(p, x * 4, 0);
            }

            return p;
        }

        /// <summary>
        /// Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        /// from MarshalManagedToNative.  The return value is unused.
        /// </summary>
        /// <param name="pNativeData"></param>
        /// <returns></returns>
        public virtual object MarshalNativeToManaged(IntPtr pNativeData)
        {
            return m_obj;
        }

        /// <summary>
        /// Release the (now unused) buffer
        /// </summary>
        /// <param name="pNativeData"></param>
        public virtual void CleanUpNativeData(IntPtr pNativeData)
        {
            if (pNativeData != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pNativeData);
            }
        }

        /// <summary>
        /// Release the (now unused) managed object
        /// </summary>
        /// <param name="managedObj"></param>
        public virtual void CleanUpManagedData(object managedObj)
        {
            m_obj = null;
        }

        /// <summary>
        /// This routine is (apparently) never called by the marshaler.  However it can be useful.
        /// </summary>
        /// <returns></returns>
        public abstract int GetNativeDataSize();

        // GetInstance is called by the marshaler in preparation to doing custom marshaling.  The (optional)
        // cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.

        // It is commented out in this abstract class, but MUST be implemented in derived classes
        //public static ICustomMarshaler GetInstance(string cookie)
    }

    /// <summary>
    /// c# does not correctly marshal arrays of pointers.
    /// </summary>
    [PublicAPI]
    internal class EMTMarshaler : DsMarshaler
    {
        /// <summary>
        /// Initializes new instance of the <see cref="EMTMarshaler"/> class
        /// </summary>
        /// <param name="cookie"></param>
        public EMTMarshaler(string cookie)
            : base(cookie)
        {
        }

        /// <summary>
        /// Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        /// from MarshalManagedToNative.  The return value is unused.
        /// </summary>
        /// <param name="pNativeData"></param>
        /// <returns></returns>
        public override object MarshalNativeToManaged(IntPtr pNativeData)
        {
            AMMediaType[] emt = m_obj as AMMediaType[];

            for (int x = 0; x < emt.Length; x++)
            {
                // Copy in the value, and advance the pointer
                IntPtr p = Marshal.ReadIntPtr(pNativeData, x * IntPtr.Size);
                if (p != IntPtr.Zero)
                {
                    emt[x] = (AMMediaType)Marshal.PtrToStructure(p, typeof(AMMediaType));
                }
                else
                {
                    emt[x] = null;
                }
            }

            return null;
        }

        /// <summary>
        /// The number of bytes to marshal out
        /// </summary>
        /// <returns></returns>
        public override int GetNativeDataSize()
        {
            // Get the array size
            int i = ((Array)m_obj).Length;

            // Multiply that times the size of a pointer
            int j = i * IntPtr.Size;

            return j;
        }

        /// <summary>
        /// This method is called by interop to create the custom marshaler.  The (optional)
        /// cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static ICustomMarshaler GetInstance(string cookie)
        {
            return new EMTMarshaler(cookie);
        }
    }

    /// <summary>
    /// c# does not correctly create structures that contain ByValArrays of structures (or enums!).  Instead
    /// of allocating enough room for the ByValArray of structures, it only reserves room for a ref,
    /// even when decorated with ByValArray and SizeConst.  Needless to say, if DirectShow tries to
    /// write to this too-short buffer, bad things will happen.
    ///
    /// To work around this for the DvdTitleAttributes structure, use this custom marshaler
    /// by declaring the parameter DvdTitleAttributes as:
    ///
    ///    [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(DTAMarshaler))]
    ///   DvdTitleAttributes pTitle
    ///
    /// See DsMarshaler for more info on custom marshalers
    /// </summary>
    [PublicAPI]
    internal class DTAMarshaler : DsMarshaler
    {
        /// <summary>
        /// Initializes new instance of the <see cref="DTAMarshaler"/> class.
        /// </summary>
        /// <param name="cookie"></param>
        public DTAMarshaler(string cookie)
            : base(cookie)
        {
        }

        /// <summary>
        /// Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        /// from MarshalManagedToNative.  The return value is unused.
        /// </summary>
        /// <param name="pNativeData"></param>
        /// <returns></returns>
        public override object MarshalNativeToManaged(IntPtr pNativeData)
        {
            DvdTitleAttributes dta = m_obj as DvdTitleAttributes;

            // Copy in the value, and advance the pointer
            dta.AppMode = (DvdTitleAppMode)Marshal.ReadInt32(pNativeData);
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(int)));

            // Copy in the value, and advance the pointer
            dta.VideoAttributes = (DvdVideoAttributes)Marshal.PtrToStructure(pNativeData, typeof(DvdVideoAttributes));
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(DvdVideoAttributes)));

            // Copy in the value, and advance the pointer
            dta.ulNumberOfAudioStreams = Marshal.ReadInt32(pNativeData);
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(int)));

            // Allocate a large enough array to hold all the returned structs.
            dta.AudioAttributes = new DvdAudioAttributes[8];
            for (int x = 0; x < 8; x++)
            {
                // Copy in the value, and advance the pointer
                dta.AudioAttributes[x] =
                    (DvdAudioAttributes)Marshal.PtrToStructure(pNativeData, typeof(DvdAudioAttributes));
                pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(DvdAudioAttributes)));
            }

            // Allocate a large enough array to hold all the returned structs.
            dta.MultichannelAudioAttributes = new DvdMultichannelAudioAttributes[8];
            for (int x = 0; x < 8; x++)
            {
                // MultichannelAudioAttributes has nested ByValArrays.  They need to be individually copied.

                dta.MultichannelAudioAttributes[x].Info = new DvdMUAMixingInfo[8];

                for (int y = 0; y < 8; y++)
                {
                    // Copy in the value, and advance the pointer
                    dta.MultichannelAudioAttributes[x].Info[y] =
                        (DvdMUAMixingInfo)Marshal.PtrToStructure(pNativeData, typeof(DvdMUAMixingInfo));
                    pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(DvdMUAMixingInfo)));
                }

                dta.MultichannelAudioAttributes[x].Coeff = new DvdMUACoeff[8];

                for (int y = 0; y < 8; y++)
                {
                    // Copy in the value, and advance the pointer
                    dta.MultichannelAudioAttributes[x].Coeff[y] =
                        (DvdMUACoeff)Marshal.PtrToStructure(pNativeData, typeof(DvdMUACoeff));
                    pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(DvdMUACoeff)));
                }
            }

            // The DvdMultichannelAudioAttributes needs to be 16 byte aligned
            pNativeData = (IntPtr)(pNativeData.ToInt64() + 4);

            // Copy in the value, and advance the pointer
            dta.ulNumberOfSubpictureStreams = Marshal.ReadInt32(pNativeData);
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(int)));

            // Allocate a large enough array to hold all the returned structs.
            dta.SubpictureAttributes = new DvdSubpictureAttributes[32];
            for (int x = 0; x < 32; x++)
            {
                // Copy in the value, and advance the pointer
                dta.SubpictureAttributes[x] =
                    (DvdSubpictureAttributes)Marshal.PtrToStructure(pNativeData, typeof(DvdSubpictureAttributes));
                pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(DvdSubpictureAttributes)));
            }

            // Note that 4 bytes (more alignment) are unused at the end

            return null;
        }

        /// <summary>
        /// The number of bytes to marshal out
        /// </summary>
        /// <returns></returns>
        public override int GetNativeDataSize()
        {
            // This is the actual size of a DvdTitleAttributes structure
            return 3208;
        }

        /// <summary>
        /// This method is called by interop to create the custom marshaler.  The (optional)
        /// cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static ICustomMarshaler GetInstance(string cookie)
        {
            return new DTAMarshaler(cookie);
        }
    }

    /// <summary>
    /// See DTAMarshaler for an explanation of the problem.  This class is for marshaling
    /// a DvdKaraokeAttributes structure.
    /// </summary>
    internal class DKAMarshaler : DsMarshaler
    {
        /// <summary>
        /// Initializes new instance of the <see cref="DKAMarshaler"/> class.
        /// </summary>
        /// <param name="cookie"></param>
        // The constructor.  This is called from GetInstance (below)
        public DKAMarshaler(string cookie)
            : base(cookie)
        {
        }

        /// <summary>
        /// Called just after invoking the COM method.  The IntPtr is the same one that just got returned
        /// from MarshalManagedToNative.  The return value is unused.
        /// </summary>
        /// <param name="pNativeData"></param>
        /// <returns></returns>
        public override object MarshalNativeToManaged(IntPtr pNativeData)
        {
            DvdKaraokeAttributes dka = m_obj as DvdKaraokeAttributes;

            // Copy in the value, and advance the pointer
            dka.bVersion = (byte)Marshal.ReadByte(pNativeData);
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(byte)));

            // DWORD Align
            pNativeData = (IntPtr)(pNativeData.ToInt64() + 3);

            // Copy in the value, and advance the pointer
            dka.fMasterOfCeremoniesInGuideVocal1 = Marshal.ReadInt32(pNativeData) != 0;
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(bool)));

            // Copy in the value, and advance the pointer
            dka.fDuet = Marshal.ReadInt32(pNativeData) != 0;
            pNativeData = (IntPtr)(pNativeData.ToInt64() + Marshal.SizeOf(typeof(bool)));

            // Copy in the value, and advance the pointer
            dka.ChannelAssignment = (DvdKaraokeAssignment)Marshal.ReadInt32(pNativeData);
            pNativeData = (IntPtr)(pNativeData.ToInt64()
                                   + Marshal.SizeOf(
                                       DvdKaraokeAssignment.GetUnderlyingType(typeof(DvdKaraokeAssignment))));

            // Allocate a large enough array to hold all the returned structs.
            dka.wChannelContents = new DvdKaraokeContents[8];
            for (int x = 0; x < 8; x++)
            {
                // Copy in the value, and advance the pointer
                dka.wChannelContents[x] = (DvdKaraokeContents)Marshal.ReadInt16(pNativeData);
                pNativeData = (IntPtr)(pNativeData.ToInt64()
                                       + Marshal.SizeOf(
                                           DvdKaraokeContents.GetUnderlyingType(typeof(DvdKaraokeContents))));
            }

            return null;
        }

        /// <summary>
        /// The number of bytes to marshal out
        /// </summary>
        /// <returns></returns>
        public override int GetNativeDataSize()
        {
            // This is the actual size of a DvdKaraokeAttributes structure.
            return 32;
        }

        /// <summary>
        /// This method is called by interop to create the custom marshaler.  The (optional)
        /// cookie is the value specified in MarshalCookie="asdf", or "" is none is specified.
        /// </summary>
        /// <param name="cookie"></param>
        /// <returns></returns>
        public static ICustomMarshaler GetInstance(string cookie)
        {
            return new DKAMarshaler(cookie);
        }
    }

    #endregion
}