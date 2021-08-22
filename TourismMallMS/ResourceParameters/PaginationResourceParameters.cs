using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TourismMallMS.ResourceParameters
{
    public class PaginationResourceParameters
    {
        private const int MAXPAGESIZE = 50;
        private int _pageNumber = 1;
        public int PageNumber
        {
            get
            { return _pageNumber; }
            set
            {
                if (value >= 1)
                {
                    _pageNumber = value;
                }
            }
        }
        private int _pageSize = 10;
        public int PageSize
        {
            get
            { return _pageSize; }
            set
            {
                if (value >= 1)
                {
                    _pageSize = (value > MAXPAGESIZE) ? MAXPAGESIZE : value;
                }
            }
        }
    }
}
