using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleMain
{
    /// <summary>
    /// サンプル用のメッセージクラス
    /// </summary>
    /// <remarks>
    /// サンプルとして、16バイトのヘッダを持ち、文字列をデータとするメッセージを定義する。
    /// </remarks>
    public class SampleMessage
    {
        // ヘッダ定義
        public const int HEADER_LENGTH = 16;   // ヘッダ長
        public const int PACKET_LEN_OFS = 0;   // パケット長オフセット（ヘッダ）
        public const int PACKET_LEN_SIZE = sizeof(UInt16);  // パケット長サイズ（ヘッダ）
        const int SRC_OFS = 2;          // 送信元オフセット（ヘッダ）
        const int SRC_LEN = 6;          // 送信元バイト長（ヘッダ）
        const int DST_OFS = 8;          // 送信先オフセット（ヘッダ）
        const int DST_LEN = 6;          // 送信先バイト長（ヘッダ）
        const int DTYPE_OFS = 14;       // データ種別オフセット（ヘッダ）

        // ヘッダ
        byte[] _head;

        // データ（中身はUTF8文字列）
        byte[] _data;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>受信したバイトデータから生成する。</remarks>
        /// <param name="head">ヘッダ</param>
        /// <param name="data">データ</param>
        public SampleMessage(byte[] head, byte[] data)
        {
            _head = new byte[head.Length];
            Buffer.BlockCopy(head, 0, _head, 0, head.Length);
            _data = new byte[data.Length];
            Buffer.BlockCopy(data, 0, _data, 0, data.Length);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <remarks>送信用に生成する。データはSetText()で設定する。</remarks>
        /// <param name="src">送信元</param>
        /// <param name="dst">送信先</param>
        public SampleMessage(string src, string dst)
        {
            _head = new byte[HEADER_LENGTH];
            SetSrc(src);
            SetDst(dst);
        }

        /// <summary>
        /// 送信データの設定
        /// </summary>
        /// <param name="dtype">データ種別</param>
        /// <param name="data">送信データ</param>
        public void SetText(byte dtype, string data)
        {
            UInt16 len = (UInt16)Encoding.UTF8.GetByteCount(data);
            SetDtype(dtype);
            SetDataLength(len);
            _data = new byte[len];
            byte[] dat = Encoding.UTF8.GetBytes(data);
            Buffer.BlockCopy(dat, 0, _data, 0, dat.Length);
        }

        private void SetSrc(string str)
        {
            byte[] src = Encoding.UTF8.GetBytes(str);
            Buffer.BlockCopy(src, 0, _head, SRC_OFS, src.Length);
        }
        private void SetDst(string str)
        {
            byte[] dst = Encoding.UTF8.GetBytes(str);
            Buffer.BlockCopy(dst, 0, _head, DST_OFS, dst.Length);
        }

        private void SetDtype(byte dtype)
        {
            _head[DTYPE_OFS] = dtype;
        }

        private void SetDataLength(UInt16 len)
        {
            UInt16 packetlen = (UInt16)(len + HEADER_LENGTH);
            byte[] dat = BitConverter.GetBytes(packetlen);
            Array.Reverse(dat);
            Buffer.BlockCopy(dat, 0, _head, PACKET_LEN_OFS, dat.Length);
        }

        public byte GetDtype()
        {
            return _head[DTYPE_OFS];
        }
        public string GetSrc()
        {
            byte[] src = new byte[SRC_LEN];
            Buffer.BlockCopy(_head, SRC_OFS, src, 0, SRC_LEN);
            return Encoding.UTF8.GetString(src).TrimEnd('\0');
        }
        public string GetDst()
        {
            byte[] dst = new byte[DST_LEN];
            Buffer.BlockCopy(_head, DST_OFS, dst, 0, DST_LEN);
            return Encoding.UTF8.GetString(dst).TrimEnd('\0');
        }
        public string GetText()
        {
            return Encoding.UTF8.GetString(_data);
        }

        public byte[] GetHead()
        {
            byte[] buf = new byte[_head.Length];
            Buffer.BlockCopy(_head, 0, buf, 0, _head.Length);
            return buf;
        }
        public byte[] GetData()
        {
            byte[] buf = new byte[_data.Length];
            Buffer.BlockCopy(_data, 0, buf, 0, _data.Length);
            return buf;
        }
    }
}
