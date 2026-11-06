//------------------------------------------------------------------------------
// <copyright file="SafeNativeMethods.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------    

namespace System.Messaging.Interop
{
    using System.Text;
    using System.Threading;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using Microsoft.Win32;
    using System.Security;
    using System.Security.Permissions;
    using System.Runtime.ConstrainedExecution; //for ReliabilityContract
    using System.Messaging.Store; // messagingForCore: the queue store

    [System.Runtime.InteropServices.ComVisible(false),
    System.Security.SuppressUnmanagedCodeSecurityAttribute()]
    internal static class SafeNativeMethods
    {
        public unsafe delegate void ReceiveCallback(int result, IntPtr handle, int timeout, int action, IntPtr propertiesPointer, NativeOverlapped* overlappedPointer, IntPtr cursorHandle);

        // messagingForCore: MSMQ's runtime (mqrt.dll) is replaced by messagingForCore's queue store (see
        // UnsafeNativeMethods), here and in every MQ function below.
        public static int MQBeginTransaction(out ITransaction refTransaction)
        {
            return QueueStore.BeginTransaction(out refTransaction);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static int MQCloseQueue(IntPtr handle)
        {
            return QueueStore.CloseQueue(handle);
        }

        public static int MQPathNameToFormatName(string pathName, StringBuilder formatName, ref int count)
        {
            return QueueStore.PathNameToFormatName(pathName, formatName, ref count);
        }

        public static int MQInstanceToFormatName(byte[] id, StringBuilder formatName, ref int count)
        {
            return QueueStore.InstanceToFormatName();
        }

        public static int MQCreateCursor(MessageQueueHandle handle, out CursorHandle cursorHandle)
        {
            return QueueStore.CreateCursor(handle, out cursorHandle);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static int MQCloseCursor(IntPtr cursorHandle)
        {
            return QueueStore.CloseCursor(cursorHandle);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static void MQFreeSecurityContext(IntPtr handle)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public static int MQLocateEnd(IntPtr enumHandle)
        {
            return 0;
        }

        public static int MQLocateNext(LocatorHandle enumHandle, ref int propertyCount, [Out] MQPROPVARIANTS[] variantArray)
        {
            return QueueStore.LocateNext();
        }

        public static void MQFreeMemory(IntPtr memory)
        {
            QueueStore.FreeMemory(memory);
        }

        // messagingForCore: the store's queue handles aren't file handles, so asynchronous receives take MSMQ's callback
        // path rather than the I/O completion port (ThreadPool.BindHandle and overlapped I/O, Windows only).
        public static bool GetHandleInformation(SafeHandle handle, out int handleInformation)
        {
            handleInformation = 0;
            return false;
        }

        [DllImport(ExternDll.Kernel32)]
        public static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport(ExternDll.Advapi32)]
        public static extern int SetEntriesInAclW(int count,
            //[MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 0, ArraySubType = ExplicitAccess)]
            //ExplicitAccess[] entries,
                                                  IntPtr entries,
                                                  IntPtr oldacl,
                                                  out IntPtr newAcl);

        // messagingForCore: the computer's name from .NET, on every platform (Kernel32's GetComputerName is Windows only).
        public static bool GetComputerName(StringBuilder lpBuffer, int[] nSize)
        {
            lpBuffer.Clear().Append(Environment.MachineName);
            return true;
        }

        public const int FORMAT_MESSAGE_ALLOCATE_BUFFER = 0x00000100,
            FORMAT_MESSAGE_IGNORE_INSERTS = 0x00000200,
            FORMAT_MESSAGE_FROM_STRING = 0x00000400,
            FORMAT_MESSAGE_FROM_HMODULE = 0x00000800,
            FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000,
            FORMAT_MESSAGE_ARGUMENT_ARRAY = 0x00002000,
            FORMAT_MESSAGE_MAX_WIDTH_MASK = 0x000000FF;

        // messagingForCore: no system message table (Windows only): 0, "not found", so MessageQueueException uses its own
        // "UnknownError" text for a code that has no string.
        public static int FormatMessage(int dwFlags, IntPtr lpSource, int dwMessageId,
                                                int dwLanguageId, StringBuilder lpBuffer, int nSize, IntPtr arguments)
        {
            return 0;
        }

    }
}
