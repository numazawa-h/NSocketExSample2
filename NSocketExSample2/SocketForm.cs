using NCommonUtility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleMain
{
    public partial class SocketForm : Form
    {
        SampleSocket _Socket;

        public SocketForm(SampleSocket socket)
        {
            _Socket = socket;
            _Socket.OnDisConnectEvent += OnDisConnect;
            _Socket.OnRecvMessageEvent += OnReceive;
            _Socket.OnSendMessageEvent += OnSend;

            InitializeComponent();

            txt_ipAddr1.Text = socket.LocalIPAddress?.ToString();
            txt_portNo1.Text = socket.LocalPortno?.ToString();
            txt_ipAddr2.Text = socket.RemoteIPAddress.ToString();
            txt_portNo2.Text = socket.RemotePortno.ToString();

            if (socket.isServer)
            {
                this.Text = "サーバーソケット";
            }
            if (socket.isClient)
            {
                this.Text = "クライアントソケット";
            }
        }

        private void DisplayLog(string message)
        {
            txt_log.Text += $"{DateTime.Now} {message}\r\n";
        }

        private void OnDisConnect(object sender, NSocketEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new NSocketEventHandler(OnDisConnect), new object[] { sender, args });
                return;
            }
            this.Close();
        }

        private void OnReceive(object sender, SampleMessageEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SampleMessageEventHandler(OnReceive), new object[] { sender, args });
                return;
            }
            SampleMessage msg = args.Message;
            string src = msg.GetSrc();
            string dst = msg.GetDst();
            string dtype = $"{msg.GetDtype():X2}";
            DisplayLog($"RECV[{src}->{dst}][{dtype}]{msg.GetText()}");
        }

        private void OnSend(object sender, SampleMessageEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SampleMessageEventHandler(OnSend), new object[] { sender, args });
                return;
            }
            SampleMessage msg = args.Message;
            string src = msg.GetSrc();
            string dst = msg.GetDst();
            string dtype = $"{msg.GetDtype():X2}";
            DisplayLog($"SEND[{src}->{dst}][{dtype}]{msg.GetText()}");
        }


        private void SocketForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _Socket.Close();
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            SampleMessage msg = new SampleMessage(txt_src.Text, txt_dst.Text);
            int dtype = int.Parse(txt_dtype.Text.Trim());
            msg.SetText((byte)dtype, txt_sendData.Text);
            _Socket.Send(msg);
        }
    }
}
