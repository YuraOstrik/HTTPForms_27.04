using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Web;
using System.Net;

namespace WinFormsApp3
{
    public partial class Form1 : Form
    {
        private Dictionary<string, string> bookLinks = new Dictionary<string, string>();

        public Form1()
        {
            InitializeComponent();
            button1.Click += SearchBooks;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
        }

        private async void SearchBooks(object sender, EventArgs e)
        {
            string query = HttpUtility.UrlEncode(textBox1.Text.Trim());
            if (string.IsNullOrWhiteSpace(query)) return;
            string url = $"https://www.gutenberg.org/ebooks/search/?query={query}";
            listBox1.Items.Clear();
            bookLinks.Clear();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                    string html = await client.GetStringAsync(url);
                    string pattern = @"<li class=""booklink"">.*?<a class=""link"" href=""/ebooks/(\d+)"".*?<span class=""title"">(.*?)</span>";
                    MatchCollection matches = Regex.Matches(html, pattern, RegexOptions.Singleline);
                    foreach (Match m in matches)
                    {
                        string bookId = m.Groups[1].Value;
                        string title = WebUtility.HtmlDecode(m.Groups[2].Value);
                        string bookUrl = $"https://www.gutenberg.org/files/{bookId}/{bookId}-0.txt";

                        bookLinks[title] = bookUrl;
                        listBox1.Items.Add(title);
                    }
                    if (listBox1.Items.Count == 0)
                        MessageBox.Show("Ничего нету");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
        private async void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null) return;
            string title = listBox1.SelectedItem.ToString();
            if (bookLinks.TryGetValue(title, out string url))
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                        string text = await client.GetStringAsync(url);
                        textBox2.Text = text;
                    }
                }
                catch (Exception ex)
                {
                    textBox2.Text = $"Ошибка: {ex.Message}";
                }
            }
        }

    }
}
