using System.Security.Cryptography;

namespace CryptoPass
{
    public partial class CryptoPass : Form
    {
        public CryptoPass()
        {
            InitializeComponent();
            label5.AutoSize = false;
            label5.Size = new Size(363, 27);
            label3.AutoSize = false;
            label3.Size = new Size(420, 27);
            MaximizeBox = false;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (label5.Text != "Add a file...")
            {

                string filePath = label5.Text;
                string encrypted_file = filePath + "encrypted_file.txt";
                if (label2.Text != "None")
                {
                    DialogResult dialogResult = MessageBox.Show("You are about to irreversibly encrypt the file with the key provided and will make it only recoverable(readable) again with the same exact key used.", "🔐 Do you really want to procced?", MessageBoxButtons.OKCancel);
                    if (dialogResult == DialogResult.OK)
                    {
                        SharpAESCrypt.SharpAESCrypt.Encrypt(label2.Text, filePath, encrypted_file);
                        File.Delete(filePath);
                        File.Move(encrypted_file, filePath);
                        label3.ForeColor = Color.LightGreen;
                        label3.Text = filePath.Substring(filePath.LastIndexOf(@"\") + 1) + " was successfuly encrypted!";
                        label5.ForeColor = Color.Gray;
                        label5.Text = "Add a file...";
                    }

                    else if (dialogResult == DialogResult.Cancel)
                    {
                        label3.ForeColor = Color.Red;
                        label3.Text = "File encryption canceled.";
                    }
                }
                else
                {
                    label3.ForeColor = Color.Red;
                    label3.Text = "Please generate or import a key first.";
                }

            }
            else
            {
                label3.ForeColor = Color.Red;
                label3.Text = "No file selected.";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (label5.Text == "Add a file...")
            {
                label3.ForeColor = Color.Red;
                label3.Text = "No file selected.";
            }
            else
            {
                if (label2.Text != "None")
                {

                    string filePath = label5.Text;
                    string decrypted_file = filePath + "decrypted_file.txt";
                    try
                    {
                        SharpAESCrypt.SharpAESCrypt.Decrypt(label2.Text, filePath, decrypted_file);
                        File.Delete(filePath);
                        File.Move(decrypted_file, filePath);
                        label3.ForeColor = Color.LightGreen;
                        label3.Text = filePath.Substring(filePath.LastIndexOf(@"\") + 1) + " was successfuly decrypted!";
                        label5.ForeColor = Color.Gray;
                        label5.Text = "Add a file...";

                    }
                    catch (Exception ex)
                    {
                        if (ex is SharpAESCrypt.SharpAESCrypt.WrongPasswordException)
                        {
                            label3.ForeColor = Color.Red;
                            label3.Text = "Wrong dectryption key.";
                            File.Delete(decrypted_file);
                            label5.ForeColor = Color.Gray;
                            label5.Text = "Add a file...";
                        }

                        if (ex is SharpAESCrypt.SharpAESCrypt.HashMismatchException)
                        {
                            label3.ForeColor = Color.Red;
                            label3.Text = "Wrong dectryption key.";
                            File.Delete(decrypted_file);
                            label5.ForeColor = Color.Gray;
                            label5.Text = "Add a file...";
                        }
                        else
                        {
                            label3.ForeColor = Color.Red;
                            label3.Text = "Wrong dectryption key.";
                            File.Delete(decrypted_file);
                            label5.ForeColor = Color.Gray;
                            label5.Text = "Add a file...";
                        }
                    }

                }
                else
                {
                    label3.ForeColor = Color.Red;
                    label3.Text = "Please import an encryption key first.";
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string? keyPath = string.Empty;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {

                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "Key files (*.cpkey)|*.cpkey";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    keyPath = openFileDialog.FileName;
                    string password = File.ReadAllText(keyPath);
                    label2.Text = password;
                    label3.ForeColor = Color.LightGreen;
                    label3.Text = "Key imported successfuly.";
                    label4.ForeColor = Color.LightGreen;
                    var keyName = keyPath.Substring(keyPath.LastIndexOf(@"-") + 1);
                    label4.Text = "Key " + keyName.Remove(keyName.IndexOf('.')) + " is present.";
                }
                else
                {
                    label3.ForeColor = Color.Red;
                    label3.Text = "Key selection canceled.";
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {

            OpenFileDialog folderBrowser = new OpenFileDialog
            {
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "Folder Selection."
            };
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

                using (RSACryptoServiceProvider? rsa = new RSACryptoServiceProvider(4096))
                {
                    Guid myuuid = Guid.NewGuid();
                    string uuid = myuuid.ToString();
                    using (StreamWriter? sw = new StreamWriter(folderPath + "/key" + uuid.Substring(uuid.LastIndexOf("-")) + ".cpkey", false))
                    {
                        sw.Write(rsa.ToXmlString(false));
                    }

                    label2.Text = rsa.ToXmlString(false);
                    label3.ForeColor = Color.LightGreen;
                    label3.Text = "Key exported to " + folderPath + ".";
                    label4.ForeColor = Color.LightGreen;
                    label4.Text = "Key " + uuid.Substring(uuid.LastIndexOf("-")+1) + " is present.";
                }
            }
            else
            {
                label3.ForeColor = Color.Red;
                label3.Text = "Key generation canceled.";
            }

        }
        private void button5_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "all files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    label5.ForeColor = Color.Black;
                    label5.Text = openFileDialog.FileName;
                    label3.ForeColor = Color.LightGreen;
                    label3.Text = "File added successfuly.";
                }
                else
                {
                    label3.ForeColor = Color.Red;
                    label3.Text = "File selection canceled.";
                }
            }
        }
    }
}