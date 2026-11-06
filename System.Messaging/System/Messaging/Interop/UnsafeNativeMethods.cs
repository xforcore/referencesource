//------------------------------------------------------------------------------
// <copyright file="UnsafeNativeMethods.cs" company="Microsoft">
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
    using System.Messaging.Store; // messagingForCore: the queue store

    [System.Runtime.InteropServices.ComVisible(false),
    System.Security.SuppressUnmanagedCodeSecurityAttribute()]
    internal static class UnsafeNativeMethods
    {

        // messagingForCore: MSMQ's runtime (mqrt.dll) is replaced by messagingForCore's queue store, which keeps local
        // private queues in a SQLite database (System.Messaging.Store, messagingForCore's own code). Each MQ function
        // below calls it with the arguments it gave MSMQ; the store returns MSMQ's status codes, fills the property
        // buffers as MSMQ did, and names what it doesn't support in a MessageQueueException. Asynchronous receives take
        // MSMQ's callback path (SafeNativeMethods.GetHandleInformation), so the overlapped pointer is always null.
        public static int MQOpenQueue(string formatName, int access, int shareMode, out MessageQueueHandle handle)
        {
            return QueueStore.OpenQueue(formatName, access, shareMode, out handle);
        }

        public static int MQSendMessage(MessageQueueHandle handle, MessagePropertyVariants.MQPROPS properties, IntPtr transaction)
        {
            return QueueStore.SendMessage(handle, properties, transaction, null);
        }

        public static int MQSendMessage(MessageQueueHandle handle, MessagePropertyVariants.MQPROPS properties, ITransaction transaction)
        {
            return QueueStore.SendMessage(handle, properties, IntPtr.Zero, transaction);
        }

        public unsafe static int MQReceiveMessage(MessageQueueHandle handle, uint timeout, int action, MessagePropertyVariants.MQPROPS properties, NativeOverlapped* overlapped,
                                                                                                     SafeNativeMethods.ReceiveCallback receiveCallback, CursorHandle cursorHandle, IntPtr transaction)
        {
            return QueueStore.ReceiveMessage(handle, timeout, action, properties, Completion(receiveCallback, timeout, action), cursorHandle, transaction, null);
        }

        public unsafe static int MQReceiveMessage(MessageQueueHandle handle, uint timeout, int action, MessagePropertyVariants.MQPROPS properties, NativeOverlapped* overlapped,
                                                                                                    SafeNativeMethods.ReceiveCallback receiveCallback, CursorHandle cursorHandle, ITransaction transaction)
        {
            return QueueStore.ReceiveMessage(handle, timeout, action, properties, Completion(receiveCallback, timeout, action), cursorHandle, IntPtr.Zero, transaction);
        }

        public unsafe static int MQReceiveMessageByLookupId(MessageQueueHandle handle, long lookupId, int action, MessagePropertyVariants.MQPROPS properties, NativeOverlapped* overlapped,
                                                     SafeNativeMethods.ReceiveCallback receiveCallback, IntPtr transaction)
        {
            return QueueStore.ReceiveMessageByLookupId(handle, lookupId, action, properties, Completion(receiveCallback, 0, action), transaction, null);
        }

        public unsafe static int MQReceiveMessageByLookupId(MessageQueueHandle handle, long lookupId, int action, MessagePropertyVariants.MQPROPS properties, NativeOverlapped* overlapped,
                                                     SafeNativeMethods.ReceiveCallback receiveCallback, ITransaction transaction)
        {
            return QueueStore.ReceiveMessageByLookupId(handle, lookupId, action, properties, Completion(receiveCallback, 0, action), IntPtr.Zero, transaction);
        }

        // messagingForCore: MSMQ's completion callback, as the store calls it when an asynchronous receive completes.
        private unsafe static Action<int> Completion(SafeNativeMethods.ReceiveCallback receiveCallback, uint timeout, int action)
        {
            if (receiveCallback == null)
                return null;
            return result => receiveCallback(result, IntPtr.Zero, (int)timeout, action, IntPtr.Zero, null, IntPtr.Zero);
        }

        public static int MQCreateQueue(IntPtr securityDescriptor, MessagePropertyVariants.MQPROPS queueProperties, StringBuilder formatName, ref int formatNameLength)
        {
            return QueueStore.CreateQueue(securityDescriptor, queueProperties, formatName, ref formatNameLength);
        }

        public static int MQDeleteQueue(string formatName)
        {
            return QueueStore.DeleteQueue(formatName);
        }

        public static int MQLocateBegin(string context, Restrictions.MQRESTRICTION Restriction, Columns.MQCOLUMNSET columnSet, out LocatorHandle enumHandle)
        {
            return QueueStore.LocateBegin(out enumHandle);
        }

        public static int MQGetMachineProperties(string machineName, IntPtr machineIdPointer, MessagePropertyVariants.MQPROPS machineProperties)
        {
            return QueueStore.GetMachineProperties(machineName, machineIdPointer, machineProperties);
        }

        public static int MQGetQueueProperties(string formatName, MessagePropertyVariants.MQPROPS queueProperties)
        {
            return QueueStore.GetQueueProperties(formatName, queueProperties);
        }

        public static int MQMgmtGetInfo(string machineName, string objectName, MessagePropertyVariants.MQPROPS queueProperties)
        {
            return QueueStore.MgmtGetInfo(machineName, objectName, queueProperties);
        }

        public static int MQPurgeQueue(MessageQueueHandle handle)
        {
            return QueueStore.PurgeQueue(handle);
        }

        public static int MQSetQueueProperties(string formatName, MessagePropertyVariants.MQPROPS queueProperties)
        {
            return QueueStore.SetQueueProperties(formatName, queueProperties);
        }

        // This method gets us the current security descriptor In "self-relative" format - so it contains offsets instead of pointers,
        // and we don't know how big the return buffer is, so we just use an IntPtr parameter
        // messagingForCore: queue security isn't supported (the store throws, naming it), so the Advapi32 calls below
        // are never reached.
        public static int MQGetQueueSecurity(string formatName, int SecurityInformation, IntPtr SecurityDescriptor, int length, out int lengthNeeded)
        {
            return QueueStore.GetQueueSecurity(out lengthNeeded);
        }

        // This method takes a security descriptor In "absolute" formate - so it will always be the same size and
        // we can just use the SECURITY_DESCRIPTOR class.
        public static int MQSetQueueSecurity(string formatName, int SecurityInformation, NativeMethods.SECURITY_DESCRIPTOR SecurityDescriptor)
        {
            return QueueStore.SetQueueSecurity();
        }

        [DllImport(ExternDll.Advapi32, SetLastError = true)]
        public static extern bool GetSecurityDescriptorDacl(IntPtr pSD, out bool daclPresent, out IntPtr pDacl, out bool daclDefaulted);

        [DllImport(ExternDll.Advapi32, SetLastError = true)]
        public static extern bool SetSecurityDescriptorDacl(NativeMethods.SECURITY_DESCRIPTOR pSD, bool daclPresent, IntPtr pDacl, bool daclDefaulted);

        [DllImport(ExternDll.Advapi32, SetLastError = true)]
        public static extern bool InitializeSecurityDescriptor(NativeMethods.SECURITY_DESCRIPTOR SD, int revision);

        [DllImport(ExternDll.Advapi32, CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        public static extern bool LookupAccountName(string lpSystemName,
                                                     string lpAccountName,
                                                     IntPtr sid,
                                                     ref int sidSize,
                                                     StringBuilder DomainName,
                                                     ref int DomainSize,
                                                     out int pUse);
    }
}
