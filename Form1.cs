using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Security;
using System.Data.OleDb;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            readDate("data.txt");
        }

        private void readDate(String filename)
        {
            List<String> strList = new List<string>(File.ReadAllLines("data.txt").Select(o=>o.Trim()).ToList());
            List<String> hrStrList = new List<String>(extractData(strList, "<HRM>", "</HRM>"));
            List<String> saleList = new List<String>(extractData(strList, "<SALES>", "</SALES>"));

            //for extractHrmData function 
            List<String> idList = new List<String>();
            List<String> nameList = new List<String>();
            List<String> posList = new List<String>();
            extractHrmData(hrStrList, idList, nameList, posList);

            //for extractSaleData function
            List<String> saleId = new List<String>();
            List<List<double>> saleMonth = new List<List<double>>();

            extractSalesData(saleList, saleId, saleMonth, "<SalesPerson>", "<MonthlySales>");

            saveResult(idList, nameList, saleId, saleMonth);
            //showList(saleId);
        }

        private List<String> extractData(List<String> list , String startTag , String endTag)
        {
            List<String> temp = new List<String>();
            int initIndex, fnlIndex;

            temp.AddRange(list);

            initIndex = temp.IndexOf(startTag);
            fnlIndex = temp.IndexOf(endTag);

            temp.RemoveRange(fnlIndex, list.Count - fnlIndex);
            temp.RemoveRange(0, initIndex + 1);

            return temp;
        }

        private void extractHrmData(List<String> hrList , List<String> id , List<String> name , List<String> pos)
        {
            foreach(string i in hrList)
            {
                List<String> lst = new List<String>(i.Split(',').ToList());
                id.Add(lst[1]);
                name.Add(lst[2]);
                pos.Add(lst[3]);
            }
        }

        private void extractSalesData(List<string> salesStrList, List<string> salesIdList, List<List<double>> monthlySalesList, string salesPersonTag, string monthlySalesTag)
        {
            for(int i = 0; i < salesStrList.Count; i++)
            {
                List<String> lst = new List<String>(salesStrList[i].Split(',').ToList());
                if (lst.Contains(salesPersonTag))
                    salesIdList.Add(lst[1]);
                else
                {
                    List<double> sale = new List<double>();

                    for (int y = 1; y < lst.Count; y++)
                        sale.Add(Convert.ToDouble(lst[y]));

                    monthlySalesList.Add(sale);
                }
            }
        }

        private void saveResult(List<string> staffIdList, List<string> staffNameList, List<string> salesIdList, List<List<double>> monthlySalesList)
        {
            int counter = 0;
            for (int x = 0; x < monthlySalesList.Count; x++)
            {
                double total = 0.0;
                int index = staffIdList.FindIndex(a => a.Contains(salesIdList[x]));

                String textDetail = staffIdList[index] + " " + staffNameList[index] + "'s Monthly Sales : " + Environment.NewLine;
                File.AppendAllText("result.txt", textDetail);

                List<double> saleDetail = new List<double>(monthlySalesList[x]);

                for (int y = 0; y < saleDetail.Count; y++)
                {
                    total += saleDetail[y];
                    String textToWrite = getMonth(y) + "(RM" + saleDetail[y] + "), ";
                    File.AppendAllText("result.txt", textToWrite);
                }
                File.AppendAllText("result.txt", Environment.NewLine + "= Total Sales (RM" + total + ")");
                File.AppendAllText("result.txt", "\n\n");
            }
        }

        private String getMonth(int m)
        {
            String[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            String mon = months[m];
            return mon;
        }

        private void showList(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(i + ")" + list[i]);
            }
        }
    }
}
