using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MtApi5;
using vtocSqlInterface;


namespace mtapi5test
{
    public partial class Form1 : Form
    {
        List<OrderItems> Ilist;
        private List<Symbols> Symbol;
        OrderItems Oitem;
        public MtApi5Client apiClient = new MtApi5Client();
        private vtocSqlInterface.sqlInterface mySql;
        private string sqlCmd;
        private int Scop;
        public DataTable prow = new DataTable();
        public double Totalcapital;

        public Form1()
        {
            InitializeComponent();
            mySql = new sqlInterface(Properties.Settings.Default.sqlserver, "AdjPrice",
                                     Properties.Settings.Default.username, Properties.Settings.Default.pass);
            if (txtip.Text == "")
            {
                apiClient.BeginConnect(Convert.ToInt16(txtport.Text));
            }
            else
            {
                apiClient.BeginConnect(txtip.Text, Convert.ToInt16(txtport.Text));
            }

            prow.Columns.Add("Symbol");
            prow.Columns.Add("Price");
            prow.Columns.Add("Volume");
            prow.Columns.Add("#Order");
            prow.Columns.Add("Time");

            Ilist = new List<OrderItems>();
            Symbol = new List<Symbols>();

            ReadSymbols();

            Symbol = Symbol.OrderBy(t => t.Symbol).ToList();
            lstFirstSymbol.DataSource = Symbol;
            lstFirstSymbol.ValueMember = "InsCode";
            lstFirstSymbol.DisplayMember = "Symbol";
            lstFirstSymbol.SelectedValue = "Selected";

        }

        Dictionary<int, double> timeLog = new Dictionary<int, double>();

