using System;
using ExitGames.Client.Photon;
using ExitGames.Concurrency.Fibers;

namespace BootcampClient
{
    public class TestClient : IDisposable, IPhotonPeerListener
    {
        private PhotonPeer PhotonClient;
        private PoolFiber ServiceFiber;
        private PoolFiber ExecuteFiber;

        /// <summary>
        /// Peer連線狀態
        /// </summary>
        public PeerStateValue ConnectionState { get { return PhotonClient.PeerState; } }
        public IPhotonPeerListener Listener { get; set; }

        public TestClient(IPhotonPeerListener listener)
        {
            Listener = listener;

            PhotonClient = new PhotonPeer(this, ConnectionProtocol.Tcp);
            ServiceFiber = new PoolFiber();
            ExecuteFiber = new PoolFiber();
        }

        public void Connect()
        {
            Connect(ClientSetting.Default.ServerHost, ClientSetting.Default.TcpPort, ClientSetting.Default.ApplicationName);
        }

        public void Connect(string hostName, int port, string applicationId)
        {
            bool aaa = PhotonClient.Connect(string.Format("{0}:{1}", hostName, port), applicationId);
            StartService();
        }

        public void Close()
        {
            PhotonClient.Disconnect();

            StopService();
            StopExecute();
        }

        private void OnConnect()
        {
            StartExecute();
        }

        private void OnDisconnect()
        {

        }
        private void StartService()
        {
            ServiceFiber.Start();
            ServiceFiber.Enqueue(Service);
        }

        private void StopService()
        {
            if (ServiceFiber != null)
            {
                ServiceFiber.Stop();
                ServiceFiber.Dispose();
                ServiceFiber = new PoolFiber();
            }
            
        }

        private void Service()
        {
            PhotonClient.Service();
            ServiceFiber.Enqueue(Service);
        }

        private void StartExecute()
        {
            ExecuteFiber.Start();
        }

        private void StopExecute()
        {
            if (ExecuteFiber != null)
            {
                ExecuteFiber.Dispose();
                ExecuteFiber = new PoolFiber();
            }
        }
        public void SendMessage(string message)
        {
            ExecuteFiber.Enqueue(() => { PhotonClient.OpCustom(0, new System.Collections.Generic.Dictionary<byte, object> { { 0, message } }, true); });
        }
        public void DebugReturn(DebugLevel level, string message)
        {
            Listener?.DebugReturn(level, message);
        }

        public void OnEvent(EventData eventData)
        {
            Listener?.OnEvent(eventData);
        }

        public void OnOperationResponse(OperationResponse operationResponse)
        {
            Listener?.OnOperationResponse(operationResponse);
        }

        public void OnStatusChanged(StatusCode statusCode)
        {
            if(statusCode == StatusCode.Connect)
            {
                OnConnect();
            }
            else if(statusCode == StatusCode.Disconnect)
            {
                OnDisconnect();
            }

            Listener?.OnStatusChanged(statusCode);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 偵測多餘的呼叫

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 處置 Managed 狀態 (Managed 物件)。
                    ServiceFiber.Dispose();
                    ExecuteFiber.Dispose();
                }

                // TODO: 釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下方的完成項。
                // TODO: 將大型欄位設為 null。

                disposedValue = true;
            }
        }

        // TODO: 僅當上方的 Dispose(bool disposing) 具有會釋放 Unmanaged 資源的程式碼時，才覆寫完成項。
        // ~TestClient() {
        //   // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 加入這個程式碼的目的在正確實作可處置的模式。
        public void Dispose()
        {
            // 請勿變更這個程式碼。請將清除程式碼放入上方的 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果上方的完成項已被覆寫，即取消下行的註解狀態。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
