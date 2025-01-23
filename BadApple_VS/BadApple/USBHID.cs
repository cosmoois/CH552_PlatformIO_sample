using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Rikka
{
    public class USBHID
    {
        #region NativeFunctions
        [DllImport("hid.dll")]
        public static extern void HidD_GetHidGuid(out Guid HidGuid);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidP_GetCaps(IntPtr pPHIDP_PREPARSED_DATA, out HIDP_CAPS myPHIDP_CAPS);

        [DllImport("hid.dll", SetLastError = true)]
        public static extern int HidD_GetPreparsedData(IntPtr hObject, out IntPtr pPHIDP_PREPARSED_DATA);

        [DllImport("hid.dll")]
        public static extern bool HidD_FreePreparsedData(IntPtr PreparsedData);





        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(Guid ClassGuid, uint Enumerator, IntPtr HwndParent, DIGCF Flags);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("setupapi.dll", SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces(IntPtr hDevInfo, IntPtr devInfo, Guid interfaceClassGuid, int memberIndex,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetDeviceRegistryPropertyW")]
        public static extern bool SetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, SPDRP Property,
            [Out]IntPtr PropertyRegDataType, [Out]IntPtr PropertyBuffer, int PropertyBufferSize, out int RequiredSize);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetDeviceInterfaceDetailW")]
        public static extern bool SetupDiGetDeviceInterfaceDetail(IntPtr deviceInfoSet, SP_DEVICE_INTERFACE_DATA deviceInterfaceData, [Out]IntPtr deviceInterfaceDetailData,
             int deviceInterfaceDetailDataSize, out int requiredSize, ref SP_DEVINFO_DATA deviceInfoData);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetupDiGetDevicePropertyW")]
        public static extern bool SetupDiGetDeviceProperty(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, DEVPROPKEY PropertyKey, out int PropertyType,
            [Out]IntPtr PropertyBuffer, int PropertyBufferSize, out int RequiredSize, int Flags);

        [DllImport("setupapi.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "SetupDiOpenDeviceInfoW")]
        public static extern bool SetupDiOpenDeviceInfo(IntPtr DeviceInfoSet, IntPtr DeviceInstanceId, IntPtr hwndParent, int OpenFlags, out SP_DEVINFO_DATA DeviceInfoData);





        [DllImport("CfgMgr32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern int CM_Get_Parent(ref int pdnDevInst, int dnDevInst, int ulFlags);
        [DllImport("CfgMgr32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        public static extern int CM_Get_Device_ID_Size(ref int pulLen, int dnDevInst, int ulFlags);
        [DllImport("CfgMgr32.dll", SetLastError = true, CharSet = CharSet.Unicode, EntryPoint = "CM_Get_Device_IDW")]
        public static extern int CM_Get_Device_ID(int dnDevInst, IntPtr Buffer, int BufferLen, int ulFlags);





        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, uint lpSecurityAttributes,
            uint dwCreationDisposition, uint dwFlagsAndAttributes, uint hTemplateFile);

        [DllImport("kernel32.dll")]
        public static extern int CloseHandle(IntPtr hObject);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern int ReadFile(IntPtr hFile, IntPtr lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        public static extern int WriteFile(IntPtr hFile, IntPtr lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, IntPtr lpOverlapped);


        /// <summary>
        /// Registers a window for device insert/remove messages
        /// </summary>
        /// <param name="hwnd">Handle to the window that will receive the messages</param>
        /// <param name="oInterface">DeviceBroadcastInterrface structure</param>
        /// <param name="nFlags">set to DEVICE_NOTIFY_WINDOW_HANDLE</param>
        /// <returns>A handle used when unregistering</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hwnd, DeviceBroadcastInterface oInterface, uint nFlags);
        /// <summary>
        /// Unregister from above.
        /// </summary>
        /// <param name="hHandle">Handle returned in call to RegisterDeviceNotification</param>
        /// <returns>True if success</returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool UnregisterDeviceNotification(IntPtr hHandle);
        #endregion

        #region Structs
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public int cbSize;
            public Guid interfaceClassGuid;
            public int flags;
            public IntPtr reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public int cbSize;
            public Guid classGuid; // temp
            public int devInst; // dumy
            public IntPtr reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct HIDP_CAPS
        {
            public System.UInt16 Usage;                    // USHORT
            public System.UInt16 UsagePage;                // USHORT
            public System.UInt16 InputReportByteLength;
            public System.UInt16 OutputReportByteLength;
            public System.UInt16 FeatureReportByteLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 17)]
            public System.UInt16[] Reserved;                // USHORT  Reserved[17];            
            public System.UInt16 NumberLinkCollectionNodes;
            public System.UInt16 NumberInputButtonCaps;
            public System.UInt16 NumberInputValueCaps;
            public System.UInt16 NumberInputDataIndices;
            public System.UInt16 NumberOutputButtonCaps;
            public System.UInt16 NumberOutputValueCaps;
            public System.UInt16 NumberOutputDataIndices;
            public System.UInt16 NumberFeatureButtonCaps;
            public System.UInt16 NumberFeatureValueCaps;
            public System.UInt16 NumberFeatureDataIndices;
        }


        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct DEVPROPKEY
        {
            public Guid fmtid;
            public uint pid;
        }

        /// <summary>
        /// Used when registering a window to receive messages about devices added or removed from the system.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 1)]
        public class DeviceBroadcastInterface
        {
            public int Size;
            public int DeviceType;
            public int Reserved;
            public Guid ClassGuid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Name;
        }
        #endregion

        #region Constants
        public enum DIGCF
        {
            DIGCF_DEFAULT = 0x1,
            DIGCF_PRESENT = 0x2,
            DIGCF_ALLCLASSES = 0x4,
            DIGCF_PROFILE = 0x8,
            DIGCF_DEVICEINTERFACE = 0x10
        }

        public enum SPDRP
        {
            SPDRP_DEVICEDESC = 0x00,
            SPDRP_FRIENDLYNAME = 0x0C
        }

        public const uint GENERIC_READ = 0x80000000;
        public const uint GENERIC_WRITE = 0x40000000;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const int OPEN_EXISTING = 3;

        public const int DEVTYP_DEVICEINTERFACE = 0x05;
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        public const int WM_DEVICECHANGE = 0x0219;
        public const int DEVICE_ARRIVAL = 0x8000;
        public const int DEVICE_REMOVECOMPLETE = 0x8004;

        public static DEVPROPKEY DEVPKEY_Device_BusReportedDeviceDesc;

        static USBHID()
        {
            DEVPKEY_Device_BusReportedDeviceDesc = new DEVPROPKEY
            {
                fmtid = new Guid(0x540b947e, 0x8b40, 0x45bc, 0xa8, 0xa2, 0x6a, 0x0b, 0x89, 0x4c, 0xbd, 0xa2),
                pid = 4
            };
        }
        #endregion

        private static Dictionary<string, USBHID> Devices = new Dictionary<string, USBHID>();
        private IntPtr hDev;
        private static int fakeInt = 0;


        public string devicePath { get; private set; }
        public string deviceDesc { get; private set; }
        public string busDescription { get; private set; }
        public int vid { get; private set; }
        public int pid { get; private set; }
        public int mi { get; private set; }

        public int InputReportByteLength { get; private set; }
        public int OutputReportByteLength { get; private set; }

        public string GetDisplayName()
        {
            return deviceDesc + " (" + busDescription + ")";
        }

        public void Open()
        {
            hDev = CreateFile(devicePath, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, 0, OPEN_EXISTING, 0, 0);



            IntPtr myPtrToPreparsedData;
            int result = HidD_GetPreparsedData(hDev, out myPtrToPreparsedData);
            HIDP_CAPS myHIDP_CAPS;
            result = HidP_GetCaps(myPtrToPreparsedData, out myHIDP_CAPS);
            InputReportByteLength = myHIDP_CAPS.InputReportByteLength;
            OutputReportByteLength = myHIDP_CAPS.OutputReportByteLength;
            HidD_FreePreparsedData(myPtrToPreparsedData);
        }

        public void Close()
        {
            CloseHandle(hDev);
        }

        public void Read(byte[] data)
        {
            int bytes = 0;
            IntPtr bufferPtr = Marshal.AllocHGlobal(data.Length + 1);
            ReadFile(hDev, bufferPtr, data.Length + 1, ref bytes, IntPtr.Zero);
            Marshal.Copy((IntPtr)(bufferPtr.ToInt64() + 1), data, 0, data.Length);
            Marshal.FreeHGlobal(bufferPtr);
        }

        public void Write(byte[] data)
        {
            int bytes = 0;
            IntPtr bufferPtr = Marshal.AllocHGlobal(data.Length + 1);
            Marshal.Copy(data, 0, (IntPtr)(bufferPtr.ToInt64() + 1), data.Length);
            int a = WriteFile(hDev, bufferPtr, data.Length + 1, ref bytes, IntPtr.Zero);
            Marshal.FreeHGlobal(bufferPtr);
        }


        #region Static
        public static IntPtr RegisterForUsbEvents(IntPtr hWnd)
        {
            DeviceBroadcastInterface oInterfaceIn = new DeviceBroadcastInterface();
            oInterfaceIn.Size = Marshal.SizeOf(oInterfaceIn);
            oInterfaceIn.ClassGuid = new Guid();
            HidD_GetHidGuid(out oInterfaceIn.ClassGuid);
            oInterfaceIn.DeviceType = DEVTYP_DEVICEINTERFACE;
            oInterfaceIn.Reserved = 0;
            return RegisterDeviceNotification(hWnd, oInterfaceIn, DEVICE_NOTIFY_WINDOW_HANDLE);
        }

        public static USBHID FindHIDDevice(int vid, int pid, int mi = 0)
        {
            RefreshList();

            foreach (USBHID dev in Devices.Values)
            {
                if (dev.vid == vid && dev.pid == pid && dev.mi == mi)
                    return dev;
            }

            return null;
        }

        public static USBHID GetHIDDevice(string devicePath)
        {
            return Devices[devicePath];
        }

        public static ICollection<string> GetDevicePathList()
        {
            return Devices.Keys;
        }

        public static ICollection<USBHID> GetDeviceList()
        {
            return Devices.Values;
        }

        public static void RefreshList()
        {
            Devices.Clear();

            IntPtr hidHandle = IntPtr.Zero;
            Guid hidGuid;

            HidD_GetHidGuid(out hidGuid);

            IntPtr hDevInfo = SetupDiGetClassDevs(hidGuid, 0, IntPtr.Zero, DIGCF.DIGCF_PRESENT | DIGCF.DIGCF_DEVICEINTERFACE);

            if (hDevInfo == IntPtr.Zero)
                throw new Win32Exception();



            SP_DEVICE_INTERFACE_DATA devInterfaceData = new SP_DEVICE_INTERFACE_DATA();
            SP_DEVINFO_DATA devInfo = new SP_DEVINFO_DATA();
            for (int deviceID = 0; ; deviceID++)
            {
                devInterfaceData.cbSize = Marshal.SizeOf(devInterfaceData);

                bool result = SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, hidGuid, deviceID, ref devInterfaceData);
                if (!result)
                {
                    if (Marshal.GetLastWin32Error() == 259L) //ERROR_NO_MORE_ITEMS
                        break;
                    else
                        throw new Win32Exception();
                }


                devInfo.cbSize = Marshal.SizeOf(devInfo);
                int bufferLength = 0;
                //Get required buffer length
                SetupDiGetDeviceInterfaceDetail(hDevInfo, devInterfaceData, IntPtr.Zero, 0, out bufferLength, ref devInfo);

                //Get devicePath
                IntPtr detailDataBuffer = Marshal.AllocHGlobal(bufferLength);
                Marshal.WriteInt32(detailDataBuffer, 8);
                /*
                struct _SP_DEVICE_INTERFACE_DETAIL_DATA_W {
                    DWORD  cbSize;                      //
                    WCHAR  DevicePath[ANYSIZE_ARRAY];   //
                } 
                */
                if (!SetupDiGetDeviceInterfaceDetail(hDevInfo, devInterfaceData, detailDataBuffer, bufferLength, out fakeInt, ref devInfo))
                    throw new Win32Exception();

                // 2022/06/20 修正 「System.OverflowException: '算術演算の結果オーバーフローが発生しました。'」対策
                var strPtr = new IntPtr(detailDataBuffer.ToInt64() + 4);
                string devicePath = Marshal.PtrToStringAuto(strPtr);
                //string devicePath = Marshal.PtrToStringUni((IntPtr)((int)detailDataBuffer + 4));
                Marshal.FreeHGlobal(detailDataBuffer);


                int vidIndex = devicePath.IndexOf("vid_");
                if (vidIndex > 0)
                {
                    //Make sure it's a USB HID device <= has vid

                    USBHID device = new USBHID()
                    {
                        devicePath = devicePath,
                        deviceDesc = GetStringUsingSetupDiGetDeviceRegistryProperty(hDevInfo, devInfo, SPDRP.SPDRP_DEVICEDESC),
                        busDescription = GetDeviceBusDescription(hDevInfo, GetParentDevInfoData(hDevInfo, devInfo)),
                        vid = int.Parse(devicePath.Substring(vidIndex + 4, 4), System.Globalization.NumberStyles.HexNumber),
                        pid = int.Parse(devicePath.Substring(devicePath.IndexOf("pid_") + 4, 4), System.Globalization.NumberStyles.HexNumber),
                    };

                    if (devicePath.Contains("mi_"))
                    {
                        device.mi = int.Parse(devicePath.Substring(devicePath.IndexOf("mi_") + 3, 2), System.Globalization.NumberStyles.HexNumber);
                    }

                    Devices.Add(devicePath, device);
                }
            }

            SetupDiDestroyDeviceInfoList(hDevInfo);
        }
        #endregion


        #region Private functions
        private static SP_DEVINFO_DATA GetParentDevInfoData(IntPtr hDevInfo, SP_DEVINFO_DATA devInfo)
        {
            int bufferLength = 0;
            int devInstParent = 0;
            CM_Get_Parent(ref devInstParent, devInfo.devInst, 0);
            CM_Get_Device_ID_Size(ref bufferLength, devInstParent, 0);
            IntPtr parentDevPathPointer = Marshal.AllocHGlobal(bufferLength * 2 + 2);
            CM_Get_Device_ID(devInstParent, parentDevPathPointer, bufferLength * 2 + 2, 0);

            SP_DEVINFO_DATA ret = new SP_DEVINFO_DATA();
            ret.cbSize = Marshal.SizeOf(typeof(SP_DEVINFO_DATA));
            SetupDiOpenDeviceInfo(hDevInfo, parentDevPathPointer, IntPtr.Zero, 0, out ret);
            Marshal.FreeHGlobal(parentDevPathPointer);
            return ret;
        }

        private static string GetStringUsingSetupDiGetDeviceRegistryProperty(IntPtr DeviceInfoSet, SP_DEVINFO_DATA DeviceInfoData, SPDRP Property)
        {
            int bufferLength;
            SetupDiGetDeviceRegistryProperty(DeviceInfoSet, DeviceInfoData, Property, IntPtr.Zero, IntPtr.Zero, 0, out bufferLength);

            IntPtr stringPointer = Marshal.AllocHGlobal(bufferLength);
            SetupDiGetDeviceRegistryProperty(DeviceInfoSet, DeviceInfoData, Property, IntPtr.Zero, stringPointer, bufferLength, out fakeInt);

            string ret = Marshal.PtrToStringUni(stringPointer);
            Marshal.FreeHGlobal(stringPointer);

            return ret;
        }

        private static string GetDeviceBusDescription(IntPtr hDeviceInfoSet, SP_DEVINFO_DATA deviceInfoData)
        {
            int bufferLength;
            int propRegDataType;
            SetupDiGetDeviceProperty(hDeviceInfoSet, deviceInfoData, DEVPKEY_Device_BusReportedDeviceDesc,
                out propRegDataType, IntPtr.Zero, 0, out bufferLength, 0);

            IntPtr bufPtr = Marshal.AllocHGlobal(bufferLength);
            SetupDiGetDeviceProperty(hDeviceInfoSet, deviceInfoData, DEVPKEY_Device_BusReportedDeviceDesc,
                out propRegDataType, bufPtr, bufferLength, out fakeInt, 0);

            string ret = Marshal.PtrToStringUni(bufPtr);
            Marshal.FreeHGlobal(bufPtr);
            return ret;
        }
        #endregion
    }

}
