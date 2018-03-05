using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Lib.Web.Security
{
    /// <summary>
    /// 实现Ticket加密、解密的接口
    /// </summary>
    public interface ITicketEncryption
    {
        /// <summary>
        /// 加密Ticket
        /// </summary>
        /// <param name="ticket">Ticket对象</param>
        /// <param name="oParam">附加的参数</param>
        /// <returns>返回一个加密过的二进制流</returns>
        byte[] EncryptTicket(ILibAuthenticationTicket ticket, object oParam);

        /// <summary>
        /// 解密Ticket
        /// </summary>
        /// <param name="encryptedData">加密过的ticket的二进制数据</param>
        /// <param name="oParam">附加的参数</param>
        /// <returns>解密后的ticket对象</returns>
        ILibAuthenticationTicket DecryptTicket(byte[] encryptedData, object oParam);
    }

    /// <summary>
    /// 票据加密处理类
    /// </summary>
    public class TicketEncryption : ITicketEncryption
    {
        private const int C_DATA_BLOCK_SIZE = 100;
        private const int C_ENCRYPT_BLOCK_SIZE = 128;

        /// <summary>
        /// 
        /// </summary>
        public TicketEncryption()
        {
        }

        #region IEncryptTicket 成员
        /// <summary>
        /// 加密票据数据
        /// </summary>
        /// <param name="ticket">票据信息</param>
        /// <param name="oParam">加密参数</param>
        /// <returns>加密后字节</returns>
        public byte[] EncryptTicket(ILibAuthenticationTicket ticket, object oParam)
        {
            string strKeyInfo = (string)oParam;

            CspParameters cspParams = new CspParameters();
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.KeyContainerName = "TicketContainer";

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParams);

            rsa.FromXmlString(strKeyInfo);

            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(ticket.Serialize());

            return RSAEncryptData(dataToEncrypt, rsa, false);
        }

        /// <summary>
        /// 解密后票据信息
        /// </summary>
        /// <param name="encryptedData">加密的字节信息</param>
        /// <param name="oParam">解密参数</param>
        /// <returns>解密后的票据信息</returns>
        public ILibAuthenticationTicket DecryptTicket(byte[] encryptedData, object oParam)
        {
            string strKeyInfo = (string)oParam;

            CspParameters cspParams = new CspParameters();
            cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
            cspParams.KeyContainerName = "TicketContainer";

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cspParams);

            rsa.FromXmlString(strKeyInfo);

            byte[] data = RSADecryptData(encryptedData, rsa, false);

            string strXML = Encoding.UTF8.GetString(data);

            return new LibAuthenticationTicket(strXML);
        }
        #endregion

        private byte[] RSAEncryptData(byte[] dataToEncrypt, RSACryptoServiceProvider rsa, bool doOAEPPadding)
        {
            Stream stream = new MemoryStream();

            BinaryWriter bw = new BinaryWriter(stream);

            int nBlocks = (dataToEncrypt.Length - 1) / TicketEncryption.C_DATA_BLOCK_SIZE + 1;

            byte[] srcData = new byte[TicketEncryption.C_DATA_BLOCK_SIZE];

            for (int i = 0; i < nBlocks - 1; i++)
            {
                Array.Copy(dataToEncrypt, TicketEncryption.C_DATA_BLOCK_SIZE * i, srcData, 0, TicketEncryption.C_DATA_BLOCK_SIZE);
                byte[] encData = rsa.Encrypt(srcData, false);

                bw.Write((Int32)encData.Length);
                bw.Write(encData);
            }

            int nRemain = dataToEncrypt.Length - (nBlocks - 1) * TicketEncryption.C_DATA_BLOCK_SIZE;

            if (nRemain > 0)
            {
                srcData = new byte[nRemain];
                Array.Copy(dataToEncrypt, TicketEncryption.C_DATA_BLOCK_SIZE * (nBlocks - 1), srcData, 0, nRemain);
                byte[] encData = rsa.Encrypt(srcData, false);

                bw.Write((Int32)encData.Length);
                bw.Write(encData);
            }

            bw.Write((Int32)(-1));

            byte[] resultData = new byte[stream.Length];

            stream.Position = 0;
            stream.Read(resultData, 0, (int)stream.Length);

            return resultData;
        }

        private byte[] RSADecryptData(byte[] dataToDecrypt, RSACryptoServiceProvider rsa, bool doOAEPPadding)
        {
            Stream streamIn = new MemoryStream(dataToDecrypt);
            Stream streamOut = new MemoryStream();

            BinaryReader br = new BinaryReader(streamIn);
            BinaryWriter bw = new BinaryWriter(streamOut);

            byte[] encData = new byte[TicketEncryption.C_ENCRYPT_BLOCK_SIZE];

            int size = br.ReadInt32();

            while (size > 0)
            {
                br.Read(encData, 0, size);
                bw.Write(rsa.Decrypt(encData, false));
                size = br.ReadInt32();
            }

            streamOut.Position = 0;

            byte[] resultData = new byte[streamOut.Length];
            streamOut.Read(resultData, 0, (int)streamOut.Length);

            return resultData;
        }
    }
}
