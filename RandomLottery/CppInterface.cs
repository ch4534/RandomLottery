using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RandomLottery
{
    /// <summary>
    /// C++动态库接口
    /// </summary>
    class CppInterface
    {
        /// <summary>
        /// 读取奖品INI信息
        /// </summary>
        /// <returns>
        /// 返回字符串数组
        /// </returns>
        public static string[] LoadLottteryIni()
        {
            int iStringCountReceiver = 0;
            IntPtr UmanageStringBufferIni = IntPtr.Zero;

            LoadLotteryIniInfo(out UmanageStringBufferIni, out iStringCountReceiver);

            string[] ManagedStringArray = null;

            MarshalUnmananagedStrArray2ManagedStrArray(UmanageStringBufferIni, iStringCountReceiver, out ManagedStringArray);

            return ManagedStringArray;
        }

        /// <summary>
        /// 将从C++动态库中读取的字符串数组缓存数据解析为C#字符串数组
        /// </summary>
        /// <param name="pUnmanagedStringArray">缓存地址</param>
        /// <param name="StringCount">字符串数组维数</param>
        /// <param name="ManagedStringArray">返回解析完成的字符串数组</param>
        protected static void MarshalUnmananagedStrArray2ManagedStrArray(IntPtr pUnmanagedStringArray, int StringCount, out string[] ManagedStringArray)
        {
            IntPtr[] pIntPtrArray = new IntPtr[StringCount];
            ManagedStringArray = new string[StringCount];

            Marshal.Copy(pUnmanagedStringArray, pIntPtrArray, 0, StringCount);

            for (int i = 0; i < StringCount; i++)
            {
                ManagedStringArray[i] = Marshal.PtrToStringAnsi(pIntPtrArray[i]);
                Marshal.FreeCoTaskMem(pIntPtrArray[i]);
            }

            Marshal.FreeCoTaskMem(pUnmanagedStringArray);
        }

        /// <summary>
        /// 读取网卡设备信息
        /// </summary>
        /// <returns>
        /// 返回网卡设备的字符串数组
        /// </returns>
        public static string[] LoadNetDevice()
        {
            int iStringCountReceiver = 0;
            IntPtr UmanageStringBufferDevice = IntPtr.Zero;

            GetNetDevice(out UmanageStringBufferDevice, out iStringCountReceiver);

            string[] ManagedStringArray = null;

            MarshalUnmananagedStrArray2ManagedStrArray(UmanageStringBufferDevice, iStringCountReceiver, out ManagedStringArray);

            return ManagedStringArray;
        }

        /// <summary>
        /// 读取拦截到的数组包，一般来说，从C++返回的字符串可以直接写入StringBuilder缓存中，但是由于C++无法确定传入的地址哪部分可以写，所以会造成内存泄漏，为了更加安全的读写，
        /// 这里采用返回字符串地址，然后有C#进行解析，然后再有C#去释放这个内存，而为了更加高效的读写，这个函数里面不再释放内存，而交由C++动态库中加载和卸载的时候自动释放申请的缓存，
        /// 这里返回的字符串数组的编码方式为ANSI
        /// </summary>
        /// <param name="iType">传入需要读取的数据包类型</param>
        /// <param name="strLevel">返回等级信息</param>
        /// <param name="strScriptId">返回脚本ID</param>
        /// <param name="strScriptUid">返回脚本UID</param>
        /// <param name="strContent">返回内容</param>
        /// <param name="strScriptName">返回脚本名字</param>
        public static void LoadScriptMsg(int iType, out string strLevel, out string strScriptId, out string strScriptUid, out string strContent, out string strScriptName)
        {
            IntPtr PtrLevle      = IntPtr.Zero;
            IntPtr PtrScriptId   = IntPtr.Zero;
            IntPtr PtrScriptUid  = IntPtr.Zero;
            IntPtr PtrContent    = IntPtr.Zero;
            IntPtr PtrScriptName = IntPtr.Zero;

            bool result = GetScriptMsg(iType, out PtrLevle, out PtrScriptId, out PtrScriptUid, out PtrContent, out PtrScriptName);

            if (result)
            {
                strLevel = Marshal.PtrToStringAnsi(PtrLevle);
                strScriptId = Marshal.PtrToStringAnsi(PtrScriptId);
                strScriptUid = Marshal.PtrToStringAnsi(PtrScriptUid);
                strContent = Marshal.PtrToStringAnsi(PtrContent);
                strScriptName = Marshal.PtrToStringAnsi(PtrScriptName);
            }
            else
            {
                strLevel = "";
                strScriptId = "";
                strScriptUid = "";
                strScriptName = "";
                strContent = "";
            }
        }

        [DllImport("RLLIB.dll", CallingConvention = CallingConvention.StdCall)]
        protected static extern void LoadLotteryIniInfo(out IntPtr UmanageStringBufferIni, out int iStringCountReceiver);

        [DllImport("RLLIB.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        public static extern void SaveLotteryIniInfo(string[] StringBufferIni, int StringsCount); 

        [DllImport("RLLIB.dll", CallingConvention=CallingConvention.StdCall)]
        public static extern bool InitDll();

        [DllImport("RLLIB.dll", CallingConvention = CallingConvention.StdCall)]
        protected static extern void GetNetDevice(out IntPtr UmanageStringBufferDevice, out int iStringCountReceiver);

        [DllImport("RLLIB.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenNetDevice(int indexDev);

        [DllImport("RLLIB.dll", CallingConvention = CallingConvention.StdCall)]
        public static extern void CloseNetDevice();

        [DllImport("RLLIB.dll", CallingConvention = CallingConvention.StdCall/*, CharSet = CharSet.Unicode*/)]
        protected static extern bool GetScriptMsg(int iType, out IntPtr PtrLevel, out IntPtr PtrScriptId, out IntPtr PtrScriptUid, out IntPtr PtrContent, out IntPtr PtrScriptName);
    }
}
