using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DP.Data.Common
{
    public class SortOrder
    {
        public SortOrder(string orderName, OrderDirection orderDirection)
        {
            _orderDirection = orderDirection;
            _orderName = orderName;
        }

        private string _orderName = "";

        private OrderDirection _orderDirection = OrderDirection.ASC;

        public OrderDirection OrderDirection
        {
            get { return _orderDirection; }
            set { _orderDirection = value; }
        }

        public string OrderName
        {
            get { return _orderName; }
            set { _orderName = value; }
        }
    }
}
