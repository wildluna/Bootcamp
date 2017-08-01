using Photon.SocketServer;

namespace Bootcamp
{
    public class BootcampApplication : ApplicationBase
    {
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            return new BootcampPeer(initRequest);
        }

        protected override void Setup()
        {
            
        }

        protected override void TearDown()
        {
            
        }
    }
}
