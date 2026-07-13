using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MiComanderaApp.Core.Application.Request
{
    public class ProductoRequest
    {

        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public double Price { get; set; }
        public bool IsAvailable { get; set; }
        public string ImageUrl { get; set; } = "";
        public string CategoryName { get; set; } = "";

    }
}