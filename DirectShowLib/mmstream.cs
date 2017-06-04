#region license

/*
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
*/

#endregion

using System;
using System.Runtime.InteropServices;
using System.Security;

using JetBrains.Annotations;

namespace DirectShowLib.MultimediaStreaming
{
    #region Utility classes

    public sealed class MsResults
    {
        private MsResults()
        {
            // Prevent people from trying to instantiate this class
        }

        public const int S_Pending                  = unchecked((int)0x00040001);
        public const int S_NoUpdate                 = unchecked((int)0x00040002);
        public const int S_EndOfStream              = unchecked((int)0x00040003);
        public const int E_SampleAlloc              = unchecked((int)0x80040401);
        public const int E_PurposeId                = unchecked((int)0x80040402);
        public const int E_NoStream                 = unchecked((int)0x80040403);
        public const int E_NoSeeking                = unchecked((int)0x80040404);
        public const int E_Incompatible             = unchecked((int)0x80040405);
        public const int E_Busy                     = unchecked((int)0x80040406);
        public const int E_NotInit                  = unchecked((int)0x80040407);
        public const int E_SourceAlreadyDefined     = unchecked((int)0x80040408);
        public const int E_InvalidStreamType        = unchecked((int)0x80040409);
        public const int E_NotRunning               = unchecked((int)0x8004040a);
    }

    public sealed class MsError
    {
        private MsError()
        {
            // Prevent people from trying to instantiate this class
        }

        public static string GetErrorText(int hr)
        {
            string sRet = null;

            switch(hr)
            {
                case MsResults.S_Pending:
                    sRet = "Sample update is not yet complete.";
                    break;
                case MsResults.S_NoUpdate:
                    sRet = "Sample was not updated after forced completion.";
                    break;
                case MsResults.S_EndOfStream:
                    sRet = "End of stream. Sample not updated.";
                    break;
                case MsResults.E_SampleAlloc:
                    sRet = "An IMediaStream object could not be removed from an IMultiMediaStream object because it still contains at least one allocated sample.";
                    break;
                case MsResults.E_PurposeId:
                    sRet = "The specified purpose ID can't be used for the call.";
                    break;
                case MsResults.E_NoStream:
                    sRet = "No stream can be found with the specified attributes.";
                    break;
                case MsResults.E_NoSeeking:
                    sRet = "Seeking not supported for this IMultiMediaStream object.";
                    break;
                case MsResults.E_Incompatible:
                    sRet = "The stream formats are not compatible.";
                    break;
                case MsResults.E_Busy:
                    sRet = "The sample is busy.";
                    break;
                case MsResults.E_NotInit:
                    sRet = "The object can't accept the call because its initialize function or equivalent has not been called.";
                    break;
                case MsResults.E_SourceAlreadyDefined:
                    sRet = "Source already defined.";
                    break;
                case MsResults.E_InvalidStreamType:
                    sRet = "The stream type is not valid for this operation.";
                    break;
                case MsResults.E_NotRunning:
                    sRet = "The IMultiMediaStream object is not in running state.";
                    break;
                default:
                    sRet = DsError.GetErrorText(hr);
                    break;
            }

            return sRet;
        }

