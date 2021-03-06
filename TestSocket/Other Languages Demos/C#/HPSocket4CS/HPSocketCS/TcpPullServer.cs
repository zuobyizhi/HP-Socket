﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using HPSocketCS.SDK;

namespace HPSocketCS
{
    public class TcpPullServer : TcpServer
    {

        protected HPSocketSdk.HP_FN_OnPullReceive OnPullReceiveCallback;

        public TcpPullServer()
        {
            CreateListener();
        }

        /// <summary>
        /// 创建socket监听&服务组件
        /// </summary>
        /// <param name="isUseDefaultCallback">是否使用tcppullserver类默认回调函数</param>
        /// <returns></returns>
        public override bool CreateListener()
        {
            if (IsCreate == true || pListener != IntPtr.Zero || pServer != IntPtr.Zero)
            {
                return false;
            }

            pListener = HPSocketSdk.Create_HP_TcpPullServerListener();
            if (pListener == IntPtr.Zero)
            {
                return false;
            }

            pServer = HPSocketSdk.Create_HP_TcpPullServer(pListener);
            if (pServer == IntPtr.Zero)
            {
                return false;
            }
            
            IsCreate = true;

            return true;
        }

        /// <summary>
        /// 抓取数据
        /// 用户通过该方法从 Socket 组件中抓取数据
        /// </summary>
        /// <param name="connId"></param>
        /// <param name="pBuffer"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public En_HP_FetchResult Fetch(uint connId, IntPtr pBuffer, int size)
        {
            return HPSocketSdk.HP_TcpPullServer_Fetch(pServer, connId, pBuffer, size);
        }

        /// <summary>
        /// 设置回调函数
        /// </summary>
        /// <param name="prepareListen"></param>
        /// <param name="accept"></param>
        /// <param name="send"></param>
        /// <param name="recv"></param>
        /// <param name="close"></param>
        /// <param name="error"></param>
        /// <param name="shutdown"></param>
        public virtual void SetCallback(HPSocketSdk.HP_FN_OnPrepareListen prepareListen,
            HPSocketSdk.HP_FN_OnAccept accept, HPSocketSdk.HP_FN_OnSend send,
            HPSocketSdk.HP_FN_OnPullReceive recv, HPSocketSdk.HP_FN_OnClose close,
            HPSocketSdk.HP_FN_OnError error, HPSocketSdk.HP_FN_OnServerShutdown shutdown)
        {

            // 设置 Socket 监听器回调函数
            OnPullReceiveCallback = new HPSocketSdk.HP_FN_OnPullReceive(recv);

            // 设置 Socket 监听器回调函数
            HPSocketSdk.HP_Set_FN_Server_OnPullReceive(pListener, OnPullReceiveCallback);

            base.SetCallback(prepareListen, accept, send, OnReceive, close, error, shutdown);
        }

        public virtual void SetOnPullReceiveCallback(HPSocketSdk.HP_FN_OnPullReceive recv)
        {
            OnPullReceiveCallback = new HPSocketSdk.HP_FN_OnPullReceive(recv);
            HPSocketSdk.HP_Set_FN_Server_OnPullReceive(pListener, OnPullReceiveCallback);
        }

        /// <summary>
        /// 释放TcpPullServer和TcpPullServerListener
        /// </summary>
        public override void Destroy()
        {
            Stop();

            if (pServer != IntPtr.Zero)
            {
                HPSocketSdk.Destroy_HP_TcpPullServer(pServer);
                pServer = IntPtr.Zero;
            }
            if (pListener != IntPtr.Zero)
            {
                HPSocketSdk.Destroy_HP_TcpPullServerListener(pListener);
                pListener = IntPtr.Zero;
            }
            IsCreate = false;
        }

        /// <summary>
        /// 数据到达
        /// </summary>
        /// <param name="dwConnID"></param>
        /// <param name="iLength"></param>
        /// <returns></returns>
        protected virtual En_HP_HandleResult OnPullReceive(uint dwConnID, int iLength)
        {
            return En_HP_HandleResult.HP_HR_OK;
        }
    }
}
