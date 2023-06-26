using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace UdpChat
{
    public partial class Form1 : Form
    {
        bool alive = false; // чи буде працювати потік для приймання
        UdpClient client;
        const int TTL = 20;
        const string HOST = "235.5.5.1"; // хост для групового розсилання
        IPAddress groupAddress; // адреса для групового розсилання
        string userName; // ім’я користувача в чаті
        int localPort;
        int remotePort;
        string configPath = "config.txt";
        string logPath = "log.txt";
    }
    public Form1()
    {
        InitializeComponent();
        loginButton.Enabled = true; // кнопка входу
        logoutButton.Enabled = false; // кнопка виходу
        sendButton.Enabled = false; // кнопка отправки
        chatTextBox.ReadOnly = true;
    }
    private string logFileName = "chat.log"; // ім'я файлу для зберігання логу

    // метод для запису повідомлень у файл
    private void WriteToLogFile(string message)
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(logFileName, true))
            {
                string time = DateTime.Now.ToShortTimeString();
                sw.WriteLine(time + " " + message);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    // метод приймання повідомлення
    private void ReceiveMessages()
    {
        alive = true;
        try
        {
            while (alive)
            {
                IPEndPoint remoteIp = null;
                byte[] data = client.Receive(ref remoteIp);
                string message = Encoding.Unicode.GetString(data);
                // добавляем полученное сообщение в текстовое поле
                this.Invoke(new MethodInvoker(() =>
                {
                    string time = DateTime.Now.ToShortTimeString();
                    chatTextBox.Text = time + " " + message + "\r\n" + chatTextBox.Text;
                }));
                // записуємо повідомлення у лог-файл
                WriteToLogFile(message);
            }
        }
        catch (ObjectDisposedException)
        {
            if (!alive)
                return;
            throw;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