        /// <summary>
        /// If hr has a "failed" status code (E_*), throw an exception.  Note that status
        /// messages (S_*) are not considered failure codes.  If DES or DShow error text
        /// is available, it is used to build the exception, otherwise a generic com error
        /// is thrown.
        /// </summary>
        /// <param name="hr">The HRESULT to check</param>
        public static void ThrowExceptionForHR(int hr)
        {
            // If an error has occurred
            if (hr < 0)
            {
                // If a string is returned, build a com error from it
                string buf = GetErrorText(hr);

                if (buf != null)
                {
                    throw new COMException(buf, hr);
                }
                else
                {
                    // No string, just use standard com error
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
        }
    }

    #endregion

    #region Classes

    /// <summary>
    /// From CLSID_AMMultiMediaStream
    /// </summary>
    [ComImport, Guid("49c47ce5-9ba4-11d0-8212-00c04fc32c45")]
    public class AMMultiMediaStream
    {
    }

    /// <summary>
    /// From CLSID_AMMediaTypeStream
    /// </summary>
    [ComImport, Guid("CF0F2F7C-F7BF-11d0-900D-00C04FD9189D")]
    public class AMMediaTypeStream
    {
    }

    /// <summary>
    /// From CLSID_AMDirectDrawStream
    /// </summary>
    [ComImport, Guid("49c47ce4-9ba4-11d0-8212-00c04fc32c45")]
    public class AMDirectDrawStream
    {
    }

    /// <summary>
    /// From CLSID_AMAudioStream
    /// </summary>
    [ComImport, Guid("8496e040-af4c-11d0-8212-00c04fc32c45")]
    public class AMAudioStream
    {
    }

    /// <summary>
    /// From CLSID_AMAudioData
    /// </summary>
    [ComImport, Guid("f2468580-af8a-11d0-8212-00c04fc32c45")]
    public class AMAudioData
    {
    }

    #endregion

    #region Declarations

    /// <summary>
    /// From COMPLETION_STATUS_FLAGS
    /// </summary>
    [Flags]
    [PublicAPI]
    public enum CompletionStatusFlags
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Force the update to complete as soon as possible, even if the sample update isn't yet complete. 
        /// </summary>
        NoUpdateOk = 0x1,

        /// <summary>
        /// Wait until the sample finishes updating before returning from this method.
        /// </summary>
        Wait = 0x2,

        /// <summary>
        /// Forces the update to complete, even if it's currently updating.
        /// </summary>
        Abort = 0x4
    }

    /// <summary>
    /// From unnamed enum
    /// </summary>
    [Flags]
    [PublicAPI]
    public enum SSUpdate
    {
        /// <summary>
        /// None
        /// </summary>
        None = 0x0,

        /// <summary>
        /// Specifies an asynchronous update
        /// </summary>
        Async = 0x1,

        /// <summary>
        /// Continuously update the sample
        /// </summary>
        Continuous = 0x2
    }

    /// <summary>
    /// From STREAM_STATE
    /// </summary>
    [PublicAPI]
    public enum StreamState
    {
        /// <summary>
        /// Stream is running
        /// </summary>
        Run = 1,

        /// <summary>
        /// Stream is stopped
        /// </summary>
        Stop = 0
    }

    /// <summary>
    /// From STREAM_TYPE
    /// </summary>
    [PublicAPI]
    public enum StreamType
    {
        /// <summary>
        /// Stream can read
        /// </summary>
        Read = 0,

        /// <summary>
        /// Stream can transform
        /// </summary>
        Transform = 2,

        /// <summary>
        /// Stream can write
        /// </summary>
        Write = 1
    }

    /// <summary>
    /// From unnamed enum
    /// </summary>
    [PublicAPI]
    public enum MMSSF
    {
        /// <summary>
        /// 
        /// </summary>
        HasClock = 0x1,

        /// <summary>
        /// 
        /// </summary>
        SupportSeek = 0x2,

        /// <summary>
        /// 
        /// </summary>
        Asynchronous = 0x4
    }

    #endregion

    #region GUIDS

    public sealed class MSPID
    {
        private MSPID()
        {
            // Prevent people from trying to instantiate this class
        }

        /// <summary> MSPID_PrimaryVideo </summary>
        public static readonly Guid PrimaryVideo = new Guid(0xa35ff56a, 0x9fda, 0x11d0, 0x8f, 0xdf, 0x0, 0xc0, 0x4f, 0xd9, 0x18, 0x9d);

        /// <summary> MSPID_PrimaryAudio </summary>
        public static readonly Guid PrimaryAudio = new Guid(0xa35ff56b, 0x9fda, 0x11d0, 0x8f, 0xdf, 0x0, 0xc0, 0x4f, 0xd9, 0x18, 0x9d);
    }

    #endregion

    #region Interfaces

    /// <summary>
    /// The IMediaStream interface provides access to the characteristics of a media stream, such as the stream's media type and purpose ID. 
    /// It also has methods that create data samples.
    /// </summary>
    [ComImport, SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("B502D1BD-9A57-11D0-8FDE-00C04FD9189D")]
    public interface IMediaStream
    {
        /// <summary>
        /// Retrieves a pointer to the multimedia stream that contains the specified media stream.
        /// </summary>
        /// <param name="ppMultiMediaStream">Address of a pointer to an <see cref="IMultiMediaStream"/> interface object that will point to 
        /// the multimedia stream from which the current media stream was created.</param>
        /// <returns>Returns S_OK if successful or E_POINTER if ppMultiMediaStream is invalid.</returns>
        [PreserveSig]
        int GetMultiMediaStream(
            [MarshalAs(UnmanagedType.Interface)] out IMultiMediaStream ppMultiMediaStream
            );

