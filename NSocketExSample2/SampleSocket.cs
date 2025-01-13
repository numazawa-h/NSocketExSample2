using NCommonUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleMain
{
    public delegate void SampleMessageEventHandler(Object sender, SampleMessageEventArgs args);
    public class SampleMessageEventArgs : EventArgs
    {
        public SampleSocket Socket { get; private set; }
        public SampleMessage Message { get; private set; }

        public SampleMessageEventArgs(SampleSocket socket, SampleMessage msg)
        {
            Socket = socket;
            Message = msg;
        }
    }


    public class SampleSocket: NSocketEx
    {
    
        public event SampleMessageEventHandler OnSendMessageEvent;
        public event SampleMessageEventHandler OnPreSendMessageEvent;
        public event SampleMessageEventHandler OnRecvMessageEvent;

        public SampleSocket() : base() 
        {
            base.init(SampleMessage.HEADER_LENGTH, SampleMessage.PACKET_LEN_OFS, SampleMessage.PACKET_LEN_SIZE, true);
        }

        public SampleSocket(Socket soc) : base(soc)
        {
            base.init(SampleMessage.HEADER_LENGTH, SampleMessage.PACKET_LEN_OFS, SampleMessage.PACKET_LEN_SIZE, true);
        }

        public void Send(SampleMessage msg)
        {
            OnPreSendMessageEvent?.Invoke(this, new SampleMessageEventArgs(this, msg));
            Send(msg.GetHead(), msg.GetData());
            OnSendMessageEvent?.Invoke(this, new SampleMessageEventArgs(this, msg));
        }

        protected override void AcceptCallback(IAsyncResult ar)
        {
            SampleSocket socket = (SampleSocket)ar.AsyncState;
            if (socket._soc == null)
            {
                return;
            }
            try
            {
                Socket soc = socket._soc.EndAccept(ar);
                OnAccept(new SampleSocket(soc));
            }
            catch (Exception ex)
            {
                OnDisConnect();
                OnException(ex);
            }
        }   

        protected override void OnRecvEx()
        {
            SampleMessage msg = new SampleMessage(_comm_header, _comm_data);
            OnRecvMessageEvent?.Invoke(this, new SampleMessageEventArgs(this, msg));
        }
    }
}
