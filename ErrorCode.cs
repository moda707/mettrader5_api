using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mtapi5test
{
    public static class ErrorCode
    {   
        public static void getdes(long ecode, out string econst, out string edef)
        {
            econst = "UnIdentified Error";
            edef = "";

            switch (ecode)
            {
                //--- requote
                case 10004:
                    {
                        econst = "TRADE_RETCODE_REQUOTE";
                        edef = "Requote";
                        break;
                    }
                //--- order is not accepted by the server
                case 10006:
                    {
                        econst = "TRADE_RETCODE_REJECT";
                        edef = "Request rejected";
                        break;
                    }
                case 10007:
                    {
                        econst = "TRADE_RETCODE_CANCEL";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10008:
                    {
                        econst = "TRADE_RETCODE_PLACED";
                        edef = "Order placed";
                        break;
                    }
                case 10009:
                    {
                        econst = "TRADE_RETCODE_DONE";
                        edef = "Request completed";
                        break;
                    }
                case 100010:
                    {
                        econst = "TRADE_RETCODE_DONE_PARTIAL";
                        edef = "Only part of the request was completed";
                        break;
                    }////////////////
                case 10011:
                    {
                        econst = "TRADE_RETCODE_CANCEL";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10012:
                    {
                        econst = "TRADE_RETCODE_CANCEL";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10013:
                    {
                        econst = "TRADE_RETCODE_CANCEL";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10014:
                    {
                        econst = "TRADE_RETCODE_INVALID_VOLUME";
                        edef = "Request canceled by trader";
                        break;
                    }
                //--- invalid price
                case 10015:
                    {
                        econst = "TRADE_RETCODE_INVALID_PRICE";
                        edef = "Request canceled by trader";
                        break;
                    }
                //--- invalid SL and/or TP
                case 10016:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10017:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10018:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                //--- not enough money for a trade operation 
                case 10019:
                    {
                        econst = "TRADE_RETCODE_NO_MONEY";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10020:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10021:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10022:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10023:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10024:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10025:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10026:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10027:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10028:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10029:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10030:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10031:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10032:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10033:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }
                case 10034:
                    {
                        econst = "TRADE_RETCODE_INVALID_STOPS";
                        edef = "Request canceled by trader";
                        break;
                    }                            
            }
        }
        
    }

    
    
}