        /// <summary>
        /// Retrieves the stream's purpose ID and media type.
        /// </summary>
        /// <param name="pPurposeId">Pointer to an <see cref="Guid"/> value that will contain the stream's purpose ID 
        /// (see Multimedia Streaming Data Types).</param>
        /// <param name="pType">Pointer to a <see cref="StreamType"/> enumerated data type value that will contain the stream's media type.</param>
        /// <returns>Returns S_OK if successful or E_POINTER if one of the parameters is invalid.</returns>
        [PreserveSig]
        int GetInformation(
            out Guid pPurposeId,
            out StreamType pType
            );

        /// <summary>
        /// Sets the media stream to the same format as a previous stream.
        /// </summary>
        /// <param name="pStreamThatHasDesiredFormat">Reference to a media stream object that has the same format.</param>
        /// <param name="dwFlags">Reserved for flag data. Must be zero.</param>
        /// <returns></returns>
        [PreserveSig]
        int SetSameFormat(
            [In, MarshalAs(UnmanagedType.Interface)] IMediaStream pStreamThatHasDesiredFormat,
            [In] int dwFlags
            );

        /// <summary>
        /// Allocates a new stream sample object for the current media stream.
        /// </summary>
        /// <param name="dwFlags">Flags. Must be zero.</param>
        /// <param name="ppSample">Address of a pointer to the newly created stream sample's <see cref="IStreamSample"/> interface.</param>
        /// <returns></returns>
        [PreserveSig]
        int AllocateSample(
            [In] int dwFlags,
            [MarshalAs(UnmanagedType.Interface)] out IStreamSample ppSample
            );

        /// <summary>
        /// Creates a new stream sample that shares the same backing object as the existing sample.
        /// </summary>
        /// <param name="pExistingSample">Pointer to the existing sample.</param>
        /// <param name="dwFlags">Reserved for flag data. Must be zero.</param>
        /// <param name="ppNewSample">Address of a pointer to an <see cref="IStreamSample"/> interface that will point to the newly created shared sample.</param>
        /// <returns></returns>
        [PreserveSig]
        int CreateSharedSample(
            [In, MarshalAs(UnmanagedType.Interface)] IStreamSample pExistingSample,
            [In] int dwFlags,
            [MarshalAs(UnmanagedType.Interface)] out IStreamSample ppNewSample
            );

        /// <summary>
        /// Forces the current stream to end. If the current stream isn't writable, this method does nothing.
        /// </summary>
        /// <param name="dwFlags">Reserved for flag data. Must be zero.</param>
        /// <returns>Returns S_OK if successful or MS_E_INCOMPATIBLE if the existing sample isn't compatible with the current media stream.</returns>
        [PreserveSig]
        int SendEndOfStream(
            int dwFlags
            );
    }

