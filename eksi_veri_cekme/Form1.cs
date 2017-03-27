using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eksi_veri_cekme
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Metodumuz parametre olarak sadece içeriğin çekileceği urli alır.
        void downloadData(string url)
        {
            string text; //text isimli string bir değişken oluşuturuyoruz. 
            WebClient wClient = new WebClient(); //webclientimizi oluşturuyoruz.

            /*aşağıdaki for döngüsünün 1'den 10'a kadar dönmesinin sebebi
             ekşi sözlükte her sayfada 10 entry bulunduğu için*/
            for (int i = 1; i <= 10; i++)
            {
                //WebClient nesnemize encoding işlemi yapıyoruz çünkü indirdiğimiz datadaki türkçe karakterler sıkıntı yaratıyor
                wClient.Encoding = System.Text.Encoding.UTF8;

                //girilen urli tamamen indiriyoruz
                string webSite = wClient.DownloadString(url);

                //nesnemizi oluşturuyoruz.
                HtmlAgilityPack.HtmlDocument hDocument = new HtmlAgilityPack.HtmlDocument();

                //indirdiğimiz datayı aktarıyoruz.
                hDocument.LoadHtml(webSite);

                //Aşağıda yapılan işlemleri anlamak için XPath nasıl kullanılır bunu bilmemiz gerekiyor.
                //Öğrenmek için aşağıdaki adrese göz atabilirsiniz.
                //http://www.buraksenyurt.com/post/XPath-ve-Net-bsenyurt-com-dan.aspx
                HtmlNodeCollection hnCollection = hDocument.DocumentNode.SelectNodes("//*[@id=\"entry-list\"]/li[" + i + "]/div[1]");


                foreach (HtmlNode hn in hnCollection)
                {
                    text = hn.InnerText;
                    //writeData metodu ile çektiğimiz verileri tek tek metin belgesine kaydediyoruz.
                    writeData(text);
                }
            }








        }

        //Bu metot çektiğimiz veriyi metin belgesine kaydeder.
        void writeData(string data)
        {

            StreamWriter writer = new StreamWriter("dosya.txt", true);
            string lastData = Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + data;
            writer.WriteLine(lastData);
            writer.Close();

        }



        string count;
        string calculatePageCount(string url)
        {

            WebClient wClient = new WebClient();
            wClient.Encoding = System.Text.Encoding.UTF8;
            string webSite = wClient.DownloadString(url);

            HtmlAgilityPack.HtmlDocument hd = new HtmlAgilityPack.HtmlDocument();
            hd.LoadHtml(webSite);
            HtmlNodeCollection hnc = hd.DocumentNode.SelectNodes("//*[@id=\"topic\"]/div[2]");

            foreach (HtmlNode hn in hnc)
            {
                count = hn.Attributes["data-pagecount"].Value;

            }
            return count;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            //Url adresini ben aşağıda elle verdim siz isterseniz formda textboxtan alabilirsiniz.
            string url = txtUrl.Text;

            //Aşağıda yazdığımız metot sayesinde sayfa sayısını alıyoruz.
            string pageCount = calculatePageCount(url);

            //Döngümüz 1'den sayfa sayısına kadar dönecek.
            for (int i = 1; i <= Convert.ToInt32(pageCount); i++)
            {
                //Aşağıda if else dallanması yapmamın nedeni ekşi sözlüğün parametreleri ile alakalı.
                //Döngü ilk çalıştığında if bloğuna düşer.
                if (i == 1)
                    downloadData(url);
                else
                {
                    /*Aşağıda ekşi sözlüğün parametre değerlerine göre yeniden urlmizi belirliyoruz.Örneğin;
                      https://eksisozluk.com/cnbc-e--91396 linkimiz  döngü ikinci kez çalıştığında şu şekilde
                      olur https://eksisozluk.com/cnbc-e--91396?p=2 böylece konunun ikinci sayfası açılmış olur*/
                    string newUrl = url + "?p=" + i;
                    downloadData(newUrl);
                }
            }
        }
    }
}
