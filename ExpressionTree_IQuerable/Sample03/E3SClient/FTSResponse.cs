﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample03.E3SClient
{
	public class Item<T>
	{
		public T data { get; set; }

	}

	public class FTSResponse<T> where T : class
	{
		public int total { get; set; }
	    //public int took { get; set; }
     //   public int facets { get; set; }

        public List<Item<T>> items { get; set; }
	}
}