        private void ReadSymbols()
        {

            sqlCmd = @"  SELECT DISTINCT S.LVal18AFC as SymbolName, S.InsCode as Symbol,S.LVal30
                          FROM [TseTrade].[dbo].[vwTseInstrument] S
                          JOIN TseTrade.dbo.vwTsePrice T ON T.InsCode = S.InsCode
                          WHERE S.Flow in (1,2) and YMarNSC='No' and YVal in (300 ,303)
                          ORDER BY LVal18AFC";
            DataTable dtSymbols = mySql.SqlExecuteReader(sqlCmd);
            foreach (DataRow row in dtSymbols.Rows)
            {
                if (dtSymbols.Columns.Contains("Symbol") && dtSymbols.Columns.Contains("SymbolName"))
                {
                    var sCode = row["Symbol"].ToString();
                    var sName = row["SymbolName"].ToString();

                    Symbol.Add(new Symbols(sName, sCode));
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MqlTradeRequest request = new MqlTradeRequest();
            MqlTradeResult result;


            DateTime tstart = DateTime.Now;
            DateTime tend;
            for (int i = 0; i < 2000; i++)
            {

                if (apiClient.ConnectionState.ToString() != "Connected")
                {
                    MessageBox.Show("There is no connection. Please check IP and Ports in Setting Tab.");
                }
                else
                {
                    //http://www.mql5.com/en/docs/trading/ordersend
                    // important link in methods of requesting : http://www.mql5.com/en/docs/constants/structures/mqltraderequest
                    request.Symbol = txtsymbol.Text;
                    request.Price = Convert.ToDouble(txtprice.Text);
                    request.Volume = Convert.ToDouble(txtvolume.Text);
                    switch (cmbExp.Text)
                    {
                        case "GTC":
                            request.Type_time = ENUM_ORDER_TYPE_TIME.ORDER_TIME_GTC;
                            break;
                        case "DAY":
                            request.Type_time = ENUM_ORDER_TYPE_TIME.ORDER_TIME_DAY;
                            break;
                        case "Specified":
                            request.Type_time = ENUM_ORDER_TYPE_TIME.ORDER_TIME_SPECIFIED;
                            break;
                    }

                    request.Expiration = DateTime.Now.AddDays(2); // can specify by adding a real date and time

                    //request.Tp = Convert.ToDouble(txttp.Text);
                    //request.Sl = Convert.ToDouble(txtsl.Text);

                    //switch (cmbordertype.Text)
                    //{
                    //    case "Buy":
                    //        request.Type = ENUM_ORDER_TYPE.ORDER_TYPE_BUY;
                    //        break;
                    //    case "BUY_LIMIT":
                    //        request.Type = ENUM_ORDER_TYPE.ORDER_TYPE_BUY_LIMIT;
                    //        break;
                    //    case "BUY_STOP":
                    //        request.Type = ENUM_ORDER_TYPE.ORDER_TYPE_BUY_STOP;
                    //        break;
                    //    case "BUY_STOP_LIMIT":
                    //        request.Type = ENUM_ORDER_TYPE.ORDER_TYPE_BUY_STOP_LIMIT;
                    //        break;
                    //    case "SELL":
                    //        request.Type = ENUM_ORDER_TYPE.ORDER_TYPE_SELL;
                    //        break;
                    //    case "SELL_LIMIT":
                    //        request.Type = ENUM_ORDER_TYPE.ORDER_TYPE_SELL_LIMIT;
                    //        break;
                    //    case "SELL_STOP":
                    //        request.Type = ENUM_ORDER_TYPE.ORDER_TYPE_SELL_STOP;
                    //        break;
                    //    case "SELL_STOP_LIMIT":
                    //        request.Type = ENUM_ORDER_TYPE.ORDER_TYPE_SELL_STOP_LIMIT;
                    //        break;
                    //}

                    switch (cmbaction.Text)
                    {
                        case "DEAL":
                            request.Action = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_DEAL;
                            break;
                        case "MODIFY":
                            request.Action = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_MODIFY;
                            break;
                        case "PENDING":
                            request.Action = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_PENDING;
                            break;
                        case "REMOVE":
                            request.Action = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_REMOVE;
                            break;
                        case "SLTP":
                            request.Action = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_SLTP;
                            break;
                    }

                    switch (cmbfilling.Text)
                    {
                        case "RETURN":
                            request.Type_filling = ENUM_ORDER_TYPE_FILLING.ORDER_FILLING_RETURN;
                            break;
                        case "FOK":
                            request.Type_filling = ENUM_ORDER_TYPE_FILLING.ORDER_FILLING_FOK;
                            break;
                        case "IOC":
                            request.Type_filling = ENUM_ORDER_TYPE_FILLING.ORDER_FILLING_IOC;
                            break;
                    }

                    if (i == (i / 2) * 2)
                    {
                        request.Type = ENUM_ORDER_TYPE.ORDER_TYPE_BUY;
                    }
                    else
                    {
                        request.Type = ENUM_ORDER_TYPE.ORDER_TYPE_SELL;
                    }


                    //Send Order and Get Result
                    bool x = apiClient.OrderSend(request, out result);
                    if (result != null)
                    {
                        long answer = result.Retcode;
                        string ErrorCons;
                        string ErrorDef;
                        ErrorCode.getdes(answer, out ErrorCons, out  ErrorDef);
                        double xprice = result.Price;
                        string currenttime = DateTime.Now.ToShortTimeString();
                        if (cmbaction.Text == "PENDING" && ErrorCons == "TRADE_RETCODE_DONE")
                        {
                            ErrorDef = "Pending";
                            xprice = Convert.ToDouble(txtprice.Text);
                            Oitem = new OrderItems();
                            Oitem.price = xprice;
                            Oitem.symbol = txtsymbol.Text;
                            Oitem.volume = result.Volume;
                            Oitem.type = cmbordertype.Text;
                            Oitem.Orderticket = result.Order;
                            Oitem.time = currenttime;
                            Oitem.sender = "Me";
                            Oitem.state = "Unknown";

                            Ilist.Add(Oitem);
                        }


                        int nrow = dggrid.RowCount + 1;
                        dggrid.Rows.Add(nrow, txtsymbol.Text, xprice, result.Volume, result.Order, currenttime, ErrorDef, request.Type);
                        dggrid.FirstDisplayedScrollingRowIndex = dggrid.Rows.Count - 1;
                    }
                    else
                    {
                        MessageBox.Show("Unknown Error occured.");
                    }

                }
                if (i % 100 == 0)
                {
                    tend = DateTime.Now;
                    double tdur = tend.Subtract(tstart).TotalMilliseconds;
                    timeLog.Add(i, tdur);
                }
            }

            //MessageBox.Show(tdur.ToString());

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "Connecting ...";
            cmbaction.SelectedItem = cmbaction.Items[0];
            cmbordertype.SelectedItem = cmbordertype.Items[0];
            cmbfilling.SelectedItem = cmbfilling.Items[2];
            cmbExp.SelectedItem = cmbExp.Items[0];
            timer1.Start();

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Text = apiClient.ConnectionState.ToString();
            SyncOrders();
        }

        private void btnrecon_Click(object sender, EventArgs e)
        {
            try
            {
                apiClient.BeginDisconnect();
                if (txtip.Text == "")
                {
                    apiClient.BeginConnect(Convert.ToInt16(txtport.Text));
                }
                else
                {
                    apiClient.BeginConnect(txtip.Text, Convert.ToInt16(txtport.Text));
                }
            }
            catch
            {

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SyncOrders();
        }


        private void SyncOrders()
        {
            ulong ticket = 0;
            double open_price;
            double initial_volume;
            int time_setup;
            string symbol;
            string type;
            long order_magic;
            long positionID;
            textBox1.Text = "";

            int total = apiClient.OrdersTotal();

            var Mlist = new List<OrderItems>();
            for (int i = 0; i < total; i++)
            {
                if ((ticket = apiClient.OrderGetTicket(i)) > 0)
                {

                    open_price = apiClient.OrderGetDouble(ENUM_ORDER_PROPERTY_DOUBLE.ORDER_PRICE_OPEN);
                    time_setup = Convert.ToInt32(apiClient.OrderGetInteger(ENUM_ORDER_PROPERTY_INTEGER.ORDER_TIME_SETUP));
                    symbol = apiClient.OrderGetString(ENUM_ORDER_PROPERTY_STRING.ORDER_SYMBOL);
                    order_magic = apiClient.OrderGetInteger(ENUM_ORDER_PROPERTY_INTEGER.ORDER_MAGIC);
                    positionID = apiClient.OrderGetInteger(ENUM_ORDER_PROPERTY_INTEGER.ORDER_POSITION_ID);
                    initial_volume = apiClient.OrderGetDouble(ENUM_ORDER_PROPERTY_DOUBLE.ORDER_VOLUME_INITIAL);
                    type = getStringOrdertype(apiClient.OrderGetInteger(ENUM_ORDER_PROPERTY_INTEGER.ORDER_TYPE));

                    Oitem = new OrderItems();
                    Oitem.price = open_price;
                    Oitem.time = time_setup.ToString();
                    Oitem.symbol = symbol;
                    Oitem.volume = initial_volume;
                    Oitem.type = type;
                    Oitem.Orderticket = ticket;
                    //Oitem.state = "Pending";
                    Mlist.Add(Oitem);
                    //foreach (var x in Ilist)
                    //{
                    //    if (Oitem.Equals(x))
                    //    {
                    //        dgpending.Rows.Add(symbol, initial_volume, type, time_setup, "Me");
                    //    }
                    //    else
                    //    {
                    //        dgpending.Rows.Add(symbol, initial_volume, type, time_setup, "Other");
                    //    }
                    //}
                }
            }
            var Ulist = new List<OrderItems>();
            Ulist.AddRange(Mlist);
            Ulist.Union(Ilist);
            //Ulist = Mlist.Union<OrderItems>(Ilist).ToList();


            dgpending.Rows.Clear();
            foreach (var x in Ulist)
            {
                if (OrderItems.Contains(Ilist, x) && OrderItems.Contains(Mlist, x))
                {
                    x.sender = "Me";
                    x.state = "Pending";
                }
                else if (OrderItems.Contains(Ilist, x) && !OrderItems.Contains(Mlist, x))
                {
                    x.sender = "Me";
                    x.state = "Removed";
                }
                else if (!OrderItems.Contains(Ilist, x) && OrderItems.Contains(Mlist, x))
                {
                    x.sender = "Others";
                    x.state = "Pending";
                }
                else
                {
                    // how come you have reached here?!
                }

                dgpending.Rows.Add(x.symbol, x.volume, x.type, x.time, x.sender, x.state);

            }


        }
        ///http://www.mql5.com/en/docs/trading/ordergetticket
        ///


        public string getStringOrdertype(long code)
        {
            switch (code)
            {
                case 0:
                    return "Buy";
                case 1:
                    return "SELL";
                case 2:
                    return "BUY_LIMIT";
                case 3:
                    return "SELL_LIMIT";
                case 4:
                    return "BUY_STOP";
                case 5:
                    return "SELL_STOP";
                case 6:
                    return "BUY_STOP_LIMIT";
                case 7:
                    return "SELL_STOP_LIMIT";
                default:
                    return "NAN";
            }
        }

        public class Symbols
        {
            public Symbols(string _Name, string _InsCode)
            {
                Symbol = _Name;
                InsCode = _InsCode;
            }

            public bool Selected { get; set; }
            public string InsCode { get; set; }
            public string Symbol { get; set; }

        }

        private void cmdAlgTrd_Click(object sender, EventArgs e)
        {
            AlgoTrading();
           timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            AlgoTrading();
        }

        private string Trade(string SymbolName, Int32 Volume, string TrType)
        {
            string results = "";
            MqlTradeRequest request = new MqlTradeRequest();
            MqlTradeResult result;



            DateTime tstart = DateTime.Now;
            DateTime tend;


            if (apiClient.ConnectionState.ToString() != "Connected")
            {
                MessageBox.Show("There is no connection. Please check IP and Ports in Setting Tab.");
            }
            else
            {
                //http://www.mql5.com/en/docs/trading/ordersend
                // important link in methods of requesting : http://www.mql5.com/en/docs/constants/structures/mqltraderequest
                request.Symbol = SymbolName;
                //request.Price = Convert.ToDouble(txtprice.Text);
                request.Volume = Volume;
                request.Type_time = ENUM_ORDER_TYPE_TIME.ORDER_TIME_GTC;

                //switch (cmbExp.Text)
                //{
                //    case "GTC":
                //        request.Type_time = ENUM_ORDER_TYPE_TIME.ORDER_TIME_GTC;
                //        break;
                //    case "DAY":
                //        request.Type_time = ENUM_ORDER_TYPE_TIME.ORDER_TIME_DAY;
                //        break;
                //    case "Specified":
                //        request.Type_time = ENUM_ORDER_TYPE_TIME.ORDER_TIME_SPECIFIED;
                //        break;
                //}

                //request.Expiration = DateTime.Now.AddDays(2); // can specify by adding a real date and time

                request.Action = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_DEAL;
                //switch (cmbaction.Text)
                //{
                //    case "DEAL":
                //        request.Action = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_DEAL;
                //        break;
                //    case "MODIFY":
                //        request.Action = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_MODIFY;
                //        break;
                //    case "PENDING":
                //        request.Action = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_PENDING;
                //        break;
                //    case "REMOVE":
                //        request.Action = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_REMOVE;
                //        break;
                //    case "SLTP":
                //        request.Action = ENUM_TRADE_REQUEST_ACTIONS.TRADE_ACTION_SLTP;
                //        break;
                //}

                request.Type_filling = ENUM_ORDER_TYPE_FILLING.ORDER_FILLING_RETURN;
                //switch (cmbfilling.Text)
                //{
                //    case "RETURN":
                //        request.Type_filling = ENUM_ORDER_TYPE_FILLING.ORDER_FILLING_RETURN;
                //        break;
                //    case "FOK":
                //        request.Type_filling = ENUM_ORDER_TYPE_FILLING.ORDER_FILLING_FOK;
                //        break;
                //    case "IOC":
                //        request.Type_filling = ENUM_ORDER_TYPE_FILLING.ORDER_FILLING_IOC;
                //        break;
                //}

                if (TrType == "Buy")
                {
                    request.Type = ENUM_ORDER_TYPE.ORDER_TYPE_BUY;
                }
                else if (TrType == "Sell")
                {
                    request.Type = ENUM_ORDER_TYPE.ORDER_TYPE_SELL;
                }


                //Send Order and Get Result
                bool x = apiClient.OrderSend(request, out result);
                if (result != null)
                {
                    long answer = result.Retcode;
                    string ErrorCons;
                    string ErrorDef;
                    ErrorCode.getdes(answer, out ErrorCons, out  ErrorDef);
                    double xprice = result.Price;
                    string currenttime = DateTime.Now.ToShortTimeString();
                    if (cmbaction.Text == "PENDING" && ErrorCons == "TRADE_RETCODE_DONE")
                    {
                        ErrorDef = "Pending";
                        xprice = Convert.ToDouble(txtprice.Text);
                        Oitem = new OrderItems();
                        Oitem.price = xprice;
                        Oitem.symbol = SymbolName;
                        Oitem.volume = result.Volume;
                        Oitem.type = cmbordertype.Text;
                        Oitem.Orderticket = result.Order;
                        Oitem.time = currenttime;
                        Oitem.sender = "Me";
                        Oitem.state = "Unknown";
                        Ilist.Add(Oitem);
                    }


                    int nrow = dggrid.RowCount + 1;
                    dggrid.Rows.Add(nrow, SymbolName, xprice, result.Volume, result.Order, currenttime, ErrorDef, request.Type);
                    dggrid.FirstDisplayedScrollingRowIndex = dggrid.Rows.Count - 1;
                    results = ErrorDef;
                }
                else
                {
                    //MessageBox.Show("Unknown Error occured.");
                }

            }


            return results;
        }

        private void AlgoTrading()
        {
            string cscop = cmbScop.Text;
            string results = "";


            //for (int i = 0; i < clb_Symbols.Items.Count; i++)
            //{
            //    bool s = clb_Symbols.GetItemChecked(i);
            //    var insCode = ((Symbols)clb_Symbols.Items[i]).InsCode;
            //    Symbol.FindLast(t => t.InsCode == insCode).Selected = s;
            //}


            switch (cscop)
            {
                case "Daily":
                    Scop = 1;
                    break;
                case "1 Hour":
                    Scop = 2;
                    break;
                case "10 Minute":
                    Scop = 3;
                    break;
                case "1 Minute":
                    Scop = 4;
                    break;
            }

            foreach (var t in lstFinalSymbol.Items)
            {
                var s = (Symbols)t;
                Totalcapital = apiClient.AccountInfoDouble(ENUM_ACCOUNT_INFO_DOUBLE.ACCOUNT_EQUITY);

                textBox3.Text = Totalcapital.ToString();

                double[] Signal = { 0, 0, 0 };
                Signal = FindSignal(s.InsCode, Convert.ToInt16(lstperiod.Text));
                Int32 Volume = (Int32)(0.05 * Totalcapital / Signal[2]);

                if (Signal[0] > 0 && Signal[1] == 0)
                {
                    results = Trade(s.Symbol, Volume, "Buy");
                }
                else if (Signal[0] == 0 && Signal[1] > 0)
                {
                    results = Trade(s.Symbol, Volume, "Sell");
                }
                string currenttime = DateTime.Now.ToShortTimeString();
                dgSignalViewer.Rows.Add(s.Symbol, Signal[0], Signal[1], Signal[2], results, currenttime);

            }

        }

        private double[] FindSignal(string InsCode, int TPeriod)
        {
            string alg = cmbalg.Text;
            double[] Signal = { 0, 0, 0 };
            int Num = TPeriod + 50;
            if (alg.Contains("Alligator"))
            {
                sqlCmd = @"SELECT TOP 1 T.DEven, T.HEven, T.price, T.Buy, T.Sell FROM TseTrade.dbo.fn_AT_IND_Alligator_SIGNALS(" + InsCode + ",13,8,5,8,5,3," + TPeriod + "," + Scop + "," + Num + ") T ORDER BY T.DEven DESC,T.HEven DESC";
            }
            else if (alg.Contains("MACD"))
            {
                sqlCmd = @"SELECT TOP 1 T.DEven, T.HEven, T.price, T.Buy, T.Sell FROM TseTrade.dbo.fn_AT_IND_MACD_SIGNALS(" + InsCode + ",26,12,9," + TPeriod + "," + Scop + "," + Num + ") T ORDER BY T.DEven DESC,T.HEven DESC";
            }
            else if (alg.Contains("RSI"))
            {
                sqlCmd = @"SELECT TOP 1 T.DEven, T.HEven, T.price, T.Buy, T.Sell FROM TseTrade.dbo.fn_AT_IND_RSI_SIGNALS(" + InsCode + ",14," + TPeriod + "," + Scop + "," + Num + ") T ORDER BY T.DEven DESC,T.HEven DESC";
            }
            else if (alg.Contains("CCI"))
            {
                sqlCmd = @"SELECT TOP 1 T.DEven, T.HEven, T.price, T.Buy, T.Sell FROM TseTrade.dbo.fn_AT_IND_CCI_SIGNALS(" + InsCode + ",14," + Scop + "," + Num + ") T ORDER BY T.DEven DESC,T.HEven DESC";
            }


            DataTable dtSignal = mySql.SqlExecuteReader(sqlCmd);
            Signal[0] = Convert.ToDouble(dtSignal.Rows[0]["Buy"]);
            Signal[1] = Convert.ToDouble(dtSignal.Rows[0]["Sell"]);
            Signal[2] = Convert.ToDouble(dtSignal.Rows[0]["Price"]);
            return Signal;
        }



        private void txtsymbolsearch_TextChanged(object sender, EventArgs e)
        {
            List<Symbols> tmpSymbol = new List<Symbols>();

            tmpSymbol = Symbol.FindAll(t => t.Symbol.Contains(txtsymbolsearch.Text));
            lstFirstSymbol.DataSource = tmpSymbol;
            lstFirstSymbol.ValueMember = "InsCode";
            lstFirstSymbol.DisplayMember = "Symbol";
            lstFirstSymbol.SelectedValue = "Selected";



        }


        private void btnoneright_Click(object sender, EventArgs e)
        {
            lstFinalSymbol.Items.Add((Symbols)lstFirstSymbol.SelectedItem);
            lstFinalSymbol.ValueMember = "InsCode";
            lstFinalSymbol.DisplayMember = "Symbol";
        }

        private void btnoneleft_Click(object sender, EventArgs e)
        {
            lstFinalSymbol.Items.Remove((Symbols)lstFinalSymbol.SelectedItem);
        }

        private void lstFirstSymbol_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            btnoneright_Click(sender, e);
        }


    }

}