    /// <summary>
    /// It contains methods for enumerating the media streams, retrieving information about them, and running and stopping them.
    /// </summary>
    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("B502D1BC-9A57-11D0-8FDE-00C04FD9189D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMultiMediaStream
    {
        /// <summary>
        /// Retrieves the capabilities of the multimedia stream object.
        /// </summary>
        /// <param name="pdwFlags">Pointer to a variable that receives a bitwise combination of the following flags.
        /// <list type="bulet">
        /// <item><description>MMSSF_ASYNCHRONOUS - The object supports asynchronous sample updates. This flag is always returned.</description></item>
        /// <item><description>MMSSF_HASCLOCK - The object has a clock.</description></item>
        /// <item><description>MMSSF_SUPPORTSEEK - The object supports seeking.</description></item>
        /// </list>
        /// </param>
        /// <param name="pStreamType">Pointer to a variable that receives a member of the <see cref="StreamType"/> enumeration. This value indicates whether the multimedia stream is read-only, write-only, or read/write.</param>
        /// <returns>Returns an HRESULT value.</returns>
        [PreserveSig]
        int GetInformation(
            out MMSSF pdwFlags,
            out StreamType pStreamType
            );

        /// <summary>
        /// Retrieves a media stream, specified by purpose ID.
        /// </summary>
        /// <param name="idPurpose">Reference to an <see cref="Guid"/> that identifies the media stream to retrieve.</param>
        /// <param name="ppMediaStream">Address of variable that receives an <see cref="IMediaStream"/> interface pointer.</param>
        /// <returns></returns>
        [PreserveSig]
        int GetMediaStream(
            [In, MarshalAs(UnmanagedType.LPStruct)] Guid idPurpose,
            [MarshalAs(UnmanagedType.Interface)] out IMediaStream ppMediaStream
            );

        /// <summary>
        /// Retrieves a media stream object, specified by index.
        /// </summary>
        /// <param name="index">Zero-based index of the media stream to retrieve.</param>
        /// <param name="ppMediaStream">Address of a variable that receives an <see cref="IMediaStream"/> interface pointer.</param>
        /// <returns></returns>
        [PreserveSig]
        int EnumMediaStreams(
            [In] int index,
            [MarshalAs(UnmanagedType.Interface)] out IMediaStream ppMediaStream
            );

        /// <summary>
        /// Retrieves the current state of the multimedia stream object.
        /// </summary>
        /// <param name="pCurrentState">Pointer to a variable that receives a member of the <see cref="StreamState"/> enumeration.</param>
        /// <returns>Returns an HRESULT value.</returns>
        [PreserveSig]
        int GetState(
            out StreamState pCurrentState
            );

        /// <summary>
        /// Runs or stops the multimedia stream object.
        /// </summary>
        /// <param name="newState">A member of the <see cref="StreamState"/> enumeration, specifying the new state (running or stopped).</param>
        /// <returns>Returns an HRESULT value.</returns>
        [PreserveSig]
        int SetState(
            [In] StreamState newState
            );

        /// <summary>
        /// Retrieves the current stream time.
        /// </summary>
        /// <param name="pCurrentTime">Pointer to a variable that receives the stream time, in 100-nanosecond units.</param>
        /// <returns>Returns an HRESULT value.</returns>
        [PreserveSig]
        int GetTime(
            out long pCurrentTime
            );

        /// <summary>
        /// Retrieves the duration of the multimedia stream.
        /// </summary>
        /// <param name="pDuration">Pointer to a variable that receives of the multimedia stream, in 100-nanosecond units.</param>
        /// <returns>Returns an HRESULT value. Possible values include the following.
        /// <list type="bullet">
        /// <item><description>E_NOINTERFACE - This multimedia stream does not support seeking.</description></item>
        /// <item><description>E_POINTER - <b>null</b> pointer value.</description></item>
        /// <item><description>MS_E_INVALIDSTREAMTYPE - This multimedia stream is not read-only.</description></item>
        /// <item><description>S_FALSE - Could not determine the duration.</description></item>
        /// <item><description>S_OK - Success.</description></item>
        /// </list>
        /// </returns>
        [PreserveSig]
        int GetDuration(
            out long pDuration
            );

        /// <summary>
        /// Seeks all of the media streams to a new position.
        /// </summary>
        /// <param name="seekTime">The value that specifies the new position.</param>
        /// <returns>Returns an HRESULT value.</returns>
        [PreserveSig]
        int Seek(
            [In] long seekTime
            );

        /// <summary>
        /// Retrieves an event that is signaled when the multimedia stream completes playback.
        /// </summary>
        /// <param name="phEos">Pointer to a variable that receives a handle to the event. The event is signaled when all of the streams in the multimedia stream object complete playback.</param>
        /// <returns>Returns an HRESULT value.</returns>
        [PreserveSig]
        int GetEndOfStreamEventHandle(
            out IntPtr phEos
            );
    }

    /// <summary>
    /// Provides control over the behavior of stream samples. You can retrieve the media stream that created the sample, set or 
    /// retrieve sample start and stop times, check the sample's completion status, and perform a developer-specified function on the 
    /// sample itself.
    /// </summary>
    [ComImport, SuppressUnmanagedCodeSecurity,
    Guid("B502D1BE-9A57-11D0-8FDE-00C04FD9189D"),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IStreamSample
    {
        /// <summary>
        /// Retrieves a pointer to the media stream object that created the current sample.
        /// </summary>
        /// <param name="ppMediaStream">Address of a pointer to an <see cref="IMediaStream"/> interface that will point to the media stream that created the current sample.</param>
        /// <returns>Returns S_OK if successful or E_POINTER if <paramref name="ppMediaStream"/> is invalid.</returns>
        [PreserveSig]
        int GetMediaStream(
            [MarshalAs(UnmanagedType.Interface)] out IMediaStream ppMediaStream
            );

        /// <summary>
        /// Retrieves the current sample's start and end times. If the sample is updating, this method returns the times after the 
        /// update completes.
        /// </summary>
        /// <param name="pStartTime">Pointer to a STREAM_TIME value that will contain the sample's start time.</param>
        /// <param name="pEndTime">Pointer to a STREAM_TIME value that will contain the sample's end time.</param>
        /// <param name="pCurrentTime">Pointer to a STREAM_TIMEvalue that will contain the media stream's current media time.</param>
        /// <returns>Returns S_OK if successful or E_POINTER if one of the parameters is invalid.</returns>
        [PreserveSig]
        int GetSampleTimes(
            out long pStartTime,
            out long pEndTime,
            out long pCurrentTime
            );

        /// <summary>
        /// Sets the current sample's start and end times. You can call this method prior to updating the sample.
        /// </summary>
        /// <param name="pStartTime">Pointer to a STREAM_TIME value that contains the sample's new start time.</param>
        /// <param name="pEndTime">Pointer to a STREAM_TIME value that contains the sample's new end time.</param>
        /// <returns>Returns S_OK if successful or E_POINTER if one of the parameters is NULL.</returns>
        [PreserveSig]
        int SetSampleTimes(
            [In] DsLong pStartTime,
            [In] DsLong pEndTime
            );

        /// <summary>
        /// Performs a synchronous or an asynchronous update on the current sample.
        /// </summary>
        /// <param name="dwFlags">Flag that specifies whether the update is synchronous or asynchronous. The <see cref="SSUpdate.Async"/> 
        /// flag specifies an asynchronous update, which you can set if both hEvent and pfnAPC are NULL. Use <see cref="SSUpdate.Continuous"/> 
        /// to continuously update the sample until you call the <see cref="CompletionStatus"/> method.</param>
        /// <param name="hEvent">Handle to an event that this method will trigger when the update is complete.</param>
        /// <param name="pfnApc">Pointer to a Win32 asynchronous procedure call (APC) function that this method will call after it completes the sample update.</param>
        /// <param name="dwApcData">Value that this method passes to the function specified by the pfnAPC parameter.</param>
        /// <returns>Returns an HRESULT value.</returns>
        [PreserveSig]
        int Update(
            [In] SSUpdate dwFlags,
            [In] IntPtr hEvent,
            [In] IntPtr pfnApc,
            [In] IntPtr dwApcData
            );

        /// <summary>
        /// Retrieves the status of the current sample's latest asynchronous update. If the update isn't complete, you can force it to complete.
        /// </summary>
        /// <param name="dwFlags">Value that specifies whether to forcibly complete the update. This value is a combination of one or more of the following flags.
        /// <list type="bullet">
        /// <item><description><see cref="CompletionStatusFlags.NoUpdateOk"/> - Force the update to complete as soon as possible, even if the sample update 
        /// isn't yet complete. If the sample is updating and you didn't set the COMPSTAT_WAIT flag, the method returns MS_S_PENDING. 
        /// If the sample is waiting to be updated, this method removes it from the queue and returns MS_S_NOTUPDATED.</description></item>
        /// <item><description><see cref="CompletionStatusFlags.Wait"/> - Wait until the sample finishes updating before returning from this method.</description></item>
        /// <item><description><see cref="CompletionStatusFlags.Abort"/> - Forces the update to complete, even if it's currently updating. This leaves the sample data in an undefined state. Combine this value with the COMPSTAT_WAITFORCOMPLETION flag to ensure that the update canceled.</description></item>
        /// </list>
        /// </param>
        /// <param name="dwMilliseconds">If the <paramref name="dwFlags"/> parameter is <see cref="CompletionStatusFlags.Wait"/>, 
        /// this value is the number of milliseconds to wait for the update to complete. Specify INFINITE to indicate that you want to 
        /// wait until the sample updates before this call returns.</param>
        /// <returns>Returns an HRESULT value.</returns>
        [PreserveSig]
        int CompletionStatus(
            [In] CompletionStatusFlags dwFlags,
            [In] int dwMilliseconds
            );
    }

    #endregion
}
