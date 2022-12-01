using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mtapi5test
{
    public class OrderItems
    {
        public ulong Orderticket;
        public string symbol;
        public double price;
        public string type;
        public double volume;

        
        public string time;
        public string state;
        public string sender;


        public bool Equals(OrderItems a)
        {
            if (    a.Orderticket == Orderticket 
                &&  a.price == price 
                &&  a.type == type
                &&  a.symbol == symbol 
                &&  a.volume == volume)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public static bool Contains(List<OrderItems> L, OrderItems o)
        {
            bool found = false;
            foreach (var l in L)
            {
                if (o.Equals(l))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }
    }
}
