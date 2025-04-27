using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Windows.Forms;
namespace HTTPForms_2
{
    public partial class Form1 : Form
    {
        private Dictionary<string, string> Links = new Dictionary<string, string>();


        public Form1()
        {
            InitializeComponent();
            Loading();
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
        }

        private async void Loading()
        {
            string url = "https://www.gutenberg.org/browse/scores/top";
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                    string html = await client.GetStringAsync(url);
                    string pattern = @"<ol>(.*?)</ol>";
                    Match match = Regex.Match(html, pattern, RegexOptions.Singleline);
                    if (match.Success) {
                        string listHtml = match.Groups[1].Value;
                        pattern = @"<li><a href=""/ebooks/(\d+)"">(.*?)</a>";
                        MatchCollection matches = Regex.Matches(listHtml, pattern);

                        foreach (Match m in matches)
                        {
                            string bookId = m.Groups[1].Value;
                            string title = WebUtility.HtmlDecode(m.Groups[2].Value);
                            string txtUrl = $"https://www.gutenberg.org/files/{bookId}/{bookId}-0.txt";
                            Links[title] = txtUrl;
                            listBox1.Items.Add(title);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        private async void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;

            string title = listBox1.SelectedItem.ToString();
            if(Links.TryGetValue(title, out var url))
            {
                textBox1.Text = "Загрузка книги...";
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                        string text = await client.GetStringAsync(url);
                        textBox1.Text = text;
                    }
                }
                catch (Exception ex)
                {
                    textBox1.Text = $"Ошибка: {ex.Message}";
                }
            }
        }
    }
}
