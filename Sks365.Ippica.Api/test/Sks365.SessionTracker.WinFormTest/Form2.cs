using System;
using System.Windows.Forms;

namespace Sks365.SessionTracker.WinFormTest
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtAspNetSession.Text))
            {
                MessageBox.Show("Enter AspNetSession.");
                return;
            }

            if (string.IsNullOrEmpty(txtBookmakerId.Text))
            {
                MessageBox.Show("Enter BookmakerID.");
                return;
            }

            if (string.IsNullOrEmpty(txtUsername.Text))
            {
                MessageBox.Show("Enter Username.");
                return;
            }

            string toEncrypt = txtBookmakerId.Text + "&" + txtUsername.Text + "&" + txtAspNetSession.Text;

            txtSessionToken.Text = Sks365.SessionTracker.Client.CryptographyTool.EncryptMD5(toEncrypt, "1138D81B-F819-4416-A01D-46CB1BE71A85", true, false);
        }

        private void btnDecode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSessionToken2.Text))
            {
                MessageBox.Show("Session token missing.");
                return;
            }
            var token = txtSessionToken2.Text.Trim();

            var decodedValue = Sks365.SessionTracker.Client.CryptographyTool.DecryptMD5(token, "1138D81B-F819-4416-A01D-46CB1BE71A85", true, false);

            string[] tokenParts = decodedValue.Split('&');

            if (tokenParts.Length != 3)
            {
                throw new Exception("Token parts must have 3 parts.");
            }

            int bookmakerId = Convert.ToInt32(tokenParts[0]);
            string username = tokenParts[1];
            string aspNetSession = tokenParts[2];

            txtAspSession2.Text = aspNetSession;
            txtBookmaker2.Text = bookmakerId.ToString();
            txtUserName2.Text = username;
        }
    }
}
