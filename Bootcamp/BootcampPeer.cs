using System;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;

namespace Bootcamp
{
    class BootcampPeer : ClientPeer
    {
        public BootcampPeer(InitRequest initRequest)
            : base(initRequest)
        {
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            SendOperationResponse(new OperationResponse(operationRequest.OperationCode, operationRequest.Parameters), sendParameters);
        }
    }
}
