namespace HTTPForms_27._04
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }



        private async void button1_Click(object sender, EventArgs e)
        {
            string url = "https://www.gutenberg.org/files/1524/1524-0.txt\r\n";
            try
            {
                using(HttpClient client = new HttpClient())
                {
                    string text = await client.GetStringAsync(url);
                    textBox1.Text = text;

                }
            }
            catch(Exception ex)
            {
                textBox1.Text = ex.Message;
            }
        }
    }
}
