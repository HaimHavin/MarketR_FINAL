using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace MarketR.Models
{
    public class CondorModel
    {
        public int Id { get; set; }
        public string Deal_ID { get; set; }
        public string Balance_Off_Balance { get; set; }
        public string Main_Type { get; set; }
        public string Deal_Type { get; set; }
        public Nullable<bool> Pay_Recieve { get; set; }
        public string Underlying { get; set; }
        public string CCY { get; set; }
        public Nullable<decimal> Notional { get; set; }
        public Nullable<System.DateTime> Maturity_Date { get; set; }
        public string Interest_Type { get; set; }
        public Nullable<System.DateTime> Interest_Change_Date { get; set; }
        public string Interest_change_N { get; set; }
        public Nullable<int> Interest_change_freq { get; set; }
        public Nullable<decimal> Interest_percent { get; set; }
        public Nullable<decimal> NPL_DELTA_ILS { get; set; }
        public string Portfolio { get; set; }
        public Nullable<decimal> GAMMA_ILS { get; set; }
        public Nullable<decimal> VEGA_ILS { get; set; }
        public string Couterparty { get; set; }

        public string AddCondorData(DataTable tbl, string fullPath)
        {
            MarketREntities context = new MarketREntities();

            StringBuilder logData = new StringBuilder("");

            int count = 1;
            int countErr = 0;
            string errText = "";
            var lineCount = tbl.Rows.Count;

            foreach (DataRow row in tbl.Rows)
            {
                count++;
                CONDOR_ROW_INPUT data = new CONDOR_ROW_INPUT();
                try
                {

                    if (row["Deal ID"] != null && row["Deal ID"].ToString() != "")
                    {
                        data.Deal_ID = row["Deal ID"].ToString();
                    }
                    if (row["Balance Off Balance"] != null && row["Balance Off Balance"].ToString() != "")
                    {
                        data.Balance_Off_Balance = row["Balance Off Balance"].ToString();
                    }
                    if (row["Main Type"] != null && row["Main Type"].ToString() != "")
                    {
                        data.Main_Type = row["Main Type"].ToString();
                    }
                    if (row["Deal Type"] != null && row["Deal Type"].ToString() != "")
                    {
                        data.Deal_Type = row["Deal Type"].ToString();
                    }

                    if (row["Pay Recieve"] != null)
                    {
                        if (row["Pay Recieve"].ToString() == "1")
                        {
                            data.Pay_Recieve = true;
                        }
                        else
                        {
                            data.Pay_Recieve = false;
                        }
                    }
                    if (row["Underlying"] != null && row["Underlying"].ToString() != "")
                    {
                        data.Underlying = row["Underlying"].ToString();
                    }
                    if (row["CCY"] != null && row["CCY"].ToString() != "")
                    {
                        data.CCY = row["CCY"].ToString();
                    }

                    if (row["Notional"] != null && row["Notional"].ToString() != "")
                    {
                        data.Notional = Convert.ToDecimal(row["Notional"]);
                    }
                    if (row["Maturity Date"] != null && row["Maturity Date"].ToString() != "")
                    {
                        //data.Maturity_Date = Convert.ToDateTime(row["Maturity Date"].ToString());
                        data.Maturity_Date = DateTime.Parse(row["Maturity Date"].ToString(), new CultureInfo("en-GB"));

                    }
                    if (row["Interest Type"] != null && row["Interest Type"].ToString() != "")
                    {
                        data.Interest_Type = row["Interest Type"].ToString();
                    }

                    if (row["Interest Change Date"] != null && row["Interest Change Date"].ToString() != "")
                    {
                        //data.Interest_Change_Date = Convert.ToDateTime(row["Interest Change Date"].ToString());
                        data.Interest_Change_Date = DateTime.Parse(row["Interest Change Date"].ToString(), new CultureInfo("en-GB"));
                    }
                    if (row["Interest change N"] != null && row["Interest change N"].ToString() != "")
                    {
                        data.Interest_change_N = row["Interest change N"].ToString();
                    }
                    if (row["Interest change freq"] != null && row["Interest change freq"].ToString() != "")
                    {
                        data.Interest_change_freq = Convert.ToInt32(row["Interest change freq"]);
                    }
                    if (row["Interest percent"] != null && row["Interest percent"].ToString() != "")
                    {
                        //  data.Interest_percent = Convert.ToDecimal(row["Interest percent"]);
                    }
                    if (row["NPL-DELTA ILS"] != null && row["NPL-DELTA ILS"].ToString() != "")
                    {
                        data.NPL_DELTA_ILS = Convert.ToDecimal(row["NPL-DELTA ILS"]);
                    }
                    if (row["Portfolio"] != null && row["Portfolio"].ToString() != "")
                    {
                        data.Portfolio = row["Portfolio"].ToString();
                    }

                    if (row["GAMMA ILS"] != null && row["GAMMA ILS"].ToString() != "")
                    {
                        data.GAMMA_ILS = Convert.ToDecimal(row["GAMMA ILS"]);
                    }
                    if (row["VEGA ILS"] != null && row["VEGA ILS"].ToString() != "")
                    {
                        data.VEGA_ILS = Convert.ToDecimal(row["VEGA ILS"]);
                    }
                    if (row["Couterparty"] != null && row["Couterparty"].ToString() != "")
                    {
                        data.Couterparty = row["Couterparty"].ToString();
                    }

                    context.CONDOR_ROW_INPUT.Add(data);


                }
                catch (Exception ex)
                {
                    if (countErr == 0)
                    {
                        errText = "Row " + count;
                    }
                    else
                    {
                        errText = errText + ",  Row " + count;
                    }
                    countErr++;

                    var errorText = ex.Message.ToString();
                }
            }
            if (countErr == 0)
            {
                context.SaveChanges();
                string strError = "Uploaded successfully";
                strError = strError + "<br>Total No of Rows => " + (lineCount);
                strError = strError + "<br>Insert Rows  => " + (lineCount - countErr);
                strError = strError + "<br>Rows with error  => " + countErr;
                return strError;
            }
            else
            {
                string strError = "There are some problem";
                strError = strError + "<br>Total No of Rows => " + (lineCount);
                strError = strError + "<br>Rows with error  => " + countErr;
                strError = strError + "<br> " + errText;

                File.WriteAllText(fullPath, errText);
                return strError;
            }
        }
    }
}